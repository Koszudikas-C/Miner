using LibEntitiesClient.Entities;

namespace WorkClientBlockChain.Interface;

public interface IGetProcessInfo
{
    public Exception? GetLastError { get; protected set; }
    List<ProcessInfo> GetProcessInfo(string nameProcess);
    ProcessInfo GetProcessInfo(int port);
}
