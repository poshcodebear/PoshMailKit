using MimeKit;
using MailKit.Security;
using MailKit;
using System.Net;

namespace PoshMailKit.Internals;

public class SmtpProcessor
{
    public string SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public MimeMessage Message { get; set; }
    public PMKSmtpClient Client { get; set; }
    public DeliveryStatusNotification? Notification { get; set; }
    public SecureSocketOptions SecureSocketOptions { get; set; }
    public NetworkCredential Credential { get; set; }
    public bool RequireSecureConnection { get; set; }
    private bool SecureConnectionRequirementsMet
    {
        get
        {
            if (RequireSecureConnection && !Client.IsSecure)
                return false;
            return true;
        }
    }

    public SmtpProcessor(PMKSmtpClient client)
    {
        Client = client;
        SmtpPort = 25;
        SecureSocketOptions = SecureSocketOptions.Auto;
    }

    public SmtpProcessor()
        : this(new PMKSmtpClient())
    { }

    public void SendMailMessage()
    {
        Client.DeliveryStatusNotification = Notification;
        if (SmtpServer is not null && Message is not null)
        {
            Client.Connect(SmtpServer, SmtpPort, SecureSocketOptions);
            if (SecureConnectionRequirementsMet)
            {
                if (Credential is not null)
                    Client.Authenticate(Credential);
                Client.Send(Message);
            }
            else
                throw new System.InvalidOperationException("SecureConnection requirements not met, unable to send");
            Client.Disconnect(true);
        }
    }
}
