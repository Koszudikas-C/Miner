using System.Text.Json.Serialization;

namespace LibEntitiesClient.Entities.Xmrig;

public partial class XmrigConfig
{
    [JsonPropertyName("api")]
    public XmrigApi? Api { get; set; }

    [JsonPropertyName("http")]
    public XmrigHttp? Http { get; set; }

    [JsonPropertyName("autosave")]
    public bool Autosave { get; set; }

    [JsonPropertyName("background")]
    public bool Background { get; set; }

    [JsonPropertyName("colors")]
    public bool Colors { get; set; }

    [JsonPropertyName("title")]
    public bool Title { get; set; }

    [JsonPropertyName("randomx")]
    public XmrigRandomx? Randomx { get; set; }

    [JsonPropertyName("cpu")]
    public XmrigCpu? Cpu { get; set; }

    [JsonPropertyName("opencl")]
    public XmrigOpenCl? Opencl { get; set; }

    [JsonPropertyName("cuda")]
    public XmrigCuda? Cuda { get; set; }

    [JsonPropertyName("log-file")]
    public string? LogFile { get; set; }

    [JsonPropertyName("donate-level")]
    public long DonateLevel { get; set; }

    [JsonPropertyName("donate-over-proxy")]
    public long DonateOverProxy { get; set; }

    [JsonPropertyName("pools")]
    public XmrigPool[]? Pools { get; set; }

    [JsonPropertyName("retries")]
    public long Retries { get; set; }

    [JsonPropertyName("retry-pause")]
    public long RetryPause { get; set; }

    [JsonPropertyName("print-time")]
    public long PrintTime { get; set; }

    [JsonPropertyName("health-print-time")]
    public long HealthPrintTime { get; set; }

    [JsonPropertyName("dmi")]
    public bool Dmi { get; set; }

    [JsonPropertyName("syslog")]
    public bool Syslog { get; set; }

    [JsonPropertyName("tls")]
    public XmrigTls? Tls { get; set; }

    [JsonPropertyName("dns")]
    public XmrigDns? Dns { get; set; }

    [JsonPropertyName("user-agent")]
    public string? UserAgent { get; set; }

    [JsonPropertyName("verbose")]
    public long Verbose { get; set; }

    [JsonPropertyName("watch")]
    public bool Watch { get; set; }

    [JsonPropertyName("pause-on-battery")]
    public bool PauseOnBattery { get; set; }

    [JsonPropertyName("pause-on-active")]
    public bool PauseOnActive { get; set; }
}
