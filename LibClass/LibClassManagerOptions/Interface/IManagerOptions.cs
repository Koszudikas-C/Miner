using LibClassManagerOptions.Entities.Enum;

namespace LibClassManagerOptions.Interface;

public interface IManagerOptions
{
    Task InitializeOptions(TypeManagerOptions typeManagerOptions, CancellationToken cts = default);
    
    Task ResponseOptions(TypeManagerResponseOperations typeManagerResponseOperations, CancellationToken cts = default);
}