using System.Net.Security;
using LibEntitiesRemote.Interface;

namespace LibEntitiesRemote.Entities;

public class SslStreamWrapper(SslStream sslStream) : ISslStreamWrapper
{
    public bool IsAuthenticated => sslStream.IsAuthenticated;

    public string Remote => sslStream.RemoteCertificate?.Subject ?? string.Empty;

    public SslStream InnerSslStream => sslStream;
}
