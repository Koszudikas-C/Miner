using LibDtoClient.Dto.ClientMine;
using LibEntitiesClient.Entities;
using LibMapperObjClient.Interface;
using LibMapperObjClient.Service;

namespace WorkClientBlockChain.Utils;

public static class PosAuthUtil
{
    private static readonly IMapperObj MapperObj = new MapperObjService();
    public static ClientMineDto GetClientDtoDefault()
    {
        var clientMine = new ClientMine();
        return MapperObj.Map<ClientMine, ClientMineDto>(clientMine);
    }
}
