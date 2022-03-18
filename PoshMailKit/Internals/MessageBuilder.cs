using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;

namespace PoshMailKit.Internals
{
    public class MessageBuilder
    {
        // Properties
        private MimeMessage message { get; set; }
        private TextPart TextMailBody { get; set; }
        private Multipart MultipartMailBody { get; set; }

        // Setters
        public string Subject { set => message.Subject = value; }
        public MessagePriority Priority { set => message.Priority = value; }
        public string From { set => message.From.Add(GetMailboxAddressObj(value)); }
        public string[] To { set {
                if (value != null)
                    foreach (var v in value)
                        message.To.Add(GetMailboxAddressObj(v)); } }
        public string[] Cc { set {
                if (value != null)
                    foreach (var v in value)
                        message.Cc.Add(GetMailboxAddressObj(v)); } }
        public string[] Bcc { set {
                if (value != null)
                    foreach (var v in value)
                        message.Bcc.Add(GetMailboxAddressObj(v)); } }

        public MimeMessage Message
        {
            get
            {
                MimeMessage mimeMessage = message;
                Multipart multipart = MultipartMailBody;

                if (multipart != null)
                {
                    multipart.Add(TextMailBody);
                    mimeMessage.Body = multipart;
                }
                else if (TextMailBody != null)
                    mimeMessage.Body = TextMailBody;
                return mimeMessage;
            }
        }

        public MessageBuilder()
        {
            message = new MimeMessage();
        }

        private MailboxAddress GetMailboxAddressObj(string email)
        {
            return new MailboxAddress("", email);
        }

        public void AddAttachments(List<MimePart> filesToAttach)
        {
            if (filesToAttach != null && filesToAttach.Count > 0)
            {
                if (MultipartMailBody == null)
                {
                    MultipartMailBody = new Multipart("mixed");
                }

                foreach (MimePart attachment in filesToAttach)
                    MultipartMailBody.Add(attachment);
            }
        }

        public void NewMailBody(TextFormat format,
                                 System.Text.Encoding charsetEncoding,
                                 string body,
                                 ContentEncoding contentTransferEncoding)
        {
            TextMailBody = new TextPart(format);
            TextMailBody.ContentTransferEncoding = contentTransferEncoding;
            if (body != null)
                TextMailBody.SetText(charsetEncoding, body);
        }
    }
}
