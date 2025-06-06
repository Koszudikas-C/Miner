
using System.Text.Json.Serialization;

namespace LibEntitiesRemote.Entities.Client.Xmrig;

public class XmrigApi
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("worker-id")]
    public string WorkerId { get; set; } = string.Empty;
}
