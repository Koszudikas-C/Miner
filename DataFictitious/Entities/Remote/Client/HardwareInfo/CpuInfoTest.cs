using LibRemoteAndClient.Entities.Remote.Client.Abstract;
using LibRemoteAndClient.Entities.Remote.Client.HardwareInfo;

namespace DataFictitious.Entities.Remote.Client.HardwareInfo;

public class CpuInfoTest : HardwareInfoBase
{
    public static CpuInfo CreateFictitiousCpuInfo()
    {
        return new CpuInfo
        {
            Name = "Intel Core i9-13900K",
            Status = "Active",
            UsagePercentage = 35.2,
            TotalCapacity = 5.8 // GHz
        };
    }
    
    public static List<CpuInfo> CreateFictitiousCpuInfoList()
    {
        return
        [
            CreateFictitiousCpuInfo(),
            new CpuInfo
            {
                Name = "AMD Ryzen 9 7950X",
                Status = "Idle",
                UsagePercentage = 12.7,
                TotalCapacity = 5.7 // GHz
            }
        ];
    }

}
