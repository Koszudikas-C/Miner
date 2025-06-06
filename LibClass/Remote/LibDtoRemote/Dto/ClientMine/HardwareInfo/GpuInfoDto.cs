using LibDtoRemote.Dto.ClientMine.Abstract;

namespace LibDtoRemote.Dto.ClientMine.HardwareInfo;

public class GpuInfoDto : HardwareInfoBaseDto
{
    public string? Manufacturer { get; set; }
    public string? VRAM { get; set; }
    public string? VRAMUsed { get; set; }
    public string? DriverVersion { get; set; }
    public string? Resolution { get; set; }
    public string? RefreshRate { get; set; }
}
