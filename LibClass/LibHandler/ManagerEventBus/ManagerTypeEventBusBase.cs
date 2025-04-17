using System.Text.Json;

namespace LibHandler.ManagerEventBus;

public abstract class ManagerTypeEventBusBase()
{
    public abstract void PublishEventType(JsonElement listData);

    public abstract void PublishListEventType(List<JsonElement> listData);
    
    
}