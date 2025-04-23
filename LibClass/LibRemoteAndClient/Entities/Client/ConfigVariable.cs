using System.Text.Json.Serialization;

namespace LibRemoteAndClient.Entities.Client;


public class ConfigVariable
{
    [JsonPropertyName("REMOTE_SSL_HOST")]
    public string RemoteSslHost { get; set; } = string.Empty;

    [JsonPropertyName("REMOTE_SSL_PORT")]
    public int RemoteSslPort { get; set; }
}
