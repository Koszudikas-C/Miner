using LibDtoRemote.Dto.Enum;

namespace LibDtoRemote.Dto;

public class ParamsManagerOptionsDto<T>
{
    public T? ParamsForProcess { get; set; }
    public TypeManagerOptionsDto TypeManagerOptions { get; set; }
    
}
