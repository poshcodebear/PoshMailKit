using MimeKit;
using MailKit.Security;
using MailKit;

namespace PoshMailKit.Internals
{
    public class SmtpProcessor
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public MimeMessage Message { get; set; }
        public PMKSmtpClient Client { get; set; }
        public DeliveryStatusNotification? Notification { get; set; }
        public SecureSocketOptions SecureSocketOptions { get; set; }

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
            if (SmtpServer != null && Message != null)
            {
                Client.Connect(SmtpServer, SmtpPort, SecureSocketOptions);
                Client.Send(Message);
                Client.Disconnect(true);
            }
        }

        // Leaving here temporarily while implementing more functionality
        /*public static void SendMailMessage(string smtpServer, int port, MimeMessage message)
        {
            using (PMKSmtpClient client = new PMKSmtpClient())
            {
                //SecureSocketOptions secureSocketOptions = SecureSocketOptions.None;
                //client.Connect(SmtpServer, Port, secureSocketOptions);

                client.Connect(smtpServer, port, SecureSocketOptions.None);                

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("joey", "password");

                client.Send(message);
                client.Disconnect(true);
            }
        }*/
    }
}
