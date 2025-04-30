namespace WorkClientBlockChain.Utils.Interface;

public interface IProcessProgramSystem
{
    Task<bool> StartProgramAsync(string path, CancellationToken cts = default);
    Task<bool> StopProgramAsync(CancellationToken cts = default);
}