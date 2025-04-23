using LibSocketAndSslStream.Entities;
using Moq;
using WorkClientBlockChain.Interface;
using Xunit;

namespace TestWorkClientBlockChain.Service;

public class TestConnectionAndAuth
{
    private readonly Mock<IConnectionAndAuth> _iConnectionAndAuthMock = new ();
    
    [Fact]
    public void ConnectionAndAuthTest()
    {
        var cts = new CancellationTokenSource();
        var config = new ConnectionConfig { Port = 5051, MaxConnections = 5 };
        _iConnectionAndAuthMock.Setup(m => m.ConnectAndAuthAsync(config, cts.Token)).Verifiable();

        _iConnectionAndAuthMock.Object.ConnectAndAuthAsync(config, cts.Token);

        _iConnectionAndAuthMock.Verify(m => m.ConnectAndAuthAsync(config, cts.Token), Times.Once());
    }
}