using LibEntitiesRemote.Entities.Client.Abstract;
using LibEntitiesRemote.Entities.Client.HardwareInfo;

namespace DataFictitiousRemote.Entities.Client.HardwareInfo;

public class MemoryInfoTest : HardwareInfoBase
{
  public static MemoryInfo CreateFictitiousMemoryInfo()
  {
    return new MemoryInfo
    {
      Name = "System Memory",
      Status = "Active",
      UsagePercentage = 65.5,
      TotalCapacity = 32, // GB
    };
  }

  public static List<MemoryInfo> CreateFictitiousMemoryInfoList()
  {
    return new List<MemoryInfo>
    {
      CreateFictitiousMemoryInfo(),
      new MemoryInfo
      {
        Name = "GPU Memory",
        Status = "Idle",
        UsagePercentage = 10.2,
        TotalCapacity = 16 // GB
      }
    };
  }
}
