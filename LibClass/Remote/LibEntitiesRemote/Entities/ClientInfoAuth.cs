using System.Net.Sockets;
using System.Text.Json.Serialization;
using LibEntitiesRemote.Interface;

namespace LibEntitiesRemote.Entities;

public class ClientInfoAuth(Socket socket) : IAuthDisconnectClient
{
    public Guid Id { get; set; }
    
    public ISocketWrapper SocketWrapper { get; set; } = new SocketWrapper(socket);
   
    public ISslStreamWrapper? SslStreamWrapper { get; init; }
    
    public bool Connected => socket.Connected;
    
    public void Disconnect()
    {
        SslStreamWrapper!.InnerSslStream!.Close();
        SocketWrapper!.InnerSocket.Close();
    }
}
