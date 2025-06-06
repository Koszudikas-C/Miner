
using LibDtoRemote.Dto;
using LibEntitiesRemote.Entities.Params;
using LibEntitiesRemote.Entities.Params.Enum;
using LibMapperObjRemote.Interface;
using LibMapperObjRemote.Service;

namespace ApiRemoteWorkClientBlockChain.Utils;

public static class ManagerOptionsUtil
{
    public static readonly IMapperObj MapperObj = new MapperObjService();

    public static ParamsManagerOptionsDto<T> GetParamsManagerOptionsDto<T>(ParamsManagerOptions<T> paramsManagerOptions)
    {
        return MapperObj.Map<ParamsManagerOptions<T>, ParamsManagerOptionsDto<T>>(paramsManagerOptions);
    }

    public static ParamsManagerOptionsDto<T> GetParamsGetOptionsDtoDefault<T>(TypeManagerOptions typeManagerOptions,
        T paramsForProcess)
    {
        var paramsManagerOptions = new ParamsManagerOptions<T>(typeManagerOptions, paramsForProcess);

        return MapperObj.Map<ParamsManagerOptions<T>, ParamsManagerOptionsDto<T>>(paramsManagerOptions);
    }
    
    public static ParamsManagerOptionsResponse GetParamsManagerOptionsResponse(
        ParamsManagerOptionsResponseDto paramsManagerOptionsResponseDto)
    {
        return MapperObj.Map<ParamsManagerOptionsResponseDto, ParamsManagerOptionsResponse>(paramsManagerOptionsResponseDto);
    }
}
