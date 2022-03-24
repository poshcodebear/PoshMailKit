namespace PoshMailKit.Internals;

public enum DeliveryNotificationOptions
{
    None,
    OnSuccess,
    OnFailure,
    Delay,
    Never = 134217728,
}
