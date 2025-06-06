using LibManagerFileClient.Entities.Enum;

namespace LibEntitiesClient.Entities.Params;

public class UploadRequest(TypeAvailableFile fileAvailableFile, TypeExtensionFile typeFile)
{
  public TypeAvailableFile TypeFileAvailable { get; set; } = fileAvailableFile;
  public TypeExtensionFile TypeFile { get; set; } = typeFile;
}
