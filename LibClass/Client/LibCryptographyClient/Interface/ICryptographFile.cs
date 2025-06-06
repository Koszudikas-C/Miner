using LibCryptographyClient.Entities;

namespace LibCryptographyClient.Interface;

public interface ICryptographFile
{
    byte[] SaveFile(ConfigCryptograph configCryptograph);
    string LoadFile(ConfigCryptograph configCryptograph);
}
