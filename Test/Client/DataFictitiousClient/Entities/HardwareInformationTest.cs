using LibHardwareInfoClient.Entities;

namespace DataFictitiousClient.Entities;

public static class HardwareInformationTest
{
    public static HardwareInformation CreateFictitiousHardwareInfo()
    {
        return new HardwareInformation
        {
            Temperature = 65.5,
            FanSpeed = 3200,
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
            }
        ];
    }
}
