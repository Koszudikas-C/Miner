using System.Net;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Service;
using LibReceive.Interface;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSocket.Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace TestApiRemoteWorkClientBlockChain.Service;

public class TestManagerClientService
{
    private readonly ManagerClientService _managerClientService;
    private readonly Mock<ILogger<ManagerClientService>> _mockLogger = new();
    private readonly Mock<ISocketMiring> _mockSocketMiring = new();
    private readonly Mock<IReceive> _mockReceive = new();

    public TestManagerClientService()
    {
        ClientConnected.Instance.Clear();

        _managerClientService = new ManagerClientService(
            _mockLogger.Object,
            _mockSocketMiring.Object,
            _mockReceive.Object
        );
    }

    [Fact]
    public void GetAllClientInfo_ShouldReturnLimitedResults_WhenPageSizeExceedsMaximum()
    {
        var clientList = Enumerable.Range(0, 100)
            .Select(_ => new ClientInfo { Id = Guid.NewGuid() })
            .ToList();

        foreach (var client in clientList)
            ClientConnected.Instance.AddClientInfos(client);

        var response = _managerClientService.GetAllClientInfo(1, 100);

        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.True(response.Data.Count() <= 50);
    }

    [Fact]
    public void GetAllClientInfo_ShouldReturnNotFound_WhenNoClientsExist()
    {
        ClientConnected.Instance.Clear();

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
            ClientConnected.Instance.AddClientInfos(client);

        var response = _managerClientService.GetAllClientInfo(2, 10);

        Assert.True(response.Success);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.Data);
        Assert.Equal(10, response.Data.Count());
        Assert.Equal(clientList.Skip(10).Take(10).Select(c => c.Id), response.Data.Select(d => d.Id));
    }
}
