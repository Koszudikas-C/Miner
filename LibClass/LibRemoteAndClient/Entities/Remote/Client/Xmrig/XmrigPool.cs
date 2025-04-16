using System.Text.Json.Serialization;

namespace LibRemoteAndClient.Entities.Remote.Client.Xmrig;

public class XmrigPool
{
    [JsonPropertyName("algo")]
    public string? Algo { get; set; }

    [JsonPropertyName("coin")]
    public string? Coin { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("user")]
    public string? User { get; set; }

    [JsonPropertyName("pass")]
    public string? Pass { get; set; }

    [JsonPropertyName("rig-id")]
    public string? RigId { get; set; }

    [JsonPropertyName("nicehash")]
    public bool Nicehash { get; set; }

    [JsonPropertyName("keepalive")]
    public bool Keepalive { get; set; }

    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("tls")]
    public bool Tls { get; set; }

    [JsonPropertyName("sni")]
    public bool Sni { get; set; }

    [JsonPropertyName("tls-fingerprint")]
    public string? TlsFingerprint { get; set; }

    [JsonPropertyName("daemon")]
    public bool Daemon { get; set; }

    [JsonPropertyName("socks5")]
    public string? Socks5 { get; set; }

    [JsonPropertyName("self-select")]
    public string? SelfSelect { get; set; }

    [JsonPropertyName("submit-to-origin")]
    public bool SubmitToOrigin { get; set; }
}
