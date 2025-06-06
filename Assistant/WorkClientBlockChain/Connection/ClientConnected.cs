using LibEntitiesClient.Entities;
using LibHandlerClient.Entities;
using WorkClientBlockChain.Connection.Interface;

namespace WorkClientBlockChain.Connection;

public class ClientConnected : IClientConnected
{
  private static readonly object Lock = new();
  private static ClientInfo? _clientInfo;
  private static ObjSocketSslStream? _objSocketSslStream;

  private static readonly GlobalEventBus GlobalEventBus = GlobalEventBus.Instance;

  public ClientConnected()
  {
    GlobalEventBus.Subscribe<ClientInfo>(OnClientInfoReceived);
    GlobalEventBus.Subscribe<ObjSocketSslStream>(OnObjSocketSslStream);
  }

  private void OnClientInfoReceived(ClientInfo? obj)
  {
    SetClientInfo(obj);
  }

  private static void OnObjSocketSslStream(ObjSocketSslStream obj)
  {
    SetObjSocketSslStream(obj);
  }

  public ClientInfo? GetClientInfo()
  {
    lock (Lock)
    {
      return _clientInfo;
    }
  }

  public ObjSocketSslStream? GetObjSocketSslStream()
  {
    lock (Lock)
    {
      return _objSocketSslStream;
    }
  }

  public void SetClientInfo(ClientInfo? info)
  {
    lock (Lock)
    {
      _clientInfo = info;
    }
  }

  public static void SetObjSocketSslStream(ObjSocketSslStream obj)
  {
    lock (Lock)
    {
      _objSocketSslStream = obj;
    }
  }

  public void Reset()
  {
    SetClientInfo(null);
  }
}
