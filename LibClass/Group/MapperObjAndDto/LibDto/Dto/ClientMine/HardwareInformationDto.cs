using LibDto.Dto.ClientMine.HardwareInfo;

namespace LibDto.Dto.ClientMine;

public class HardwareInformationDto
{
    public double Temperature { get; set; }
    public int FanSpeed { get; set; }
    public CpuInfoDto? CpuInfo { get; set; }
    public GpuInfoDto? GpuInfo { get; set; }
    public MemoryInfoDto? MemoryInfo { get; set; }
    public string? TotalDiskSpace { get; set; }
}