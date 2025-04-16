using System.Net.Security;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace LibRemoteAndClient.Entities.Remote.Client;

public class ClientInfo
{
    public Guid Id { get; set; }
    [JsonIgnore]
    public Socket? Socket { get; set; }
    [JsonIgnore]
    public SslStream? SslStream { get; set; }
    public ClientMine? ClientMine { get; set; }
}
