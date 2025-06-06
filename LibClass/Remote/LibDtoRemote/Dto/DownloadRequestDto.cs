using LibDtoRemote.Dto.Enum;

namespace LibDtoRemote.Dto;

public class DownloadRequestDto
{
    public Guid ClientInfoId { get; set; }
    public TypeAvailableFileDto FileAvailable { get; set; }
    public TypeExtensionFileDto TypeFile { get; set; }
}
