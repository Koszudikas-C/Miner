using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace LibRemoteAndClient.Entities.Remote.Client;

public class ClientInfo
{
    public Guid Id { get; init; }
    [JsonIgnore]
    public Socket? Socket { get; init; }
    [JsonIgnore]
    public SslStream? SslStream { get; init; }
    public ClientMine? ClientMine { get; set; }
}
