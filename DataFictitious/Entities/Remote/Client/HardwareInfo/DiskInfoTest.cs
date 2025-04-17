using LibRemoteAndClient.Entities.Client.Abstract;
using LibRemoteAndClient.Entities.Remote.Client.HardwareInfo;

namespace DataFictitious.Entities.Remote.Client.HardwareInfo;

public class DiskInfoTest : HardwareInfoBase
{
    public static DiskInfo CreateFictitiousDiskInfo()
    {
        return new DiskInfo
        {
            Name = "Samsung SSD 980 PRO",
            Status = "Healthy",
            UsagePercentage = 68.4,
            TotalCapacity = 1024 // GB
        };
    }
    
    public static List<DiskInfo> CreateFictitiousDiskInfoList()
    {
        return
        [
            CreateFictitiousDiskInfo(),
            new DiskInfo
            {
                Name = "Seagate Barracuda 2TB",
                Status = "Healthy",
                UsagePercentage = 44.9,
                TotalCapacity = 2048 // GB
            }
        ];
    }

}