using System.Text.Json.Serialization;

namespace LibRemoteAndClient.Entities.Remote.Client.Xmrig;

public class XmrigTls
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("protocols")]
    public string? Protocols { get; set; }

    [JsonPropertyName("cert")]
    public string? Cert { get; set; }

    [JsonPropertyName("cert_key")]
    public string? CertKey { get; set; }

    [JsonPropertyName("ciphers")]
    public string? Ciphers { get; set; }

    [JsonPropertyName("ciphersuites")]
    public string? Ciphersuites { get; set; }

    [JsonPropertyName("dhparam")]
    public string? Dhparam { get; set; }
}
