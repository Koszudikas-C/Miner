namespace LibUploadRemote;

public class InfoFile(string nameFile)
{
    public string NameFile { get; set; } = nameFile;
    public byte[]? LengthFile { get; set; }
    public byte[]? DataByte { get; set; }
}
