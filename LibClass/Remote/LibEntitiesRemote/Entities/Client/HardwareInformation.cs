using LibEntitiesRemote.Entities.Client.HardwareInfo;

namespace LibEntitiesRemote.Entities.Client;

public class HardwareInformation
{
    public double Temperature { get; set; }
    public int FanSpeed { get; set; }
    public CpuInfo? CpuInfo { get; set; }
    public GpuInfo? GpuInfo { get; set; }
    public MemoryInfo? MemoryInfo { get; set; }
    public string? TotalDiskSpace { get; set; }
}
