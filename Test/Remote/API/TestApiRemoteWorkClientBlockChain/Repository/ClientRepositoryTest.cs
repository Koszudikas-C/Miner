using System.Net;
using ApiRemoteWorkClientBlockChain.Data;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using ApiRemoteWorkClientBlockChain.Repository;
using DataFictitiousRemote.Entities.Client;
using EntityFrameworkCore.Testing.Moq;
using LibEntitiesRemote.Entities.Client;
using Microsoft.Extensions.Logging;
using Moq;

namespace TestApiRemoteWorkClientBlockChain.Repository;

public class ClientRepositoryTest
{
    private readonly IClient _clientRepository;
    private readonly Mock<ILogger<ClientRepository>> _mockLogger = new();
    private readonly RemoteWorkClientDbContext _mockDbContext = Create.MockedDbContextFor<RemoteWorkClientDbContext>();

    public ClientRepositoryTest()
    {
        _clientRepository = new ClientRepository(
            _mockLogger.Object,
            _mockDbContext
        );
    }

    [Fact]
    public async Task add_list_client_empty()
    {
        const string messageExpect = "The list passed as a parameter is empty check it";
        var ctsSource = new CancellationTokenSource();
        var clients = new List<LibEntitiesRemote.Entities.Client.Client>();

        var apiResponse = await _clientRepository.AddListAsync(clients, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, apiResponse.StatusCode);
        Assert.False(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.Null(apiResponse.Data);
        Assert.Null(apiResponse.Errors);
    }

    [Fact]
    public async Task add_list_client_duplicate()
    {
        const string messageExpect = "No new clients to persist — all IPs already exist.";
        var ctsSource = new CancellationTokenSource();
        var clients = ClientTest.GetClients();

        await _clientRepository.AddListAsync(clients, ctsSource.Token);
        var apiResponse = await _clientRepository.AddListAsync(clients, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.Conflict, apiResponse.StatusCode);
        Assert.False(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.Null(apiResponse.Data);
        Assert.Null(apiResponse.Errors);
    }

    [Fact]
    public async Task add_list_client_success()
    {
        const string messageExpect = "Clients successfully added!";

        var ctsSource = new CancellationTokenSource();
        var clientList = ClientTest.GetClients();

        var apiResponse = await _clientRepository.AddListAsync(clientList, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.Null(apiResponse.Data);
        Assert.Null(apiResponse.Errors);
    }

    [Fact]
    public async Task update_list_client_empty()
    {
        const string messageExpect = "The list passed as a parameter is empty check it.";
        var ctsSource = new CancellationTokenSource();
        var clients = new List<LibEntitiesRemote.Entities.Client.Client>();

        var apiResponse = await _clientRepository.UpdateListAsync(clients, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.UnprocessableEntity, apiResponse.StatusCode);
        Assert.False(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.Null(apiResponse.Data);
        Assert.Null(apiResponse.Errors);
    }

    [Fact]
    public async Task update_list_client_not_found()
    {
        const string messageExpect = "One or more clients not found.";
        var ctsSource = new CancellationTokenSource();
        var clients = ClientTest.GetClients();

        var apiResponse = await _clientRepository.UpdateListAsync(clients, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.NotFound, apiResponse.StatusCode);
        Assert.False(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.Null(apiResponse.Data);
        Assert.Null(apiResponse.Errors);
        Assert.Null(apiResponse.Errors);
    }

    [Fact]
    public async Task update_list_clients_duplicate()
    {
        const string messageExpect = "An error occurred when updating customer lists, check duplication in identifiers";
        var ctsSource = new CancellationTokenSource();
        var clients = ClientTest.GetClients();
        var clients1 = ClientTest.GetClients();

        clients.AddRange(clients1);

        await _clientRepository.AddListAsync(clients1, ctsSource.Token);

        var apiResponse = await _clientRepository.UpdateListAsync(clients, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.Conflict, apiResponse.StatusCode);
        Assert.False(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.Null(apiResponse.Data);
        Assert.NotNull(apiResponse.Errors);
    }

    [Fact]
    public async Task get_all_client_no_data()
    {
        const string messageExpect = "No data has been located!.";
        var ctsSource = new CancellationTokenSource();

        var apiResponse = await _clientRepository.GetAllAsync(-1, 0, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.NotFound, apiResponse.StatusCode);
        Assert.False(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.Null(apiResponse.Data);
        Assert.Null(apiResponse.Errors);
    }

    [Fact]
    public async Task get_all_client_success()
    {
        const string messageExpect = "Data found successfully. Page: 0 Total: 5";
        var ctsSource = new CancellationTokenSource();
        var clients = ClientTest.GetClients();

        await _mockDbContext.Client.AddRangeAsync(clients, ctsSource.Token);
        await _mockDbContext.SaveChangesAsync(ctsSource.Token);
        
        var apiResponse = await _clientRepository.GetAllAsync(0, 6, ctsSource.Token);

        Assert.NotNull(apiResponse);
        Assert.Equal(HttpStatusCode.OK, apiResponse.StatusCode);
        Assert.True(apiResponse.Success);
        Assert.Equal(messageExpect, apiResponse.Message);
        Assert.NotNull(apiResponse.Data);
        Assert.Null(apiResponse.Errors);
    }
}