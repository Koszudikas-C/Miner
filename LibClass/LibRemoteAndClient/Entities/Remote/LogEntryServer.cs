using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
using LibRemoteAndClient.Util;

namespace LibRemoteAndClient.Entities.Remote;

public class LogEntryServer
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string ApplicationName { get; set; } = string.Empty;
    public string MachineName { get; } = Environment.MachineName;
    public string ProcessId { get; } = Environment.ProcessId.ToString();
    public string Architecture { get; } = RuntimeInformation.OSArchitecture.ToString();
    public string Version { get; } = Environment.Version.ToString();
    public string UserName { get; } = Environment.UserName;
    public string UserDomain { get; } = Environment.UserDomainName;
    public string ThreadId { get; set; } = Environment.CurrentManagedThreadId.ToString();
    public string Exception { get; set; } = string.Empty;

    [JsonConverter(typeof(ObjectToStringConverter))]
    public object? Source { get; set; }

    public string StackTrace { get; set; } = string.Empty;

    [JsonIgnore] public IntPtr Handle { get; set; }
}