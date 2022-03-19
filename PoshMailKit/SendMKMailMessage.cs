using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using MimeKit;
using MimeKit.Text;
using PoshMailKit.Internals;
using MailKit;

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

        [Parameter]
        public string[] ReplyTo { get; set; }

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

        // Legacy/Modern Delivery Notification Support
        [Parameter(ParameterSetName = "Legacy")]
        public DeliveryNotificationOptions DeliveryNotificationOption { get; set; }

        [Parameter(ParameterSetName = "Modern")]
        public DeliveryStatusNotification? DeliveryStatusNotification { get; set; }

        // Legacy/Modern Priority support
        [Parameter(ParameterSetName = "Legacy")]
        public MailPriority Priority { get; set; }

        [Parameter(ParameterSetName = "Modern")]
        public MessagePriority MessagePriority { get; set; } = MessagePriority.Normal;

        // Legacy/Modern Encoding support
        [Parameter(ParameterSetName = "Legacy")]
        public Encoding Encoding { get; set; }

        [Parameter(ParameterSetName = "Modern")]
        public System.Text.Encoding CharsetEncoding { get; set; } = System.Text.Encoding.UTF8;

        [Parameter(ParameterSetName = "Modern")]
        public ContentEncoding ContentTransferEncoding { get; set; } = ContentEncoding.Base64;
        #endregion

        private MessageBuilder MailMessage { get; set; }
        private List<MimePart> FilesToAttach { get; set; }
        private TextFormat BodyFormat
        { 
            get { return BodyAsHtml ? TextFormat.Html : TextFormat.Plain; }
        }

        protected override void BeginProcessing()
        {
            ProcessParameterSets();
            ProcessAttachments();
        }

        protected override void ProcessRecord()
        {
            MailMessage = new MessageBuilder
            {
                Subject = Subject,
                Priority = MessagePriority,
                From = From,
                To = To ?? null,
                Cc = Cc ?? null,
                Bcc = Bcc ?? null,
                ReplyTo = ReplyTo ?? null,
            };

            MailMessage.NewMailBody(BodyFormat, CharsetEncoding, Body, ContentTransferEncoding);
            MailMessage.AddAttachments(FilesToAttach);

            SmtpProcessor processor = new SmtpProcessor()
            {
                SmtpServer = SmtpServer,
                SmtpPort = Port,
                Message = MailMessage.Message,
                Notification = DeliveryStatusNotification,
            };

            processor.SendMailMessage();
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
                SetLegacyNotification();
            }
        }

        private void ProcessAttachments()
        {
            string workingDirectory = SessionState.Path.CurrentFileSystemLocation.Path;
            FileProcessor fileProcessor = new FileProcessor(workingDirectory);

            FilesToAttach = new List<MimePart>();

            if (Attachments != null)
            {
                ContentDispositionType attachmentContent = ContentDispositionType.Attachment;
                foreach (string file in Attachments)
                {
                    MimePart fileMimePart = fileProcessor.GetFileMimePart(file, attachmentContent);
                    FilesToAttach.Add(fileMimePart);
                }
            }

            if (InlineAttachments != null)
            {
                ContentDispositionType inlineContent = ContentDispositionType.Inline;
                foreach (string label in InlineAttachments.Keys)
                {
                    string file = (string)InlineAttachments[label];
                    MimePart fileMimePart = fileProcessor.GetFileMimePart(file, inlineContent, label);
                    FilesToAttach.Add(fileMimePart);
                }
            }
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

        private void SetLegacyNotification()
        {
            // Translate notification; default is null and does nothing
            switch (DeliveryNotificationOption)
            {
                case DeliveryNotificationOptions.OnSuccess:
                    DeliveryStatusNotification = MailKit.DeliveryStatusNotification.Success;
                    break;
                case DeliveryNotificationOptions.OnFailure:
                    DeliveryStatusNotification = MailKit.DeliveryStatusNotification.Failure;
                    break;
                case DeliveryNotificationOptions.Delay:
                    DeliveryStatusNotification = MailKit.DeliveryStatusNotification.Delay;
                    break;
                case DeliveryNotificationOptions.Never:
                    DeliveryStatusNotification = MailKit.DeliveryStatusNotification.Never;
                    break;
            }
        }
    }
}
