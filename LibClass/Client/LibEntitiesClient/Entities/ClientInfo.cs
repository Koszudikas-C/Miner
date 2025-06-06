using System.Net.Sockets;
using LibEntitiesClient.Interface;

namespace LibEntitiesClient.Entities;

public class ClientInfo
{
    public Guid Id { get; set; }
    
    public ISocketWrapper? SocketWrapper { get; init; }
   
    public ISslStreamWrapper? SslStreamWrapper { get; init; }
    
    public ClientMine? ClientMine { get; set; }
    
    public void Disconnect()
    {
        SslStreamWrapper!.InnerSslStream!.Close();
        SocketWrapper!.InnerSocket.Close();
    }
}
