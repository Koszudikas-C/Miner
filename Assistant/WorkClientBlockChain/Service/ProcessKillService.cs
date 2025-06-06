using System.Diagnostics;
using LibEntitiesClient.Entities;
using LibProcessClient.Interface;

namespace WorkClientBlockChain.Service;

public class ProcessKillService : IProcessKill
{
    private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();
    private readonly ILogger<ProcessKillService> _logger = new Logger<ProcessKillService>(LoggerFactory);
    public bool ProcessKill(object obj)
    {
        if (obj is not ProcessInfo processInfo)
            throw new InvalidCastException("The object type is incorrect");
        try
        {
            var process = Process.GetProcessById(processInfo.Pid);
            process.Kill();
            _logger.LogInformation("Process successfully finished!");

            return true;
        }
        catch (Exception e)
        {
            throw new Exception($"Check if the process pid is valid. Error{e.Message}");
        }
    }
}
