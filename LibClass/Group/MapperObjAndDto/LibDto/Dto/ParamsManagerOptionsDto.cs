using LibDto.Dto.Enum;

namespace LibDto.Dto;

public class ParamsManagerOptionsDto<T>
{
    public T? ParamsForProcess { get; set; }
    public TypeManagerOptionsDto TypeManagerOptions { get; set; }
    
}