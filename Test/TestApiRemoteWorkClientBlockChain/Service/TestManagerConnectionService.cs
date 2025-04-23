using System.Net;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Service;
using DataFictitious.Connection;
using LibCommunicationStatus;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Entities.Enum;
using LibSocketAndSslStream.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TypeRemoteClient = LibSocketAndSslStream.Entities.Enum.TypeRemoteClient;

namespace TestApiRemoteWorkClientBlockChain.Service;

public class ManagerConnectionServiceTest
{
    private readonly Mock<ILogger<ManagerConnectionService>> _loggerMock = new();
    private readonly Mock<ISocketMiring> _socketMiringMock = new();
    private readonly Mock<IAuthSsl> _authSslMock = new();
    private readonly Mock<IManagerClient> _mockManagerClient = new();
    private readonly IManagerConnection _managerConnection;

    public ManagerConnectionServiceTest()
    {
        _managerConnection = new ManagerConnectionService(
            _loggerMock.Object,
            _socketMiringMock.Object,
            _authSslMock.Object,
            _mockManagerClient.Object
        );
    }
    
    [Fact]
    public async Task InitializeAsync_ShouldReturnSuccess_WhenConnectionEstablished()
    {
        var config = ConnectionConfigTest.GetConnectionTest();
        const TypeAuthMode typeAuthMode = TypeAuthMode.RequireAuthentication;
        var cts = new CancellationTokenSource();

        CommunicationStatus.SetConnecting(true);

        _socketMiringMock
            .Setup(m => m.InitializeAsync(config.Port, config.MaxConnections, 
                TypeRemoteClient.Remote, typeAuthMode, cts.Token))
            .Returns(Task.CompletedTask)
            .Verifiable();
        
        var result = await _managerConnection.InitializeAsync(config, typeAuthMode, cts.Token);
        
        _socketMiringMock.Verify();
        Assert.True(result.Success);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }
    
    [Fact]
    public async Task InitializeAsync_ShouldReturnTimeout_WhenConnectionNotEstablished()
    {
        var config = new ConnectionConfig { Port = 5051, MaxConnections = 5 };
        const TypeAuthMode typeAuthMode = TypeAuthMode.RequireAuthentication;
        var cts = new CancellationTokenSource();

        CommunicationStatus.SetConnecting(false);

        _socketMiringMock
            .Setup(m => m.InitializeAsync(config.Port, config.MaxConnections, 
                TypeRemoteClient.Remote, typeAuthMode, cts.Token))
            .Returns(Task.CompletedTask);

        var result = await _managerConnection.InitializeAsync(config, typeAuthMode, cts.Token);

        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.GatewayTimeout, result.StatusCode);
        Assert.Contains("timeout", result.Message, StringComparison.OrdinalIgnoreCase);
    }
    
    [Fact]
    public async Task InitializeAsync_ShouldThrowException_WhenSocketInitializationFails()
    {
        var config = ConnectionConfigTest.GetConnectionTest();
        const TypeAuthMode typeAuthMode = TypeAuthMode.RequireAuthentication;
        var cts = new CancellationTokenSource();

        _socketMiringMock
            .Setup(m => m.InitializeAsync(config.Port, config.MaxConnections,
                TypeRemoteClient.Remote, typeAuthMode, cts.Token))
            .ThrowsAsync(new Exception("Port number must be a 4-digit number between 1000 and 9999."));
        
        var result = await _managerConnection.InitializeAsync(config, typeAuthMode, cts.Token);
        
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Contains("Service initialization failed. Verify the settings and try again.",
            result.Message, StringComparison.OrdinalIgnoreCase);
    }
}