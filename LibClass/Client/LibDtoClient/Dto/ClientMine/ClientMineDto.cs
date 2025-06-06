namespace LibDtoClient.Dto.ClientMine;

public class ClientMineDto
{
    public Guid Id { get; set; }
    public Guid ClientInfoId { get; set; }
    public string? IpPublic { get; set; }
    public string? IpLocal { get; set; }
    public string? Name { get; set; }
    public bool IsAuthSocks5 { get; set; }
    public string? PathLocal { get; set; }
    public bool IsStatus { get; set; }
    public bool IsStatusMining { get; set; }
    public string? PlatformSystem { get; set; }
    public string? So { get; set; }
    public int HoursRunning { get; set; }
    public HardwareInformationDto? HardwareInfo { get; set; }
    public MiningStatsDto? Mining { get; set; }
}
