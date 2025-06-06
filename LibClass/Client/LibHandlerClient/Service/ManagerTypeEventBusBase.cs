using System.Text.Json;

namespace LibHandlerClient.Service;

public abstract class ManagerTypeEventBusBase
{
    public abstract void PublishEventType(JsonElement data);

    public abstract void PublishListEventType(List<JsonElement> listData);
}
