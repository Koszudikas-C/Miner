using LibEntitiesRemote.Entities;

namespace DataFictitiousRemote.Entities.Client;

public static class ClientInfoTest
{

    public static ClientInfo GetClientInfo()
    {
        return new ClientInfo
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
