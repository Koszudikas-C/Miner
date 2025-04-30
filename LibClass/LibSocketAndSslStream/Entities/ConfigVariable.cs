using System.Text.Json.Serialization;

namespace LibSocketAndSslStream.Entities;

public class ConfigVariable
{
    public string? RemoteSocks5ApiMine { get; init; }
    public string? RemoteSocks5WorkService { get; init; }
    public string? RemoteSocks5BlockMine { get; init; }
    public string? RemoteSslBlock { get; set; }
    public int RemoteSslBlockPort { get; init; }
    public string? RemoteSocks5ProxyMine { get; init; }
    public string? CertificatePath { get; init; }
    public string? CertificatePassword { get; init; }
    public string? ProxyHost { get; init; }
    public int ProxyPort { get; init; }
    public int RemoteSocks5DefaultPort { get; init; }
    public string? RemoteUsernameTor { get; init; }
    public string? RemotePasswordTor { get; init; }
}