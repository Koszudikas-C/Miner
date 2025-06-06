using LibDtoRemote.Dto;
using LibEntitiesRemote.Entities.Params;
using LibMapperObjRemote.Interface;
using LibMapperObjRemote.Service;

namespace ApiRemoteWorkClientBlockChain.Utils;

public static class AuthSocks5Util
{
    private static readonly IMapperObj MapperObj = new MapperObjService();
    
    public static ParamsSocks5Dto GetParamsSocks5Dto(ParamsSocks5 paramsSock5)
    {
        return MapperObj.Map<ParamsSocks5, ParamsSocks5Dto>(paramsSock5);
    }

    public static ParamsSocks5Dto GetParamsSocks5DtoDefault()
    {
        var paramsSocks5 = new ParamsSocks5();
        paramsSocks5.ParamsGetProcessInfo.Port = 9050;

        return MapperObj.Map<ParamsSocks5, ParamsSocks5Dto>(paramsSocks5);
    }
}
