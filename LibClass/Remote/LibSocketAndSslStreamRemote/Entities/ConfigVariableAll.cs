using System.Text.Json.Serialization;

namespace LibSocketAndSslStreamRemote.Entities;

public class ConfigVariableAll
{
    [JsonPropertyName("REMOTE_SSL_HOST")]
    public string? RemoteSslHost { get; init; }

    [JsonPropertyName("REMOTE_SSL_PORT")]
    public int RemoteSslPort { get; init; }
}
