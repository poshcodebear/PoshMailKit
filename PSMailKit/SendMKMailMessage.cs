﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using MimeKit;
using MimeKit.Text;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace PSMailKit
{
    [Cmdlet(
        VerbsCommunications.Send, "MKMailMessage",
        DefaultParameterSetName = "Default")]
    public class SendMKMailMessage : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string[] To { get; set; }

        [Parameter]
        public string[] Cc { get; set; }

        [Parameter]
        public string[] Bcc { get; set; }

        [Parameter]
        public string Subject { get; set; }

        [Parameter]
        public string Body { get; set; }

        [Parameter]
        public SwitchParameter BodyAsHtml { get; set; }
        
        // Note: Send-MailMessage does not require this if variable $PSEmailServer is set; should support this ultimately
        [Parameter(Mandatory = true)]
        public string SmtpServer { get; set; }
        
        [Parameter(Mandatory = true)]
        public string From { get; set; }
        
        [Parameter]
        public int Port { get; set; }

        // Note: as other legacy compatibility options get added, we'll have to figure out how to handle selection without using Mandatory
        [Parameter(
            ParameterSetName = "Legacy",
            Mandatory = true)]
        public MailPriority Priority { get; set; }

        [Parameter(
            ParameterSetName = "Modern",
            Mandatory = true)]
        public MessagePriority MessagePriority { get; set; }

        protected override void BeginProcessing()
        {
            if (ParameterSetName == "Default")
            {
                MessagePriority = MessagePriority.Normal;
            }
            else if (ParameterSetName == "Legacy")
            {
                switch (Priority)
                {
                    case MailPriority.Low:
                        MessagePriority = MessagePriority.NonUrgent;
                        break;
                    case MailPriority.High:
                        MessagePriority = MessagePriority.Urgent;
                        break;
                    case MailPriority.Normal:
                        MessagePriority = MessagePriority.Normal;
                        break;
                }
            }
        }

        protected override void ProcessRecord()
        {   
            MimeMessage message = new MimeMessage();

            message.From.Add(new MailboxAddress("", From));

            foreach (string toMail in To)
                message.To.Add(new MailboxAddress("", toMail));

            if (Cc != null)
                foreach (string ccMail in Cc)
                    message.Cc.Add(new MailboxAddress("", ccMail));

            if (Bcc != null)
                foreach (string bccMail in Bcc)
                    message.Bcc.Add(new MailboxAddress("", bccMail));

            if (Subject != null)
                message.Subject = Subject;

            TextFormat bodyTextFormat;
            if (BodyAsHtml)
                bodyTextFormat = TextFormat.Html;
            else
                bodyTextFormat = TextFormat.Plain;

            if (Body != null)
                message.Body = new TextPart(bodyTextFormat) {Text = Body};

            message.Priority = MessagePriority;

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
