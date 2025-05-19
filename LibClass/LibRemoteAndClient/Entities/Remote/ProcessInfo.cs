namespace LibRemoteAndClient.Entities.Remote;

public class ProcessInfo
{
    public string? Name { get; set; }
    public int Port { get; set; }
    public int Pid { get; set; }
    public bool ProcessInit { get; set; }
    public string? LastError { get; set; }
}