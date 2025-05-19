using LibDto.Dto.Enum;

namespace LibDto.Dto;

public class DownloadRequestDto
{
    public Guid ClientInfoId { get; set; }
    public TypeAvailableFileDto FileAvailable { get; set; }
    public TypeExtensionFileDto TypeFile { get; set; }
}