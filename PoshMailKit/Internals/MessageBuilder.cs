using MimeKit;
using MimeKit.Text;
using System.Text.RegularExpressions;

namespace PoshMailKit.Internals;

public class MessageBuilder
{
    // Properties
    private TextPart TextMailBody { get; set; }
    private Multipart MultipartMailBody { get; set; }

    private static readonly Regex EmailWithDisplayNamePattern = new("^(?<DisplayName>[^<]+)<(?<Email>[^>]+)>$");

    // Setters
    public string Subject { set => message.Subject = value; }
    public MessagePriority Priority { set => message.Priority = value; }
    public string From { set => message.From.Add(GetMailboxAddressObj(value)); }
    public string[] To { set {
            if (value is not null)
                foreach (var v in value)
                    message.To.Add(GetMailboxAddressObj(v)); } }
    public string[] Cc { set {
            if (value is not null)
                foreach (var v in value)
                    message.Cc.Add(GetMailboxAddressObj(v)); } }
    public string[] Bcc { set {
            if (value is not null)
                foreach (var v in value)
                    message.Bcc.Add(GetMailboxAddressObj(v)); } }
    public string[] ReplyTo { set {
            if (value is not null)
                foreach (var v in value)
                    message.ReplyTo.Add(GetMailboxAddressObj(v)); } }

    private readonly MimeMessage message;
    public MimeMessage Message
    {
        get
        {
            MimeMessage mimeMessage = message;
            Multipart multipart = MultipartMailBody;

            if (multipart is not null)
            {
                if (TextMailBody is not null)
                    multipart.Add(TextMailBody);
                mimeMessage.Body = multipart;
            }
            else if (TextMailBody is not null)
                mimeMessage.Body = TextMailBody;
            return mimeMessage;
        }
    }

    public MessageBuilder()
    {
        message = new();
    }

    public static MailboxAddress GetMailboxAddressObj(string email)
    {
        string displayName = "";
        Match regexResult = EmailWithDisplayNamePattern.Match(email);
        if (regexResult.Success)
        {
            displayName = regexResult.Groups["DisplayName"].Value.Trim();
            email = regexResult.Groups["Email"].Value;
        }

        MailboxAddress mailboxAddress = new(displayName, email);

        // Test to ensure valid mail addersses
        var _ = ((System.Net.Mail.MailAddress)mailboxAddress);

        return mailboxAddress;
    }

    public void AddAttachments(List<MimePart> filesToAttach)
    {
        if (filesToAttach is not null && filesToAttach.Count > 0)
        {
            if (MultipartMailBody is null)
            {
                MultipartMailBody = new("mixed");
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
        TextMailBody = new(format)
        {
            ContentTransferEncoding = contentTransferEncoding
        };
        if (body is not null)
            TextMailBody.SetText(charsetEncoding, body);
    }
}
