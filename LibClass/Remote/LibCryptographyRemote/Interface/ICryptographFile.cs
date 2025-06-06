using LibCryptographyRemote.Entities;

namespace LibCryptographyRemote.Interface;

public interface ICryptographFile
{
    byte[] SaveFile(ConfigCryptograph configCryptograph);
    string LoadFile(ConfigCryptograph configCryptograph);
}
