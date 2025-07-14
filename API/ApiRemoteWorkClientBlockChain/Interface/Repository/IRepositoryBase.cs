using LibCommunicationStateRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Interface.Repository;

public interface IRepositoryBase<in TW, T>
{
    ApiResponse<bool> Add(T entity);
    ApiResponse<bool> Update(T entity);
    ApiResponse<bool> Delete(T entity);
    ApiResponse<T> GetById(TW id);
    ApiResponse<List<T>> GetAll(int page, int pageSize, bool all);

    Task<ApiResponse<bool>> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task<ApiResponse<bool>> DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task<ApiResponse<T>> GetByIdAsync(TW id, CancellationToken cancellationToken = default);
    Task<ApiResponse<IEnumerable<T>>> GetAllAsync(int page, int pageSize,
        CancellationToken cancellationToken = default);
}