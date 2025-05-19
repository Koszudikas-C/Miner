using System.Net;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Service;
using DataFictitious.Connection;
using LibCommunicationStatus;
using LibCryptography.Interface;
using LibDto.Dto;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibReceive.Interface;
using LibSend.Interface;
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
    private readonly Mock<ISearchFile> _mockSearchFile = new();
    private readonly Mock<IManagerClient> _mockManagerClient = new();
    private readonly Mock<ISend<ConfigCryptographDto>> _mockSendConfigCryptography = new();
    private readonly Mock<ISend<ConfigSaveFileDto>> _mockSendConfigSaveFile = new();
    private readonly Mock<IClientConnected> _mockClientConnected = new ();
    private readonly Mock<ICryptographFile> _mockCryptograph = new();
    private readonly Mock<IMapperObj> _mapperObj = new();
    private readonly Mock<IReceive> _mockReceive = new();
    private readonly Mock<IPosAuth> _mockPosAuth = new();
    
    private readonly IManagerConnection _managerConnection;

    public ManagerConnectionServiceTest()
    {
        _managerConnection = new ManagerConnectionService(
            _loggerMock.Object,
            _socketMiringMock.Object,
            _authSslMock.Object,
            _mockManagerClient.Object,
            _mockSearchFile.Object,
            _mockSendConfigCryptography.Object,
            _mockSendConfigSaveFile.Object,
            _mockClientConnected.Object,
            _mockCryptograph.Object,
            _mapperObj.Object,
            _mockReceive.Object,
            _mockPosAuth.Object
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
                typeAuthMode, cts.Token))
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
                typeAuthMode, cts.Token))
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
                typeAuthMode, cts.Token))
            
            .ThrowsAsync(new Exception("Port number must be a 4-digit number between 1000 and 9999."));
        
        var result = await _managerConnection.InitializeAsync(config, typeAuthMode, cts.Token);
        
        Assert.False(result.Success);
        Assert.Equal(HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.Contains("Service initialization failed. Verify the settings and try again.",
            result.Message, StringComparison.OrdinalIgnoreCase);
    }
}