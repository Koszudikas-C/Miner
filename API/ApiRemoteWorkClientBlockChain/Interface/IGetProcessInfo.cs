using LibEntitiesRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IGetProcessInfo
{
    public Exception? GetLastError { get; protected set; }
    List<ProcessInfo> GetProcessInfo(string nameProcess);
    ProcessInfo GetProcessInfo(int port);
}
