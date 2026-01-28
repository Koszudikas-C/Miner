using LibHandler.Interface;
using System.Collections.Concurrent;

namespace LibHandler.EventBus;

public class GlobalEventBusRemote : GlobalEventBusBase<GlobalEventBusRemote>, IEventBus
{
    private string GetKey(Type t1, Type t2) => $"{t1.FullName}_{t2.FullName}";

    public override void Subscribe<TW>(Action<TW> handler)
    {
        var type = typeof(TW);
        if (!Handlers.ContainsKey(type))
            Handlers[type] = [];

        lock (Handlers[type])
        {
            if (Handlers[type].All(h => h.Method != handler.Method))
                Handlers[type].Add(handler);
        }
    }

    public override void Subscribe<TW, T>(Action<TW, T> handler)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.ContainsKey(key))
            MultiHandlers[key] = [];

        lock (MultiHandlers[key])
        {
            if (MultiHandlers[key].All(h => h.Method != handler.Method))
                MultiHandlers[key].Add(handler);
        }
    }

    public override void SubscribeList<TW>(Action<List<TW>> handlers)
    {
        var type = typeof(List<TW>);
        if (!Handlers.ContainsKey(type))
            Handlers[type] = [];

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
            ((Action<TW>)handler)(eventData);
    }

    public override void Publish<TW, T>(TW data1, T data2)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.TryGetValue(key, out var handlers)) return;

        foreach (var handler in handlers.ToList())
            ((Action<TW, T>)handler)(data1, data2);
    }

    public override void PublishList<TW>(List<TW> eventData)
    {
        var type = typeof(List<TW>);
        if (!Handlers.TryGetValue(type, out var handlers)) return;

        foreach (var handler in handlers.ToList())
            ((Action<List<TW>>)handler)(eventData);
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

    public override void Unsubscribe<TW, T>(Action<TW, T> handler)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.TryGetValue(key, out var handlers)) return;

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

    public override void ClearSubscribers()
    {
        Handlers.Clear();
        MultiHandlers.Clear();
    }

    public override void ResetInstance()
    {
        _instance = new GlobalEventBusRemote();
    }
}
