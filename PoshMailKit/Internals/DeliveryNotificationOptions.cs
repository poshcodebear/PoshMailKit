namespace PoshMailKit.Internals;

[Flags]
public enum DeliveryNotificationOptions
{
    None = 0x0,
    OnSuccess = 0x1,
    OnFailure = 0x2,
    Delay = 0x4,
    Never = 0x0,
}
