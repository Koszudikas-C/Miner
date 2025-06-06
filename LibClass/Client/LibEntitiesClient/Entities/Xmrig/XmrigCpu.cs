using System.Text.Json.Serialization;

namespace LibEntitiesClient.Entities.Xmrig;

public class XmrigCpu
{
    [JsonPropertyName("enabled")]
    public bool Enabled { get; set; }

    [JsonPropertyName("huge-pages")]
    public bool HugePages { get; set; }

    [JsonPropertyName("huge-pages-jit")]
    public bool HugePagesJit { get; set; }

    [JsonPropertyName("hw-aes")]
    public string? HwAes { get; set; }

    [JsonPropertyName("priority")]
    public string? Priority { get; set; }

    [JsonPropertyName("memory-pool")]
    public bool MemoryPool { get; set; }

    [JsonPropertyName("yield")]
    public bool Yield { get; set; }

    [JsonPropertyName("asm")]
    public bool Asm { get; set; }

    [JsonPropertyName("argon2-impl")]
    public string? Argon2Impl { get; set; }

    [JsonPropertyName("argon2")]
    public long[]? Argon2 { get; set; }

    [JsonPropertyName("cn")]
    public long[][]? Cn { get; set; }

    [JsonPropertyName("cn-heavy")]
    public long[][]? CnHeavy { get; set; }

    [JsonPropertyName("cn-lite")]
    public long[][]? CnLite { get; set; }

    [JsonPropertyName("cn-pico")]
    public long[][]? CnPico { get; set; }

    [JsonPropertyName("cn/upx2")]
    public long[][]? CnUpx2 { get; set; }

    [JsonPropertyName("ghostrider")]
    public long[][]? Ghostrider { get; set; }

    [JsonPropertyName("rx")]
    public long[]? Rx { get; set; }

    [JsonPropertyName("rx/wow")]
    public long[]? RxWow { get; set; }

    [JsonPropertyName("cn-lite/0")]
    public bool CnLite0 { get; set; }

    [JsonPropertyName("cn/0")]
    public bool Cn0 { get; set; }

    [JsonPropertyName("rx/arq")]
    public string? RxArq { get; set; }
}
