namespace ApiRemoteWorkClientBlockChain.Interface.Repository;

public interface IRepositoryBase<in TW, T>
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    T GetById(TW id);
    List<T> GetAll();

    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task<T> GetByIdAsync(TW id, CancellationToken cancellationToken = default);
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken = default);
}