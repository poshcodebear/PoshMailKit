using System.Collections;
using System.Management.Automation;
using MimeKit;
using MimeKit.Text;
using PoshMailKit.Internals;
using MailKit;
using MailKit.Security;
using System.Net;

namespace PoshMailKit;

[Cmdlet(
    VerbsCommunications.Send, "MKMailMessage",
    DefaultParameterSetName = "Modern")]
public class SendMKMailMessage : PSCmdlet
{
    #region Cmdlet parameters

    #region Parameter Set: All

    #region Parameter: Attachments
    [Alias("PsPath")]
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public string[] Attachments { get; set; }
    #endregion

    #region Parameter: Bcc
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public string[] Bcc { get; set; }
    #endregion

    #region Parameter: Body
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true,
        Position = 2)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true,
        Position = 2)]
    public string Body { get; set; }
    #endregion

    #region Parameter: Cc
    [Parameter
        (ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public string[] Cc { get; set; }
    #endregion

    #region Parameter: Credential
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public PSCredential Credential { get; set; }
    #endregion

    #region Parameter: From
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true,
        Mandatory = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true,
        Mandatory = true)]
    public string From { get; set; }
    #endregion

    #region Parameter: InlineAttachments
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public Hashtable InlineAttachments { get; set; }
    #endregion

    #region Parameter: Port
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public int Port { get; set; } = 25;
    #endregion

    #region Parameter: ReplyTo
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public string[] ReplyTo { get; set; }
    #endregion

    #region Parameter: SmtpServer
    [Alias("ComputerName")]
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true,
        Position = 3)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true,
        Position = 3)]
    public string SmtpServer { get; set; }
    #endregion

    #region Parameter: Subject
    [Alias("sub")]
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true,
        Position = 1)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true,
        Position = 1)]
    public string Subject { get; set; }
    #endregion

    #region Parameter: To
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true,
        Mandatory = true,
        Position = 0)]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true,
        Mandatory = true,
        Position = 0)]
    public string[] To { get; set; }
    #endregion

    #endregion

    #region Parameter Set: Modern

    #region Parameter: BodyFormat
    // Legacy counterpart: -BodyAsHtml
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    public TextFormat BodyFormat { get; set; } = TextFormat.Plain;
    #endregion

    #region Parameter: CharsetEncoding
    // Legacy counterpart: -Encoding (for both -CharsetEncoding and -ContentTransferEncoding)
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    public System.Text.Encoding CharsetEncoding { get; set; } = System.Text.Encoding.UTF8;
    #endregion

    #region Parameter: ContentTransferEncoding
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    public ContentEncoding ContentTransferEncoding { get; set; } = ContentEncoding.Base64;
    #endregion

    #region Parameter: DeliveryStatusNotification
    // Legacy counterpart: -DeliveryNotificationOptions
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    public DeliveryStatusNotification? DeliveryStatusNotification { get; set; }
    #endregion

    #region Parameter: MessagePriority
    // Legacy counterpart: -Priority
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    public MessagePriority MessagePriority { get; set; } = MessagePriority.Normal;
    #endregion

    #region Parameter: RequireSecureConnection
    // Legacy counterpart: -UseSsl
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    public SwitchParameter RequireSecureConnection { get; set; }
    #endregion

    #region Parameter: SecureSocketOptions
    // Legacy counterpart: -UseSsl
    [Parameter(
        ParameterSetName = "Modern",
        ValueFromPipelineByPropertyName = true)]
    public SecureSocketOptions SecureSocketOptions { get; set; } = SecureSocketOptions.Auto;
    #endregion

    #endregion

    #region Parameter Set: Legacy

    #region Parameter: Legacy
    // Forces processing into Legacy mode
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public SwitchParameter Legacy { get; set; }
    #endregion

    #region Parameter: BodyAsHtml
    // Modern counterpart: -BodyFormat
    [Alias("BAH")]  
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public SwitchParameter BodyAsHtml { get; set; }
    #endregion

    #region Parameter: DeliveryNotificationOption
    // Modern counterpart: -DeliveryStatusNotification
    [Alias("DNO")]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public DeliveryNotificationOptions DeliveryNotificationOption { get; set; }
    #endregion

    #region Parameter: Encoding
    // Modern counterparts: -CharsetEncoding and -ContentTransferEncoding
    [Alias("BE")]
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public Encoding Encoding { get; set; }
    #endregion

    #region Parameter: Priority
    // Modern counterpart: -MessagePriority
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public MailPriority Priority { get; set; }
    #endregion

    #region Parameter: UseSsl
    // Modern counterpart: -SecureSocketOptions Auto -RequireSecureConnection
    [Parameter(
        ParameterSetName = "Legacy",
        ValueFromPipelineByPropertyName = true)]
    public SwitchParameter UseSsl { get; set; }
    #endregion

    #endregion

    #endregion

    private List<MimePart> FilesToAttach { get; set; }

    protected override void BeginProcessing()
    {
        ProcessParameters();
        ProcessAttachments();
    }

    protected override void ProcessRecord()
    {
        MessageBuilder MailMessageBuilder = new()
        {
            Subject = Subject,
            Priority = MessagePriority,
            From = From,
            To = To,
            Cc = Cc,
            Bcc = Bcc,
            ReplyTo = ReplyTo,
        };

        MailMessageBuilder.NewMailBody(BodyFormat, CharsetEncoding, Body, ContentTransferEncoding);
        MailMessageBuilder.AddAttachments(FilesToAttach);

        SmtpProcessor processor = new()
        {
            SmtpServer = SmtpServer,
            SmtpPort = Port,
            SecureSocketOptions = SecureSocketOptions,
            Message = MailMessageBuilder.Message,
            Notification = DeliveryStatusNotification,
            RequireSecureConnection = RequireSecureConnection,
        };

        if (Credential is not null)
            processor.Credential = (NetworkCredential)Credential;

        try
        {
            processor.SendMailMessage();
        }
        catch (Exception ex)
        {
            string errorId = "UnableToSendToServer";
            ErrorCategory category = ErrorCategory.OperationStopped;
            if (ex is InvalidOperationException)
            {
                errorId = "SecureConnectionRequirementsNotMet";
                category = ErrorCategory.SecurityError;
            }

            ErrorRecord errorRecord = new ErrorRecord(ex, errorId, category, processor);
            string errorDetails = $"{SmtpServer}:{Port} (SSO:{SecureSocketOptions})";
            errorRecord.ErrorDetails = new ErrorDetails($"{ex.Message} ({errorDetails})");
            WriteError(errorRecord);
        }
    }

    private void ProcessParameters()
    {
        if (string.IsNullOrEmpty(SmtpServer))
            SmtpServer = (string)SessionState.PSVariable.Get("PSEmailServer").Value;
        if (string.IsNullOrEmpty(SmtpServer))
        {
            string errorMessage = "The email cannot be sent because no SMTP server was specified. " +
                "You must specify an SMTP server by using either the SmtpServer parameter or the $PSEmailServer variable.";
            InvalidOperationException exception = new(errorMessage);
            ErrorRecord errorRecord = new(exception, "", ErrorCategory.InvalidArgument, null);
            ThrowTerminatingError(errorRecord);
        }

        if (ParameterSetName == "Legacy")
        {
            SetLegacySsl();
            SetLegacyPriority();
            SetLegacyEncoding();
            SetLegacyNotification();
            SetLegacyBodyFormat();
        }
    }

    private void ProcessAttachments()
    {
        string workingDirectory = SessionState.Path.CurrentFileSystemLocation.Path;
        FileProcessor fileProcessor = new(workingDirectory);

        FilesToAttach = new List<MimePart>();

        if (Attachments is not null)
        {
            ContentDispositionType attachmentContent = ContentDispositionType.Attachment;
            foreach (string file in Attachments)
            {
                MimePart fileMimePart = fileProcessor.GetFileMimePart(file, attachmentContent);
                FilesToAttach.Add(fileMimePart);
            }
        }

        if (InlineAttachments is not null)
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

    private void SetLegacySsl()
    {
        if (UseSsl)
            RequireSecureConnection = true;
        else
            SecureSocketOptions = SecureSocketOptions.None;
    }

    private void SetLegacyPriority()
    {
        MessagePriority = Priority switch
        {
            MailPriority.Low  => MessagePriority.NonUrgent,
            MailPriority.High => MessagePriority.Urgent,
            _                 => MessagePriority.Normal,
        };
    }

    private void SetLegacyEncoding()
    {
        ContentTransferEncoding = Encoding switch
        {
            Encoding.BigEndianUnicode or Encoding.Unicode or Encoding.UTF8BOM or Encoding.UTF32
                => ContentEncoding.Base64,
            _   => ContentEncoding.QuotedPrintable,
        };

        CharsetEncoding = Encoding switch
        {
            Encoding.ASCII            => System.Text.Encoding.ASCII,
            Encoding.BigEndianUnicode => System.Text.Encoding.BigEndianUnicode,
            Encoding.BigEndianUTF32   => System.Text.Encoding.GetEncoding("utf-32BE"),
            Encoding.Unicode          => System.Text.Encoding.Unicode,
            Encoding.UTF32            => System.Text.Encoding.UTF32,
            Encoding.UTF8 or
            Encoding.UTF8NoBOM or
            Encoding.UTF8BOM or
            _                         => System.Text.Encoding.UTF8,
        };
    }

    private void SetLegacyNotification()
    {
        DeliveryStatusNotification = DeliveryNotificationOption switch
        {
            DeliveryNotificationOptions.OnSuccess => MailKit.DeliveryStatusNotification.Success,
            DeliveryNotificationOptions.OnFailure => MailKit.DeliveryStatusNotification.Failure,
            DeliveryNotificationOptions.Delay     => MailKit.DeliveryStatusNotification.Delay,
            _                                     => MailKit.DeliveryStatusNotification.Never,
        };
    }

    private void SetLegacyBodyFormat()
    {
        if (BodyAsHtml)
            BodyFormat = TextFormat.Html;
    }
}
