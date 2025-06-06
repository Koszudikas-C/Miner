using LibEntitiesClient.Entities;

namespace DataFictitiousClient.Entities;

public static class ClientInfoTest
{

    public static ClientInfo GetClientInfo()
    {
        return new ClientInfo()
        {
            Id = Guid.NewGuid(),
            ClientMine = ClientMineTest.CreateFictitiousClientMine()
        };
    }
    public static List<ClientInfo> GetClientInfoList()
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
