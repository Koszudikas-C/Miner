namespace LibCommunicationStatus;

public class CommunicationStatus
{
    public static bool IsReceiving { get; private set; }
    public static bool IsSending { get; private set; }

    public static void SetReceiving(bool status)
    {
        IsReceiving = status;
    }

    public static void SetSending(bool status)
    {
        IsSending = status;
    }
}