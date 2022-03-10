using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace PSMailKit
{
    [Cmdlet(VerbsCommunications.Send, "MKMailMessage")]
    public class SendMKMailMessage : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string[] To { get; set; }
        
        [Parameter]
        public string Subject { get; set; }
        
        [Parameter]
        public string Body { get; set; }
        
        // Note: Send-MailMessage does not require this if variable $PSEmailServer is set; should support this ultimately
        [Parameter(Mandatory = true)]
        public string SmtpServer { get; set; }
        
        [Parameter(Mandatory = true)]
        public string From { get; set; }
        
        [Parameter]
        public int Port { get; set; }

        protected override void ProcessRecord()
        {
            MimeMessage message = new MimeMessage();

            foreach (string recipient in To)
                message.To.Add(new MailboxAddress("", recipient));
            message.From.Add(new MailboxAddress("", From));

            if (Subject != null)
                message.Subject = Subject;

            if (Body != null)
                message.Body = new TextPart("plain") {Text = Body};

            using (SmtpClient client = new SmtpClient())
            {
                //SecureSocketOptions secureSocketOptions = SecureSocketOptions.None;
                //client.Connect(SmtpServer, Port, secureSocketOptions);
                client.Connect(SmtpServer, Port, SecureSocketOptions.None);

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("joey", "password");

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
