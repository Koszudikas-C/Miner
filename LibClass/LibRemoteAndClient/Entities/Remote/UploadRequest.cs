using LibManagerFile.Entities.Enum;
using TypeFile = ServerBlockChain.Entities.Enum.TypeFile;

namespace LibRemoteAndClient.Entities.Remote;

public class UploadRequest(TypeAvailableFile fileAvailableFile, TypeExtensionFile typeFile)
{
    public TypeAvailableFile TypeFileAvailable { get; set; } = fileAvailableFile;
    public TypeExtensionFile TypeFile { get; set; } = typeFile;
}