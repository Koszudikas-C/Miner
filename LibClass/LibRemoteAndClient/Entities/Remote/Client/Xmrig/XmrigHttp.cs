
using System.Text.Json.Serialization;

namespace LibRemoteAndClient.Entities.Remote.Client.Xmrig;

public class XmrigHttp
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("host")]
    public string Host { get; set; } = string.Empty;

    [JsonPropertyName("port")]
    public long Port { get; set; }

    [JsonPropertyName("access-token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("restricted")]
    public bool Restricted { get; set; }
}
