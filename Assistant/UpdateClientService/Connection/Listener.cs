using System.Net.Sockets;
using LibSocket.Connection;

namespace UpdateClientService.Connection;

public sealed class Listener
{
    private static ListenerClient? _instance;
    private static readonly object Lock = new();
    
    private static readonly AddressFamily AddressFamily = AddressFamily.InterNetwork;
    private static readonly SocketType SocketType = SocketType.Stream;
    private static readonly ProtocolType ProtocolType = ProtocolType.Tcp;

    private Listener() {} // Impede instâncias diretas

    public static ListenerClient InstanceListenerClient()
    {
        if (_instance != null) return _instance;

        lock (Lock)
        {
            _instance ??= new ListenerClient(
                AddressFamily, SocketType, ProtocolType);
        }

        return _instance;
    }
}