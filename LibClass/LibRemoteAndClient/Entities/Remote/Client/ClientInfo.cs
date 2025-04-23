using System.Net.Security;
using System.Text.Json.Serialization;
using LibSocketAndSslStream.Interface;

namespace LibRemoteAndClient.Entities.Remote.Client;

public class ClientInfo
{
    public Guid Id { get; init; }
    [JsonIgnore]
    public ISocketWrapper? SocketWrapper { get; init; }
    [JsonIgnore]
    public ISslStreamWrapper? SslStreamWrapper { get; init; }
    public ClientMine? ClientMine { get; init; }
}
