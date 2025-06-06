using LibDtoClient.Dto.Enum;

namespace LibDtoClient.Dto;

public class ParamsManagerOptionsDto<T>
{
    public T? ParamsForProcess { get; set; }
    public TypeManagerOptionsDto TypeManagerOptions { get; set; }
    
}
