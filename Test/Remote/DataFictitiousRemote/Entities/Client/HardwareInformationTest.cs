using DataFictitiousRemote.Entities.Client.HardwareInfo;
using LibEntitiesRemote.Entities.Client;

namespace DataFictitiousRemote.Entities.Client;

public static class HardwareInformationTest
{
    public static HardwareInformation CreateFictitiousHardwareInfo()
    {
        return new HardwareInformation
        {
            Temperature = 65.5,
            FanSpeed = 3200,
            TotalDiskSpace = "1TB",
            CpuInfo = CpuInfoTest.CreateFictitiousCpuInfo(),
            GpuInfo = GpuInfoTest.CreateFictitiousGpuInfo(),
            MemoryInfo = MemoryInfoTest.CreateFictitiousMemoryInfo()
        };
    }

    public static List<HardwareInformation> CreateFictitiousHardwareInfoList()
    {
        return
        [
            CreateFictitiousHardwareInfo(),
            new HardwareInformation
            {
                Temperature = 72.3,
                FanSpeed = 2800,
                TotalDiskSpace = "512GB",
                CpuInfo = CpuInfoTest.CreateFictitiousCpuInfo(),
                GpuInfo = GpuInfoTest.CreateFictitiousGpuInfo(),
                MemoryInfo = MemoryInfoTest.CreateFictitiousMemoryInfo()
            }
        ];
    }
}
