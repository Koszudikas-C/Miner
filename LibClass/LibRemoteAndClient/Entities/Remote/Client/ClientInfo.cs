using System.Text.Json.Serialization;
using LibSocketAndSslStream.Interface;

namespace LibRemoteAndClient.Entities.Remote.Client;

public class ClientInfo
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public ISocketWrapper? SocketWrapper { get; init; }
    [JsonIgnore]
    public ISslStreamWrapper? SslStreamWrapper { get; init; }

    public ClientMine? ClientMine { get; set; }


    public void Disconnect()
    {
        if (!SslStreamWrapper!.IsAuthenticated)
            throw new ArgumentNullException($"It's trying to disconnect a client without being authenticated.");
        
        SslStreamWrapper!.InnerSslStream!.Close();
        SocketWrapper!.InnerSocket.Close();
    }
}
