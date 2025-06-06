namespace LibCommunicationStateClient.Entities;

public class RemoteOpenStatus (bool isConnected, int port)
{
    public bool IsConnected { get; set; } = isConnected;
    public int Port { get; set; } = port;
}
