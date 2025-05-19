using LibDto.Dto.Enum;

namespace LibDto.Dto;

public class ParamsManagerOptionsResponseDto
{
    public object? ParamsForProcessResponse { get; set; }
    public string?  TypeName { get; set; }
    public TypeManagerOptionsResponseDto TypeManagerOptionsResponse { get; set; }
}