using System.Collections.Concurrent;
using System.Net.Sockets;
using LibEntitiesRemote.Interface;

namespace LibSocketAndSslStreamRemote.Interface;

public interface IManagerSocketConnected
{
    event Action<ConcurrentDictionary<string, Socket>> ListSocketConnectedAct;
    event Action<ConcurrentDictionary<string, int>> DictionaryClientConnectedAct;
    bool CheckStateSocket(ISocketWrapper socket);
    
}