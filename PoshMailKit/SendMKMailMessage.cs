using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit.Text;
using PoshMailKit.Internals;

namespace PoshMailKit
{
    [Cmdlet(
        VerbsCommunications.Send, "MKMailMessage",
        DefaultParameterSetName = "Default")]
    public class SendMKMailMessage : PSCmdlet
    {
        #region Cmdlet parameters
        [Parameter(
            Mandatory = true,
            Position = 0)]
        public string[] To { get; set; }

        [Parameter]
        public string[] Cc { get; set; }

        [Parameter]
        public string[] Bcc { get; set; }

        [Parameter(Position = 1)]
        public string Subject { get; set; }

        [Parameter(Position = 2)]
        public string Body { get; set; }

        [Parameter]
        public SwitchParameter BodyAsHtml { get; set; }

        [Parameter]
        public string[] Attachments { get; set; }

        [Parameter]
        public Hashtable InlineAttachments { get; set; }

        // Note: Send-MailMessage does not require this if variable $PSEmailServer is set; should support this ultimately
        [Parameter(
            Mandatory = true,
            Position = 3)]
        public string SmtpServer { get; set; }
        
        [Parameter(Mandatory = true)]
        public string From { get; set; }

        [Parameter]
        public int Port { get; set; } = 25;

        // Forces processing into Legacy mode
        [Parameter(ParameterSetName = "Legacy")]
        public SwitchParameter Legacy { get; set; }

        // Legacy/Modern Priority support
        [Parameter(ParameterSetName = "Legacy")]
        public Internals.MailPriority Priority { get; set; }

        [Parameter(ParameterSetName = "Modern")]
        public MessagePriority MessagePriority { get; set; } = MessagePriority.Normal;

        // Legacy/Modern Encoding support
        [Parameter(ParameterSetName = "Legacy")]
        public Encoding Encoding { get; set; }

        [Parameter(ParameterSetName = "Modern")]
        public System.Text.Encoding CharsetEncoding { get; set; }

        [Parameter(ParameterSetName = "Modern")]
        public ContentEncoding ContentTransferEncoding { get; set; }
        #endregion

        private MimeMessage Message { get; set; }
        private Multipart MultipartMailBody { get; set; }
        private TextPart MailBody { get; set; }
        private List<MimePart> FilesToAttach { get; set; }
        private TextFormat Format
        { 
            get { return BodyAsHtml ? TextFormat.Html : TextFormat.Plain; }
        }

        protected override void BeginProcessing()
        {
            ProcessAttachments();
            ProcessParameterSets();
        }

        protected override void ProcessRecord()
        {
            GenerateMailMessage();
            GenerateMailBody();
            SendMailMessage();
        }

        private void ProcessParameterSets()
        {
            if (ParameterSetName == "Default")
            {
                MessagePriority = MessagePriority.Normal;

                // Send-MailMessage default encoding is ASCII, but if not explicitly in Legacy mode, default to UTF-8 w/BOM
                // Not sure if this is a good idea for a default or not; ASCII is probably at least a bit more universally supported
                CharsetEncoding = System.Text.Encoding.UTF8;
                ContentTransferEncoding = ContentEncoding.Base64;
            }
            else if (ParameterSetName == "Legacy")
            {
                SetLegacyPriority();
                SetLegacyEncoding();
            }
        }

        private void ProcessAttachments()
        {
            string workingDirectory = SessionState.Path.CurrentFileSystemLocation.Path;
            FileProcessor fileProcessor = new FileProcessor(workingDirectory);
            FilesToAttach = new List<MimePart>();

            if (Attachments != null)
                foreach (string attachment in Attachments)
                    FilesToAttach.Add(fileProcessor.GetFileMimePart(attachment));

            if (InlineAttachments != null)
                foreach (string label in InlineAttachments.Keys)
                    FilesToAttach.Add(fileProcessor.GetFileMimePart(
                        (string)InlineAttachments[label],
                        new ContentDisposition(ContentDisposition.Inline),
                        label));
        }

