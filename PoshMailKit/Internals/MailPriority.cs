namespace PoshMailKit.Internals
{
    // This is for legacy compatibility to avoid using the System.Net.Mail enum of the same name
    public enum MailPriority
    {
        Normal = 0,
        Low = 1, // -> MessagePriority.NonUrgent
        High = 2 // -> MessagePriority.Urgent
    }
}