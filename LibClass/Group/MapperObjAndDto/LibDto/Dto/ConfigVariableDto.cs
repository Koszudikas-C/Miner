using System.Text.Json.Serialization;

namespace LibDto.Dto;

public class ConfigVariableDto
{
    [JsonPropertyName("REMOTE_SOCKS5_API_MINE")]
    public string? RemoteSocks5ApiMine { get; init; }

    [JsonPropertyName("REMOTE_SOCKS5_WORK_SERVICE")]
    public string? RemoteSocks5WorkService { get; init; }

    [JsonPropertyName("REMOTE_SOCKS5_BLOCK_MINE")]
    public string? RemoteSocks5BlockMine { get; init; }

    [JsonPropertyName("REMOTE_SSL_BLOCK")]
    public string? RemoteSslBlock { get; set; }
    
    [JsonPropertyName("REMOTE_SSL_BLOCK_PORT")]
    public int RemoteSslBlockPort { get; init; }

    [JsonPropertyName("REMOTE_SOCKS5_PROXY_MINE")]
    public string? RemoteSocks5ProxyMine { get; init; }

    [JsonPropertyName("CERTIFICATE_PATH")]
    public string? CertificatePath { get; init; }

    [JsonPropertyName("CERTIFICATE_PASSWORD")]
    public string? CertificatePassword { get; init; }

    [JsonPropertyName("PROXY_HOST")]
    public string? ProxyHost { get; init; }

    [JsonPropertyName("PROXY_PORT")]
    public int ProxyPort { get; init; }

    [JsonPropertyName("REMOTE_SOCKS5_DEFAULT_PORT")]
    public int RemoteSocks5DefaultPort { get; init; }

    [JsonPropertyName("REMOTE_USERNAME_TOR")]
    public string? RemoteUsernameTor { get; init; }

    [JsonPropertyName("REMOTE_PASSWORD_TOR")]
    public string? RemotePasswordTor { get; init; }
}