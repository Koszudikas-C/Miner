using LibRemoteAndClient.Connection;

namespace LibRemoteAndClient.Entities.Client;

public class ClientMine()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientInfoId { get; set; } = GuidToken.GuidTokenGlobal;
    public string IpPublic { get; set; } = ConnectionHost.GetPublicIPAdress().Result.TrimEnd('\n', '\r');
    public string IpLocal { get; set; } = string.Empty;
    public string Name { get; set; } = Environment.MachineName;
    public string PathLocal { get; set; } = AppDomain.CurrentDomain.BaseDirectory;
    public bool IsStatus { get; set; } = true;
    public bool IsStatusMining { get; set; } = false;
    public string So { get; set; } = Environment.OSVersion.ToString();
    public int HoursRunning { get; set; }
    public HardwareInformation? HardwareInfo { get; set; }
    public MiningStats Mining { get; set; } = new MiningStats();
}

