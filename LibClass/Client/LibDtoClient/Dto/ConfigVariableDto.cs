using System.Text.Json.Serialization;

namespace LibDtoClient.Dto;

public class ConfigVariableDto
{
    [JsonPropertyName("REMOTE_SOCKS5_API_MINE")]
    public string? RemoteSocks5ApiMine { get; set; }

    [JsonPropertyName("REMOTE_SOCKS5_WORK_SERVICE")]
    public string? RemoteSocks5WorkService { get; set; }

    [JsonPropertyName("REMOTE_SOCKS5_BLOCK_MINE")]
    public string? RemoteSocks5BlockMine { get; set; }

    [JsonPropertyName("REMOTE_SSL_BLOCK")]
    public string? RemoteSslBlock { get; set; }
    
    [JsonPropertyName("REMOTE_SSL_BLOCK_PORT")]
    public int RemoteSslBlockPort { get; set; }

    [JsonPropertyName("REMOTE_SOCKS5_PROXY_MINE")]
    public string? RemoteSocks5ProxyMine { get; set; }
    
    [JsonPropertyName("CERTIFICATE_PATH")]
    public string? CertificatePath { get; set; }

    [JsonPropertyName("CERTIFICATE_PASSWORD")]
    public string? CertificatePassword { get; set; }

    [JsonPropertyName("PROXY_HOST")]
    public string? ProxyHost { get; set; }

    [JsonPropertyName("PROXY_PORT")]
    public int ProxyPort { get; set; }

    [JsonPropertyName("REMOTE_SOCKS5_DEFAULT_PORT")]
    public int RemoteSocks5DefaultPort { get; set; }

    [JsonPropertyName("REMOTE_USERNAME_TOR")]
    public string? RemoteUsernameTor { get; set; }

    [JsonPropertyName("REMOTE_PASSWORD_TOR")]
    public string? RemotePasswordTor { get; set; }
}
