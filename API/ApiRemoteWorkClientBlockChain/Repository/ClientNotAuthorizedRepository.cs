using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Interface.Repository;

namespace ApiRemoteWorkClientBlockChain.Repository;

public class ClientNotAuthorizedRepository : IClientNotAuthorized
{
    public void Add(ClientNotAuthorized entity)
    {
        throw new NotImplementedException();
    }

    public void Update(ClientNotAuthorized entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(ClientNotAuthorized entity)
    {
        throw new NotImplementedException();
    }

    public ClientNotAuthorized GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<ClientNotAuthorized> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(ClientNotAuthorized entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(ClientNotAuthorized entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(ClientNotAuthorized entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ClientNotAuthorized> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<ClientNotAuthorized>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}