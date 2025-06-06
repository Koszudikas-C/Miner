using System.Text.Json.Serialization;

namespace LibEntitiesClient.Entities.Xmrig;

public class XmrigDns
{
    [JsonPropertyName("ipv6")]
    public bool Ipv6 { get; set; }

    [JsonPropertyName("ttl")]
    public long Ttl { get; set; }

}
