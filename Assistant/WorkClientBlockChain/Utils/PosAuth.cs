using LibDto.Dto.ClientMine;
using LibMapperObj.Interface;
using LibMapperObj.Service;

namespace WorkClientBlockChain.Utils;

public static class PosAuthUtil
{
    private static readonly IMapperObj MapperObj = new MapperObjService();
    public static ClientMineDto GetClientDtoDefault()
    {
        var clientMine = new LibRemoteAndClient.Entities.Client.ClientMine();
        return MapperObj.Map<LibRemoteAndClient.Entities.Client.ClientMine, ClientMineDto>(clientMine);
    }
}