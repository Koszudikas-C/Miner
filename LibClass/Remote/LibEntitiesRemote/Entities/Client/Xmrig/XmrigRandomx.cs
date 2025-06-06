using System.Text.Json.Serialization;

namespace LibEntitiesRemote.Entities.Client.Xmrig;

public class XmrigRandomx
{
    [JsonPropertyName("init")]
    public int Init { get; set; }

    [JsonPropertyName("init-avx2")]
    public int InitAvx2 { get; set; }

    [JsonPropertyName("mode")]
    public string? Mode { get; set; }

    [JsonPropertyName("1gb-pages")]
    public bool GbPages { get; set; }

    [JsonPropertyName("rdmsr")]
    public bool Rdmsr { get; set; }

    [JsonPropertyName("wrmsr")]
    public bool Wrmsr { get; set; }

    [JsonPropertyName("cache_qos")]
    public bool CacheQos { get; set; }

    [JsonPropertyName("numa")]
    public bool Numa { get; set; }

    [JsonPropertyName("scratchpad_prefetch_mode")]
    public int ScratchpadPrefetchMode { get; set; }

}
