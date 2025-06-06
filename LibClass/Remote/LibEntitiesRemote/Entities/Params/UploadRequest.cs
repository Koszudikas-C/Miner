using LibManagerFileRemote.Entities.Enum;

namespace LibEntitiesRemote.Entities.Params;

public class UploadRequest(TypeAvailableFile fileAvailableFile, TypeExtensionFile typeFile)
{
  public TypeAvailableFile TypeFileAvailable { get; set; } = fileAvailableFile;
  public TypeExtensionFile TypeFile { get; set; } = typeFile;
}