        private void SetLegacyPriority()
        {
            // Translate priority; Enum default is Normal
            switch (Priority)
            {
                case MailPriority.Normal:
                    MessagePriority = MessagePriority.Normal;
                    break;
                case MailPriority.Low:
                    MessagePriority = MessagePriority.NonUrgent;
                    break;
                case MailPriority.High:
                    MessagePriority = MessagePriority.Urgent;
                    break;
            }
        }

        private void SetLegacyEncoding()
        {
            // Translate Encoding; Enum default is ASCII
            ContentTransferEncoding = ContentEncoding.QuotedPrintable;
            switch (Encoding)
            {
                case Encoding.ASCII:
                    CharsetEncoding = System.Text.Encoding.ASCII;
                    break;
                case Encoding.BigEndianUnicode:
                    CharsetEncoding = System.Text.Encoding.BigEndianUnicode;
                    ContentTransferEncoding = ContentEncoding.Base64;
                    break;
                case Encoding.BigEndianUTF32:
                    CharsetEncoding = System.Text.Encoding.GetEncoding("utf-32BE");
                    break;
                // Not going to support this for now
                /*case Encoding.OEM:
                    CharsetEncoding = System.Text.Encoding.Default; //Find
                    break;*/
                case Encoding.Unicode:
                    CharsetEncoding = System.Text.Encoding.Unicode;
                    ContentTransferEncoding = ContentEncoding.Base64;
                    break;
                case Encoding.UTF7:
                    CharsetEncoding = System.Text.Encoding.UTF7;
                    break;
                case Encoding.UTF8:
                    CharsetEncoding = System.Text.Encoding.UTF8;
                    break;
                case Encoding.UTF8BOM:
                    CharsetEncoding = System.Text.Encoding.UTF8;
                    ContentTransferEncoding = ContentEncoding.Base64;
                    break;
                case Encoding.UTF8NoBOM:
                    CharsetEncoding = System.Text.Encoding.UTF8;
                    break;
                case Encoding.UTF32:
                    CharsetEncoding = System.Text.Encoding.UTF32;
                    ContentTransferEncoding = ContentEncoding.Base64;
                    break;
            }
        }

        private void GenerateMailBody()
        {
            MailBody = new TextPart(Format);
            MailBody.SetText(CharsetEncoding, Body);
            MailBody.ContentTransferEncoding = ContentTransferEncoding;

            if (FilesToAttach.Count > 0)
            {
                MultipartMailBody = new Multipart("mixed");
                foreach (MimePart attachment in FilesToAttach)
                    MultipartMailBody.Add(attachment);
                MultipartMailBody.Add(MailBody);

                Message.Body = MultipartMailBody;
            }
            else
                Message.Body = MailBody;
        }
        
        private void GenerateMailMessage()
        {
            Message = new MimeMessage
            {
                Subject = Subject ?? null,
                Priority = MessagePriority,
            };

            Message.From.Add(new MailboxAddress("", From));

            foreach (string toMail in To)
                Message.To.Add(new MailboxAddress("", toMail));

            if (Cc != null)
                foreach (string ccMail in Cc)
                    Message.Cc.Add(new MailboxAddress("", ccMail));

            if (Bcc != null)
                foreach (string bccMail in Bcc)
                    Message.Bcc.Add(new MailboxAddress("", bccMail));
        }

        private void SendMailMessage()
        {
            using (SmtpClient client = new SmtpClient())
            {
                //SecureSocketOptions secureSocketOptions = SecureSocketOptions.None;
                //client.Connect(SmtpServer, Port, secureSocketOptions);
                client.Connect(SmtpServer, Port, SecureSocketOptions.None);

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("joey", "password");

                client.Send(Message);
                client.Disconnect(true);
            }
        }
    }
}
