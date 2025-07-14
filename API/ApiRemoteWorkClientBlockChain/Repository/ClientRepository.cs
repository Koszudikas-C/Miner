using System.Collections.Concurrent;
using System.Net;
using ApiRemoteWorkClientBlockChain.Data;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using LibCommunicationStateRemote.Entities;
using LibEntitiesRemote.Entities.Client;
using LibHandlerRemote.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiRemoteWorkClientBlockChain.Repository;

public class ClientRepository(
    ILogger<ClientRepository> logger,
    RemoteWorkClientDbContext dbContext)
    : IClient
{
    private readonly List<Client> _clients = [];
    private readonly ConcurrentDictionary<string, Client> _listClientBd = [];
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

    public ApiResponse<bool> Add(Client entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Update(Client entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Delete(Client entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<Client> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<Client> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<List<Client>> GetAll(int page, int pageSize, bool all)
    {
        try
        {
            if (all)
                return new ApiResponse<List<Client>>(HttpStatusCode.OK,
                    true, "Data successfully sought.", [dbContext.Client.ToList()]);

            return new ApiResponse<List<Client>>(HttpStatusCode.OK, true,
                "Data successfully sought.", [
                    dbContext.Client.Skip(page - 1).Take(pageSize).ToList()
                ]);
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred when " +
                            "trying to save the client. Error: {Message}", e);
            throw new Exception();
        }
    }

    public async Task<ApiResponse<bool>> AddListAsync(List<Client> clients, CancellationToken cts = default)
    {
        if (clients.Count == 0)
        {
            logger.LogInformation("Number client accepted for persistence has to be greater than 2");
            return new ApiResponse<bool>(HttpStatusCode.UnprocessableEntity,
                false, "The list passed as a parameter is empty check it");
        }
        
        try
        {
            var ips = clients.Select(c => c.Ip).Distinct().ToList();

            var existingIps = await dbContext.Client
                .Where(c => ips.Contains(c.Ip))
                .Select(c => c.Ip)
                .ToListAsync(cts);

            var newClients = clients
                .Where(c => !existingIps.Contains(c.Ip))
                .GroupBy(c => c.Ip)
                .Select(g =>
                {
                    var clientBase = g.First();
                    clientBase.AttemptsConnection = g.Count();
                    return clientBase;
                })
                .ToList();

            if (newClients.Count == 0)
            {
                logger.LogInformation("All provided IPs are already registered.");
                return new ApiResponse<bool>(HttpStatusCode.Conflict, false,
                    "No new clients to persist — all IPs already exist.");
            }

            await dbContext.Client.AddRangeAsync(newClients, cts);

            await dbContext.SaveChangesAsync(cts);
            
            _globalEventBus.Publish(clients);
            
            logger.LogInformation("List of " +
                                  $"Client added successfully. Amount: {clients.Count}");

            return new ApiResponse<bool>(HttpStatusCode.OK, true,
                "Clients successfully added!");
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred when " +
                            "trying to save the client. Error: {Message}", e);
            throw new Exception();
        }
    }

    public async Task<ApiResponse<bool>> UpdateListAsync(List<Client> clients, CancellationToken cts = default)
    {
        if (clients.Count == 0)
        {
            logger.LogWarning("Client list is empty. No updates performed.");
            return new ApiResponse<bool>(HttpStatusCode.UnprocessableEntity, false, "The list passed as a parameter is empty check it.");
        }

        try
        {
            var inputClientIds = clients.Select(c => c.Id).ToHashSet();

            var existingClients = await dbContext.Client
                .Where(c => inputClientIds.Contains(c.Id))
                .ToListAsync(cts);

            if (existingClients.Count != inputClientIds.Count)
            {
                var existingIds = existingClients.Select(c => c.Id).ToHashSet();
                var missingIds = inputClientIds.Except(existingIds);
                logger.LogWarning("Some clients were not found: {MissingIds}", string.Join(", ", missingIds));
                return new ApiResponse<bool>(HttpStatusCode.NotFound, false, "One or more clients not found.");
            }

            var inputClientsDict = clients.ToDictionary(c => c.Id);

            foreach (var existing in existingClients)
            {
                var updated = inputClientsDict[existing.Id];
                TransferNewClientProperties(existing, updated);
            }

            await dbContext.SaveChangesAsync(cts);

            _globalEventBus.Publish(clients);

            logger.LogInformation("Successfully updated {Count} clients.", clients.Count);

            return new ApiResponse<bool>(HttpStatusCode.OK, true, "Client list updated successfully.");
        }
        catch (ArgumentException ex)
        {
            logger.LogInformation(ex, "An error occurred when updating customer lists, check duplication in identifiers");
            
            return new ApiResponse<bool>(HttpStatusCode.Conflict, false,
                "An error occurred when updating customer lists, check duplication in identifiers", 
                null, [ex.Message]);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating client list.");
            return new ApiResponse<bool>(HttpStatusCode.InternalServerError, false, 
                "Internal error while updating clients.", 
                null, [ex.Message]);
        }
    }



    private void TransferNewClientProperties(Client existing, Client updated)
    {
        existing.IpLocal = updated.IpLocal;
        existing.Port = updated.Port;
        existing.AttemptsConnection = updated.AttemptsConnection;
        existing.StateClient = updated.StateClient;
        existing.TimeoutReceive = updated.TimeoutReceive;
        existing.TimeoutSend = updated.TimeoutSend;
        existing.DateConnected = updated.DateConnected;
    }

    public Task<ApiResponse<bool>> AddAsync(Client entity, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> UpdateAsync(Client entity, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> DeleteAsync(Client entity, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> DeleteAsync(int entity, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<Client>> GetByIdAsync(int id, CancellationToken cts = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<IEnumerable<Client>>> GetAllAsync(int page, int pageSize,
        CancellationToken cts = default)
    {
        var clients = await dbContext.Client
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cts);

        if (clients.Count == 0)
        {
            return new ApiResponse<IEnumerable<Client>>(HttpStatusCode.NotFound, false,
                "No data has been located!.");
        }

        return new ApiResponse<IEnumerable<Client>>(HttpStatusCode.OK, true,
            $"Data found successfully. Page: {page} Total: {clients.Count}", [clients]);
    }

    public async Task<ApiResponse<Client>> GetClientByIp(string ip, CancellationToken cts = default)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return new ApiResponse<Client>(HttpStatusCode.BadRequest, false,
                "Check the arguments provided!.");

        try
        {
            var client = await dbContext.Client.FindAsync([ip], cts);

            if (client is null)
                return new ApiResponse<Client>(HttpStatusCode.NotFound, false,
                    $"There is no related data{ip}");

            return new ApiResponse<Client>(HttpStatusCode.OK, true,
                "Data found", [client]);
        }
        catch (Exception e)
        {
            logger.LogWarning("An error occurred when " +
                              "accessing the database " +
                              "with the IP parameter{params}." +
                              " Error: {error}", ip, e);
            throw new Exception();
        }
    }
}