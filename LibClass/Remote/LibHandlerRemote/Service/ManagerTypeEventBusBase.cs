using System.Text.Json;

namespace LibHandlerRemote.Service;

public abstract class ManagerTypeEventBusBase
{
    public abstract void PublishEventType(JsonElement data);

    public abstract void PublishListEventType(List<JsonElement> listData);
    
    
}
