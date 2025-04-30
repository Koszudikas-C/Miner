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

        _iConnectionAndAuthMock.Setup(m => m.ConnectAndAuthAsync(cts.Token)).Verifiable();

        _iConnectionAndAuthMock.Object.ConnectAndAuthAsync(cts.Token);

        _iConnectionAndAuthMock.Verify(m => m.ConnectAndAuthAsync(cts.Token), Times.Once());
    }
}