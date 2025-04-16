using System.Text.Json.Serialization;

namespace LibRemoteAndClient.Entities.Client.Xmrig;

public class XmrigOpenCl
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("cache")]
    public bool Cache { get; set; }

    [JsonPropertyName("loader")]
    public string? Loader { get; set; }

    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    [JsonPropertyName("adl")]
    public bool Adl { get; set; }
}
