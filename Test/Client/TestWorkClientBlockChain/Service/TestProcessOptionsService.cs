namespace TestWorkClientBlockChain.Service;

public class TestProcessOptionsService
{
    // private readonly ProcessOptions _processOptions;
    // private readonly Mock<ILogger<ProcessOptions>> _mockLogger = new();
    // private readonly Mock<IClientContext> _mockClientContext = new();
    // private readonly Mock<IEventBus> _mockEvents = new();
    // private readonly Mock<ISocketMiring> _mockSocketMiring = new();
    //
    // public TestProcessOptionsService()
    // {
    //     _processOptions = new ProcessOptions(
    //         _mockLogger.Object,
    //         _mockClientContext.Object,
    //         _mockSocketMiring.Object
    //     );
    // }
    //
    // [Fact]
    // public async Task IsProcessAuthSocks5_Should_SetAuthCalledTrue()
    // {
    //     var cts = new CancellationTokenSource();
    //     Action<TypeManagerResponseOperations>? capturedHandler = null!;
    //         
    //     var mockSocket = new Mock<ISocketWrapper>();
    //     mockSocket.Setup(s => s.Connected).Returns(true);
    //     mockSocket.Setup(s => s.RemoteEndPoint).Returns("127.0.0.1");
    //     mockSocket.Setup(s => s.LocalEndPoint).Returns("127.0.0.1");
    //     var mockSslStreamWrapper = new Mock<ISslStreamWrapper>();
    //     mockSslStreamWrapper.Setup(s => s.IsAuthenticated).Returns(true);
    //
    //     var clientInfo = new ClientInfo
    //     {
    //         Id = Guid.NewGuid(),
    //         SocketWrapper = mockSocket.Object,
    //         SslStreamWrapper = mockSslStreamWrapper.Object
    //     };
    //
    //     _mockEvents.Setup(x =>
    //             x.Subscribe(It.IsAny<Action<TypeManagerResponseOperations>>()))
    //         .Callback<Action<TypeManagerResponseOperations>>(handler
    //             =>capturedHandler = handler );
    //     
    //     _mockEvents.Object.Subscribe<TypeManagerResponseOperations>(msg => { });
    //     
    //     _mockClientContext.Setup(x => x.GetClientInfo()).Returns(clientInfo);
    //
    //     await _processOptions.IsProcessAuthSocks5Async(cts.Token);
    //     
    //     const TypeManagerResponseOperations expected = TypeManagerResponseOperations.Success;
    //     Enum? receive = null!;
    //     capturedHandler = msg => receive = msg;
    //     capturedHandler?.Invoke(expected);
    //
    //     Assert.NotNull(receive);
    //     Assert.Equal(TypeManagerResponseOperations.Success, receive);
    // }
    //
    // [Fact]
    // public async Task IsProcessAuthSocks5_WithCanceledToken_Should_ReturnFalse()
    // {
    //     var cts = new CancellationTokenSource();
    //     await cts.CancelAsync();
    //
    //     var mockSocket = new Mock<ISocketWrapper>();
    //     mockSocket.Setup(s => s.Connected).Returns(true);
    //
    //     var mockSslStreamWrapper = new Mock<ISslStreamWrapper>();
    //     mockSslStreamWrapper.Setup(s => s.IsAuthenticated).Returns(false);
    //
    //     var clientInfo = new ClientInfo
    //     {
    //         Id = Guid.NewGuid(),
    //         SocketWrapper = mockSocket.Object,
    //         SslStreamWrapper = mockSslStreamWrapper.Object
    //     };
    //
    //     _mockClientContext.Setup(x => x.GetClientInfo()).Returns(clientInfo);
    //
    //     await Assert.ThrowsAsync<TaskCanceledException>(async () =>
    //         await _processOptions.IsProcessAuthSocks5Async(cts.Token));
    // }
    //
    // [Fact]
    // public async Task IsProcessAuthSocks5_WithDisconnectedSocket_Should_ReturnFalse()
    // {
    //     var cts = new CancellationTokenSource();
    //     var mockSocket = new Mock<ISocketWrapper>();
    //     mockSocket.Setup(s => s.Connected).Returns(false);
    //
    //     var clientInfo = new ClientInfo
    //     {
    //         Id = Guid.NewGuid(),
    //         SocketWrapper = mockSocket.Object,
    //         SslStreamWrapper = null
    //     };
    //
    //     _mockClientContext.Setup(x => x.GetClientInfo()).Returns(clientInfo);
    //
    //     await _processOptions.IsProcessAuthSocks5Async(cts.Token);
    //     // Assert.Equal(TypeManagerResponseOperations.Unauthorized, result);
    // }
    //
    // [Fact]
    // public async Task IsProcessAuthSocks5_WithoutClientInfo_Should_ReturnFalse()
    // {
    //     var cts = new CancellationTokenSource();
    //     _mockClientContext.Setup(x => x.GetClientInfo()).Returns((ClientInfo)null!);
    //
    //      await _processOptions.IsProcessAuthSocks5Async(cts.Token);
    //
    //     // Assert.Equal(TypeManagerResponseOperations.Unauthorized, result);
    // }
}
