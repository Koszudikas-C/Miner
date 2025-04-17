using LibRemoteAndClient.Entities.Remote.Client;

namespace DataFictitious.Entities.Remote.Client;

public static class ClientMineTest
{
    public static ClientMine CreateFictitiousClientMine()
    {
        return new ClientMine
        {
            Id = Guid.NewGuid(),
            ClientInfoId = Guid.NewGuid(),
            IpPublic = "187.123.45.67",
            IpLocal = "192.168.1.100",
            Name = "MinerClient01",
            IsStatus = true,
            IsStatusMining = true,
            So = "Ubuntu 22.04",
            HoursRunning = 12,
            HardwareInfo = HardwareInformationTest.CreateFictitiousHardwareInfo(),
            Mining = null
        };
    }

    public static List<ClientMine> CreateFictitiousClientMineList()
    {
        return
        [
            CreateFictitiousClientMine(),
            new ClientMine
            {
                Id = Guid.NewGuid(),
                ClientInfoId = Guid.NewGuid(),
                IpPublic = "200.98.34.76",
                IpLocal = "192.168.1.101",
                Name = "MinerClient02",
                IsStatus = true,
                IsStatusMining = false,
                So = "Windows 11",
                HoursRunning = 5,
                HardwareInfo = HardwareInformationTest.CreateFictitiousHardwareInfo(),
                Mining = null
            }
        ];
    }
}