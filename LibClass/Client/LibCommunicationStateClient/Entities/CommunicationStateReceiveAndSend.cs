using LibCommunicationStateClient.Entities;

namespace LibCommunicationStateClient.Entities;

public static class CommunicationStateReceiveAndSend
{
    public static bool IsReceiving { get; private set; }
    public static bool IsSending { get; private set; }
    public static bool IsConnected { get; private set; }
    public static bool IsConnecting { get; private set; }
    
    public static bool IsAuthenticated { get; private set; }
    
    public static List<RemoteOpenStatus> PortOpen { get; private set; } = [];
    
    public static void SetReceiving(bool status)
    {
        IsReceiving = status;
    }

    public static void SetSending(bool status)
    {
        IsSending = status;
    }
    
    public static void SetConnected(bool status)
    {
        IsConnected = status;
    }
    
    public static void SetConnecting(bool status)
    {
        IsConnecting = status;
    }
    
    public static void SetAuthenticated(bool status)
    {
        IsAuthenticated = status;
    }
}
