using ApiRemoteWorkClientBlockChain.Interface.Repository;
using LibCommunicationStateRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Repository;

public class BaseRepository<TW, T> : IRepositoryBase<TW, T>
{
    public ApiResponse<bool> Add(T entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Update(T entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<bool> Delete(T entity)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<T> GetById(TW id)
    {
        throw new NotImplementedException();
    }

    public ApiResponse<List<T>> GetAll(int page, int pageSize, bool all)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<bool>> DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<T>> GetByIdAsync(TW id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<IEnumerable<T>>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}