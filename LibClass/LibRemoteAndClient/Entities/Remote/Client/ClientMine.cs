namespace LibRemoteAndClient.Entities.Remote.Client;

public sealed class ClientMine()
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientInfoId { get; set; }
    public string IpPublic { get; set; } = string.Empty;
    public string IpLocal { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PathLocal { get; set; }
    public bool IsStatus { get; set; }
    public bool IsStatusMining { get; set; }
    public string So { get; set; } = string.Empty;
    public int HoursRunning { get; set; }
    public HardwareInformation? HardwareInfo { get; set; }
    public MiningStats? Mining { get; set; }
}