using MailKit;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoshMailKit.Internals
{
    public class PMKSmtpClient : SmtpClient
    {
		public DeliveryStatusNotification? DeliveryStatusNotification { get; set; }

		protected override DeliveryStatusNotification? GetDeliveryStatusNotifications(MimeMessage message, MailboxAddress mailbox)
		{
			return DeliveryStatusNotification;
		}
	}
}
