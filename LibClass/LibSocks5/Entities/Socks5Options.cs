using LibSocks5.Entities.Enum;

namespace LibSocks5.Entities;

public sealed class Socks5Options(string proxyHost, int proxyPort, string destHost, int destPort)
{
    public string ProxyHost { get; } = proxyHost;
    public int ProxyPort { get; } = proxyPort;
    public string DestinationHost { get; } = destHost;
    public int DestinationPort { get; } = destPort;
    public AuthType? Auth { get; set; } = AuthType.None;
    public Credential? Credentials { get; } = new();
    
    public Socks5Options(string proxyHost, int proxyPort, string destHost, int destPort, string username,
        string password) : this(proxyHost, proxyPort, destHost, destPort)
    {
        ProxyHost = proxyHost;
        ProxyPort = proxyPort;
        DestinationHost = destHost;
        DestinationPort = destPort;
        Auth = AuthType.UsernamePassword;
        Credentials!.UserName = username;
        Credentials.Password = password;
    }

    public Socks5Options(string proxyHost, string destHost, int destPort, string username, string password) :
        this(proxyHost, 1080, destHost, destPort, username, password)
    { }
}