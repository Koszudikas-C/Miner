namespace LibEntitiesClient.Entities.Params;

public class UploadResponseHeader(string nameFile, long lenghtFile)
{
    public string NameFile { get; set; } = nameFile;
    public long LenghtFile { get; set; } = lenghtFile;
}
