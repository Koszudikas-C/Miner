using LibManagerFileRemote.Entities.Enum;

namespace LibEntitiesRemote.Entities;

public class DownloadRequest(TypeAvailableFile nameFile, TypeExtensionFile typeFile)
{
    public Guid ClientInfoId { get; set; }
    public TypeAvailableFile FileAvailable { get; set; } = nameFile;
    public TypeExtensionFile TypeFile { get; set; } = typeFile;
}
