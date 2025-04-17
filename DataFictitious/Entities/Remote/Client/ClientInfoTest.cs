using LibRemoteAndClient.Entities.Remote.Client;

namespace DataFictitious.Entities.Remote.Client;

public static class ClientInfoTest
{

    public static ClientInfo ClientInfo()
    {
        return new ClientInfo()
        {
            Id = Guid.NewGuid(),
            ClientMine = ClientMineTest.CreateFictitiousClientMine()
        };
    }
    public static List<ClientInfo> ClientInfoList()
    {
        return
        [
            new ClientInfo
            {
                Id = Guid.NewGuid(),
                ClientMine = ClientMineTest.CreateFictitiousClientMine()
            },

            new ClientInfo
            {
                Id = Guid.NewGuid(),
                ClientMine = ClientMineTest.CreateFictitiousClientMine()
            }
        ];
    }
}