using System.Net.Sockets;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocket.Connection;
using LibSocketAndSslStream.Entities.Enum;

namespace WorkClientBlockChain.FilterException;

public class ClientException(string message) : Exception(message)
{
    public void ClientReconnect(ClientInfo clientInfo)
    {
        var listener = new ListenerClient(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);

        listener.ReconnectAsync(TypeAuthMode.RequireAuthentication,
            (uint)clientInfo.SocketWrapper!.PortRemote, 0).Wait();
    }
}
