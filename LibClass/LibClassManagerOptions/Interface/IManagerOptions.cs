using LibClassManagerOptions.Entities;
using LibDto.Dto;

namespace LibClassManagerOptions.Interface;

public interface IManagerOptions<T>
{
    Task InitializeOptionsAsync(ParamsManagerOptions<T> paramsManagerOptions, CancellationToken cts = default);
    
    Task ResponseOptionsAsync(ParamsManagerOptionsResponseDto paramsManagerOptionsResponseDto, CancellationToken cts = default);
}