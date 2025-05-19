using LibManagerFile.Entities.Enum;

namespace LibRemoteAndClient.Entities.Remote;

public class DownloadRequest(TypeAvailableFile nameFile, TypeExtensionFile typeFile)
{
    public Guid ClientInfoId { get; set; }
    public TypeAvailableFile FileAvailable { get; set; } = nameFile;
    public TypeExtensionFile TypeFile { get; set; } = typeFile;
}