using LibEntitiesRemote.Entities.Client.Abstract;

namespace LibEntitiesRemote.Entities.Client.HardwareInfo;

public class GpuInfo : HardwareInfoBase
{
    public string Manufacturer { get; set; } = "Empty Manufacturer";
    public string VRAM { get; set; } = "Empty VRAM";
    public string VRAMUsed { get; set; } = "Empty VRAM Used";
    public string DriverVersion { get; set; } = "Empty Driver Version";
    public string Resolution { get; set; } = "Empty Resolution";
    public string RefreshRate { get; set; } = "Empty Refresh Rate";
}
