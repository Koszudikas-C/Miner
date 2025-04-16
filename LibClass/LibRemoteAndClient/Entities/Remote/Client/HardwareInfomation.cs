using LibRemoteAndClient.Entities.Remote.Client.HardwareInfo;

namespace LibRemoteAndClient.Entities.Remote.Client;

public class HardwareInfomation
{
    public double Temperature { get; set; }
    public int FanSpeed { get; set; }
    public CpuInfo? CpuInfo { get; set; }
    public GpuInfo? GpuInfo { get; set; }
    public MemoryInfo? MemoryInfo { get; set; }
    public string? TotalDiskSpace { get; set; }
}
