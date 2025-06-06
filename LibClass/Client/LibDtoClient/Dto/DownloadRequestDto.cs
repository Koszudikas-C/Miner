using LibDtoClient.Dto.Enum;

namespace LibDtoClient.Dto;

public class DownloadRequestDto
{
    public Guid ClientInfoId { get; set; }
    public TypeAvailableFileDto FileAvailable { get; set; }
    public TypeExtensionFileDto TypeFile { get; set; }
}
