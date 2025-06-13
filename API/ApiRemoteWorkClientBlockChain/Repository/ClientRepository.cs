using System.Net;
using ApiRemoteWorkClientBlockChain.Data;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using LibHandlerRemote.Entities;
using LibSocketAndSslStreamRemote.Entities;
using Microsoft.AspNetCore.Authentication;

namespace ApiRemoteWorkClientBlockChain.Repository;

public class ClientRepository : IClient
{
    private readonly ILogger<ClientRepository> _logger;
    private readonly RemoteWorkClientDbContext _dbContext;
    private readonly List<Client> _clients = [];
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;


    public ClientRepository(ILogger<ClientRepository> logger,
        RemoteWorkClientDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
        SubscribeEvents();
    }

    private void SubscribeEvents()
    {
        _globalEventBus.SubscribeFunc<SocketsConnectedEvent>(async (socketEvents, cts) =>
        {
            try
            {
                await OnReceiveSocketConnectedEvents(socketEvents, cts);
            }
            catch (Exception e)
            {
                _logger.LogError("An error occurred when trying to " +
                                 "save the generic clients connected " +
                                 "to Remote. Error: {Message}", e);
                throw new Exception();
            }
        });
    }

    private async Task OnReceiveSocketConnectedEvents(SocketsConnectedEvent socketsConnectedEvent,
        CancellationToken cts = default)
    {
        var ip = ((IPEndPoint)socketsConnectedEvent.SocketClient.RemoteEndPoint!).Address.ToString();
        var port = ((IPEndPoint)socketsConnectedEvent.SocketClient.RemoteEndPoint!).Port.ToString();
        CreateInstanceClient(ip, port, socketsConnectedEvent);

        await AddAsync(cts);
    }

    private void CreateInstanceClient(string ip, string port,
        SocketsConnectedEvent socketsConnectedEvent)
    {
        var client = new Client(ip, port)
        {
            TimeoutReceive = socketsConnectedEvent.SocketClient.ReceiveTimeout.ToString(),
            TimeoutSend = socketsConnectedEvent.SocketClient.SendTimeout.ToString()
        };

        _clients.Add(client);
    }

    public void Add(Client entity)
    {
        throw new NotImplementedException();
    }

    public void Update(Client entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(Client entity)
    {
        throw new NotImplementedException();
    }

    public Client GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<Client> GetAll()
    {
        throw new NotImplementedException();
    }

    private async Task AddAsync(CancellationToken cts = default)
    {
        if (_clients.Count < 2)
        {
            Console.WriteLine("Number client accepted for persistence has to be greater than 2");
            return;
        }

        try
        {
            await _dbContext.Client.AddRangeAsync(_clients, cts);

            await _dbContext.SaveChangesAsync(cts);
            
            _clients.Clear();
        }
        catch (Exception e)
        {
            _logger.LogError("An error occurred when " +
                             "trying to save the client. Error: {Message}", e);
            throw new Exception();
        }
    }

    public Task AddAsync(Client entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Client entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Client entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Client> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Client>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}