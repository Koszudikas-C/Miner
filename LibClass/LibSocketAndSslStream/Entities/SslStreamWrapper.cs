using System.Net.Security;
using LibSocketAndSslStream.Interface;

namespace LibSocketAndSslStream.Entities;

public class SslStreamWrapper(SslStream sslStream) : ISslStreamWrapper
{
    public bool IsAuthenticated => sslStream.IsAuthenticated;

    public string Remote => sslStream.RemoteCertificate?.Subject ?? string.Empty;

    public SslStream InnerSslStream => sslStream;
}