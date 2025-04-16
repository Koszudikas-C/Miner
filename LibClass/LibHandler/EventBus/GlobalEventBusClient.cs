using LibHandler.EventBus;

namespace LibHandler.EventBus;

public class GlobalEventBusClient : GlobalEventBusBase<GlobalEventBusClient>
{
    public override void Subscribe<TW>(Action<TW> handler)
    {
        var type = typeof(TW);
        if (!Handlers.ContainsKey(type))
        {
            Handlers[type] = new List<object>();
        }

        lock (Handlers[type])
        {
            var existingHandler = Handlers[type]
                .Cast<Action<TW>>()
                .FirstOrDefault(h => h.Method == handler.Method);

            if (existingHandler == null)
            {
                Handlers[type].Add(handler);
            }
        }
    }

    public override void SubscribeList<TW>(Action<List<TW>> handlers)
    {
        var type = typeof(List<TW>);
        if (!Handlers.ContainsKey(type))
        {
            Handlers[type] = new List<object>();
        }

        lock (Handlers[type])
        {
            Handlers[type].Add(handlers);
        }
    }

    public override void Publish<TW>(TW eventData)
    {
        var type = typeof(TW);
        if (!Handlers.TryGetValue(type, out var handlers)) return;
        foreach (var handler in handlers.ToList())
        {
            ((Action<TW>)handler)(eventData);
        }
    }

    public override void PublishList<TW>(List<TW> eventData)
    {
        var type = typeof(List<TW>);
        if (!Handlers.TryGetValue(type, out var handlers)) return;
        foreach (var handler in handlers.ToList())
        {
            ((Action<List<TW>>)handler)(eventData);
        }
    }

    public override void Unsubscribe<TW>(Action<TW> handler)
    {
        var type = typeof(TW);
        if (!Handlers.TryGetValue(type, out var handlers)) return;
        lock (handlers)
        {
            handlers.Remove(handler);
        }
    }

    public override void UnsubscribeList<TW>(Action<List<TW>> handlers)
    {
        var type = typeof(List<TW>);
        if (!Handlers.TryGetValue(type, out var handler)) return;

        lock (handler)
        {
            handler.Remove(handlers);
        }
    }

    public override void ResetInstance()
    {
        var newInstance = new GlobalEventBusClient();
        _instance = newInstance;
    }
}
