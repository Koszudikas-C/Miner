namespace LibUtilRemote.Interface;

public interface IAwaitResult
{
    Task<object?> AwaitTaskObj(object obj, int repeat = 1, TimeSpan timeout = default);
}
