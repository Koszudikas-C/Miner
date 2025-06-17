using LibSocketAndSslStreamClient.Entities;

namespace DataFictitiousClient.Entities;

public static class ConfigVariableTest
{
    public static ConfigVariable GetConfigVariable()
    {
        var fakeConfig = new ConfigVariable
        {
            RemoteSocks5ApiMine = "http://fake-api.mine.com",
            RemoteSocks5WorkService = "http://fake-workservice.com",
            RemoteSocks5BlockMine = "http://fake-blockmine.com",
            RemoteSslBlock = "127.0.0.0",
            RemoteSslBlockPort = 3000,
            RemoteSocks5ProxyMine = "http://proxy.fake-mine.com",
            CertificatePath = "/etc/certs/fake-cert.pem",
            CertificatePassword = "FakePass123!",
            ProxyHost = "proxy.fake-server.com",
            ProxyPort = 9050,
            RemoteSocks5DefaultPort = 1080,
            RemoteUsernameTor = "fakeUserTor",
            RemotePasswordTor = "FakePassTor!"
        };

        return fakeConfig;
    }
}