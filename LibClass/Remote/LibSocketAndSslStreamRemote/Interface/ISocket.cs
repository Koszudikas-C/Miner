using LibSocketAndSslStreamRemote.Entities;
using LibSocketAndSslStreamRemote.Entities.Enum;

namespace LibSocketAndSslStreamRemote.Interface;

public interface ISocket
{
    void InitializeRemote(uint port, int maxConnection, 
        TypeAuthMode typeAuthMode);
}
