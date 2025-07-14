using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using LibCommunicationStateRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Repository;

public class ClientNotAuthorizedRepository : IClientNotAuthorized
{
    public ApiResponse<bool> Add(ClientNotAuthorized entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Update(ClientNotAuthorized entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Delete(ClientNotAuthorized entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<ClientNotAuthorized> GetById(Guid id)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<List<ClientNotAuthorized>> GetAll(int page, int pageSize,bool all)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> AddAsync(ClientNotAuthorized entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> UpdateAsync(ClientNotAuthorized entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> DeleteAsync(ClientNotAuthorized entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<ClientNotAuthorized>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<IEnumerable<ClientNotAuthorized>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}