namespace LibPreparationFileRemote.Interface;

public interface IPreparationFile
{
    string GetFileName(string path);
    
    byte[] GetFileLengthByte(string path);

    Task<byte[]> GetFileByte(string path);
}
