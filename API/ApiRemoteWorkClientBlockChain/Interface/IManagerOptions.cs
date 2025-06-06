using LibDtoRemote.Dto;
using LibEntitiesRemote.Entities.Params;

namespace ApiRemoteWorkClientBlockChain.Interface;

public interface IManagerOptions<T>
{
    Task InitializeOptionsAsync(ParamsManagerOptions<T> paramsManagerOptions, CancellationToken cts = default);
    
    Task ResponseOptionsAsync(ParamsManagerOptionsResponseDto paramsManagerOptionsResponseDto, CancellationToken cts = default);
}
