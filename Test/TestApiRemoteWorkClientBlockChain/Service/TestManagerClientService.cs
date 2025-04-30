using System.Net;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Service;
using LibCryptography.Entities;
using LibCryptography.Interface;
using LibDto.Dto;
using LibManagerFile.Entities;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSend.Interface;
using LibSocketAndSslStream.Entities;
using LibSocketAndSslStream.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TestApiRemoteWorkClientBlockChain.Service;

public class TestManagerClientService
{
    private readonly ManagerClientService _managerClientService;
    private readonly Mock<ILogger<ManagerClientService>> _mockLogger = new();
    private readonly Mock<IReceive> _mockReceive = new();
    private readonly Mock<ISend<ConfigCryptographDto>> _mockSend = new();
    private readonly Mock<ISend<ConfigSaveFileDto>> _mockSendConfigSaveFile = new(); 
    private readonly Mock<IClientConnected> _mockClientConnected = new();
    private readonly Mock<ICryptographFile> _mockCryptographFile = new();
    private readonly Mock<ISearchFile> _mockSearchFile = new();
    private readonly Mock<IMapperObj> _mockMapperObj = new();

    public TestManagerClientService()
    {
        ClientConnected.Instance.Clear();

        _managerClientService = new ManagerClientService(
            _mockLogger.Object,
            _mockReceive.Object,
            _mockClientConnected.Object,
            _mockSend.Object,
            _mockSendConfigSaveFile.Object,
            _mockCryptographFile.Object,
            _mockSearchFile.Object,
            _mockMapperObj.Object
        );
    }

    [Fact]
    public void GetAllClientInfo_ShouldReturnLimitedResults_WhenPageSizeExceedsMaximum()
    {
        var clientList = Enumerable.Range(0, 100)
            .Select(_ => new ClientInfo { Id = Guid.NewGuid() })
            .ToList();

        foreach (var client in clientList)
            _mockClientConnected.Setup(x => x.AddClientInfo(client)).Returns(clientList);
        
        _mockClientConnected.Setup(x => x.GetClientInfos()).Returns(clientList);
        
        var response = _managerClientService.GetAllClientInfo(1, 100);

        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.True(response.Data.Count() <= 50);
    }

    [Fact]
    public void GetAllClientInfo_ShouldReturnNotFound_WhenNoClientsExist()
    {
        _mockClientConnected.Setup(x => x.GetClientInfos()).Returns([]);
        var response = _managerClientService.GetAllClientInfo(1, 10);

        Assert.False(response.Success);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Null(response.Data);
    }

    [Fact]
    public void GetAllClientInfo_ShouldReturnExpectedPage_WhenClientsExist()
    {
        ClientConnected.Instance.Clear();
        var clientList = Enumerable.Range(0, 30)
            .Select(_ => new ClientInfo { Id = Guid.NewGuid() })
            .ToList();

        foreach (var client in clientList)
            _mockClientConnected.Setup(x => x.AddClientInfo(client)).Returns(clientList);
        
        _mockClientConnected.Setup(x => x.GetClientInfos()).Returns(clientList);

        var response = _managerClientService.GetAllClientInfo(2, 10);

        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(10, response.Data.Count());
        Assert.Equal(clientList.Skip(10).Take(10).Select(c => c.Id), response.Data.Select(d => d.Id));
    }
}