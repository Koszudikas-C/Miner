using LibCryptography.Entities;

namespace LibCryptography.Interface;

public interface ICryptographFile
{
    byte[] SaveFile<T>(ConfigCryptograph<T> configCryptograph);
    string LoadFile<T>(ConfigCryptograph<T> configCryptograph);
}
