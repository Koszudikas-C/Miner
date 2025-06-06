using System.Text.Json.Serialization;

namespace LibEntitiesRemote.Entities.Client.Xmrig;

public class XmrigCuda
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("loader")]
    public string? Loader { get; set; }

    [JsonPropertyName("nvml")]
    public bool Nvml { get; set; }
}
