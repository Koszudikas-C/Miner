using LibSocks5.Entities.Enum;

namespace LibSocks5.Entities;

public sealed class Socks5Options
{
    public string ProxyHost { get; } 
    public int ProxyPort { get; } 
    public string DestinationHost { get; }
    public int DestinationPort { get; }
    public AuthType? Auth { get; set; }
    public Credential? Credentials { get; } = new();

    public Socks5Options(string proxyHost, int proxyPort, string destHost, int destPort)
    {
        ProxyHost = proxyHost;
        ProxyPort = proxyPort;
        DestinationHost = destHost;
        DestinationPort = destPort;
        Auth = AuthType.None;
    }

    public Socks5Options(string proxyHost, string destHost, int destPort) : this(proxyHost, 1080, destHost, destPort) { }

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