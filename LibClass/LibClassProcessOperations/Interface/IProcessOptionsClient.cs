namespace LibClassProcessOperations.Interface;

public interface IProcessOptionsClient
{
    Task ProcessAsync(object obj, CancellationToken cts = default);
}