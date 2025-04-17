using System.Text.Json.Serialization;
using LibRemoteAndClient.Entities.Remote.Client;
using LibRemoteAndClient.Util;

namespace DataFictitious.Entities.Remote.Client;

public class LogEntryTest
{
    public static LogEntry CreateFictitiousLogEntry()
    {
        return new LogEntry
        {
            Ip = "192.168.1.1",
            Level = "Info",
            Message = "Sample log message",
            ApplicationName = "SampleApp",
            MachineName = "Machine-001",
            ProcessId = "1234",
            Architecture = "x64",
            Version = "1.0.0",
            UserName = "TestUser",
            UserDomain = "TestDomain",
            ThreadId = "5678",
            Exception = "None",
            Source = "SomeSource",
            StackTrace = "No stack trace available",
        };
    }

    public static List<LogEntry> CreateFictitiousLogEntryList()
    {
        return
        [
            CreateFictitiousLogEntry(),
            new LogEntry
            {
                Ip = "192.168.1.2",
                Level = "Error",
                Message = "An error occurred",
                ApplicationName = "AnotherApp",
                MachineName = "Machine-002",
                ProcessId = "5678",
                Architecture = "x64",
                Version = "2.0.0",
                UserName = "User2",
                UserDomain = "Domain2",
                ThreadId = "1234",
                Exception = "System.NullReferenceException",
                Source = "ErrorSource",
                StackTrace = "Stack trace details...",
            }
        ];
    }
}