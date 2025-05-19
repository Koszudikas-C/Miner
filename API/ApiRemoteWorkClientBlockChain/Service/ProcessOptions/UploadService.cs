using LibPreparationFile.Interface;
using LibUpload;
using LibUpload.Interface;

namespace ApiRemoteWorkClientBlockChain.Service.ProcessOptions;

public class UploadService(IPreparationFile preparationFile, ILogger<UploadService> logger) : IUpload
{
    public InfoFile Upload(string path)
    {
        try
        {
            var nameFile = preparationFile.GetFileName(path);
            var dataLength = preparationFile.GetFileLengthByte(path);
            var dataByte =  preparationFile.GetFileByte(path).Result;

            return new InfoFile(nameFile)
            {
                LengthFile = dataLength,
                DataByte = dataByte
            };
        }
        catch (Exception e)
        {
            logger.LogInformation($"An error occurred in the file preparation for upload. Error: {e.Message}");
            throw new Exception();
        }
    }
}