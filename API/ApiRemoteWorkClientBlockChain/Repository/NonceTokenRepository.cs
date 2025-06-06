using ApiRemoteWorkClientBlockChain.Data;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using LibRemoteAndClient.Entities.Remote.Client;
using Microsoft.EntityFrameworkCore;

namespace ApiRemoteWorkClientBlockChain.Repository;

public class NonceTokenRepository(RemoteWorkClientDbContext dbContext, ILogger<NonceTokenRepository> logger) : INonceToken
{
    public void Add(GuidTokenAuth entity)
    {
        throw new NotImplementedException();
    }
    
    public void Update(GuidTokenAuth entity)
    {
        throw new NotImplementedException();
    }
    
    public void Delete(GuidTokenAuth entity)
    {
        throw new NotImplementedException();
    }

    public GuidTokenAuth GetById(int id)
    {
        if(id == 0)
            throw new ArgumentNullException("The supplier identifier cannot be zero!",nameof(id));

        try
        {
            return dbContext.GuidTokenAuths.Find(id)!;
        }
        catch (Exception e)
        {
            logger.LogError($"An error occurred when getting the nonce token from the database. Error: {e.Message}");
            throw new Exception();
        }
    }

    public List<GuidTokenAuth> GetAll()
    {
        try
        {
            return dbContext.GuidTokenAuths.AsNoTracking().ToList();
        }
        catch (Exception e)
        {
            logger.LogError($"An error occurred when getting all the nonce token from the database. Error: {e.Message}");
            throw new Exception();
        }
    }

    public Task AddAsync(GuidTokenAuth entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task UpdateAsync(GuidTokenAuth entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task DeleteAsync(GuidTokenAuth entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<GuidTokenAuth> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<List<GuidTokenAuth>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}