using LibDtoRemote.Dto.Enum;

namespace LibDtoRemote.Dto;

public class ParamsManagerOptionsResponseDto
{
    public object? ParamsForProcessResponse { get; set; }
    public string?  TypeName { get; set; }
    public TypeManagerOptionsResponseDto TypeManagerOptionsResponse { get; set; }
}
