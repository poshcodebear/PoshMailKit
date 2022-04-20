using MailKit;
using MailKit.Net.Smtp;
using MimeKit;

namespace PoshMailKit.Internals;

public class PMKSmtpClient : SmtpClient
{
	public DeliveryStatusNotification DeliveryStatusNotification { get; set; }

	protected override DeliveryStatusNotification? GetDeliveryStatusNotifications(MimeMessage message, MailboxAddress mailbox)
	{
		return DeliveryStatusNotification;
	}
}
