using LibCryptography.Entities;

namespace LibCryptography.Interface;

public interface ICryptographFile
{
    byte[] SaveFile(ConfigCryptograph configCryptograph);
    string LoadFile(ConfigCryptograph configCryptograph);
}
