using System.Net.Sockets;
using LibEntitiesRemote.Entities.Client;
using LibEntitiesRemote.Interface;

namespace LibEntitiesRemote.Entities;

public class ClientInfo
{
    public Guid Id { get; set; }
    
    public ISocketWrapper? SocketWrapper { get; set; }
   
    public ISslStreamWrapper? SslStreamWrapper { get; init; }
  
    public ClientMine? ClientMine { get; set; } 
    
    public void Disconnect()
    {
        SslStreamWrapper!.InnerSslStream!.Close();
        SocketWrapper!.InnerSocket.Close();
    }
}
