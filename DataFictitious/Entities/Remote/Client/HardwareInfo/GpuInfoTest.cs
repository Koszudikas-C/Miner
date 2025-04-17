using LibRemoteAndClient.Entities.Remote.Client.Abstract;
using LibRemoteAndClient.Entities.Remote.Client.HardwareInfo;

namespace DataFictitious.Entities.Remote.Client.HardwareInfo;

public class GpuInfoTest : HardwareInfoBase
{
    public static GpuInfo CreateFictitiousGpuInfo()
    {
        return new GpuInfo
        {
            Name = "NVIDIA GeForce RTX 3080",
            Status = "Active",
            UsagePercentage = 72.3,
            TotalCapacity = 10, // GB de VRAM
            Manufacturer = "NVIDIA",
            VRAM = "10 GB",
            VRAMUsed = "7.2 GB",
            DriverVersion = "551.23",
            Resolution = "2560x1440",
            RefreshRate = "144Hz"
        };
    }
    
    public static List<GpuInfo> CreateFictitiousGpuInfoList()
    {
        return
        [
            CreateFictitiousGpuInfo(),
            new GpuInfo
            {
                Name = "AMD Radeon RX 6800 XT",
                Status = "Idle",
                UsagePercentage = 15.7,
                TotalCapacity = 16,
                Manufacturer = "AMD",
                VRAM = "16 GB",
                VRAMUsed = "2.1 GB",
                DriverVersion = "23.4.1",
                Resolution = "1920x1080",
                RefreshRate = "60Hz"
            }
        ];
    }
}
