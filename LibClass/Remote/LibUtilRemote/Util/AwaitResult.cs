using LibUtilRemote.Interface;

namespace LibUtilRemote.Util;

public class AwaitResult : IAwaitResult
{
    public async Task<object?> AwaitTaskObj(object obj, int repeat = 1,
        TimeSpan timeout = default)
    {
        for (var i = 0; i < repeat; i++)
        {
            if (obj is not null) break;
            await Task.Delay(timeout);
            continue;
        }   
        return obj; 
    }
}
