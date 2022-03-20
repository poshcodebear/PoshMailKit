using MimeKit;
using MimeKit.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PoshMailKit.Internals
{
    public class MessageBuilder
    {
        // Properties
        private MimeMessage message { get; set; }
        private TextPart TextMailBody { get; set; }
        private Multipart MultipartMailBody { get; set; }

        private static Regex EmailWithDisplayNamePattern = new Regex("^(?<DisplayName>[^<]+)<(?<Email>[^>]+)>$");

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
        public string[] ReplyTo { set {
                if (value != null)
                    foreach (var v in value)
                        message.ReplyTo.Add(GetMailboxAddressObj(v)); } }

        public MimeMessage Message
        {
            get
            {
                MimeMessage mimeMessage = message;
                Multipart multipart = MultipartMailBody;

                if (multipart != null)
                {
                    if (TextMailBody != null)
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

        public MailboxAddress GetMailboxAddressObj(string email)
        {
            string displayName = "";
            var regexResult = EmailWithDisplayNamePattern.Match(email);
            if (regexResult.Success)
            {
                displayName = regexResult.Groups["DisplayName"].Value.Trim();
                email = regexResult.Groups["Email"].Value;
            }

            MailboxAddress mailboxAddress = new MailboxAddress(displayName, email);
            var _ = ((System.Net.Mail.MailAddress)mailboxAddress);
            return mailboxAddress;
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
