using DataFictitiousClient.LibClass.LibSocket;
using DataFictitiousClient.LibClass.LibSocketAndSslStreamClient;
using DataFictitiousClient.LibClass.LibSocketClient;
using LibSocketAndSslStreamClient.Entities.Enum;
using LibSocketAndSslStreamClient.Interface;
using LibSocketClient.Service;
using LibSocks5Client.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TestLibSocket.Service;

public class TestSocketService
{
    private readonly Mock<IConfigVariable> _configVariableMock = new();
    private readonly Mock<ISocks5Options> _socks5OptionsMock = new();
    private readonly Mock<ISocks5> _socks5Mock = new();
    private readonly Mock<ILogger<ListenerService>> _loggerMock = new();
    private readonly Mock<IListenerWrapper> _listenerWrapperMock = new();
    
    private readonly IListener _listener;
    
    public TestSocketService()
    {
        _listener = new ListenerService(
            _configVariableMock.Object,
            _socks5OptionsMock.Object,
            _socks5Mock.Object,
            _loggerMock.Object,
            _listenerWrapperMock.Object
        );
    }

    [Fact]
    public async Task remote_success_connection()
    {
        var cts = CancellationToken.None;
        SocketTest.GetSocketConnected();
        var configVariableFake = ConfigCryptographTest.GetConfigCryptograph();
        var listenerFake = ListenerTest.GetListener();
        
        _configVariableMock.Setup(x => x.GetConfigVariable()).Returns(configVariableFake);
        _listenerWrapperMock.Setup(x => x.Listener).Returns(listenerFake);

        await _listener.StartAsync(TypeAuthMode.RequireAuthentication, 3000, cts);
    }
}