using LibHandlerRemote.Interface;

namespace LibHandlerRemote.Entities;

public class GlobalEventBus : GlobalEventBusBase<GlobalEventBus>, IEventBus
{
    public override void Subscribe<TW>(Action<TW> handler)
    {
        var type = CheckBeforeHandler<TW>();

        lock (Handlers[type])
        {
            if (Handlers[type].All(h => h.Method != handler.Method))
                Handlers[type].Add(handler);
        }
    }

    public override void Subscribe<TW>(Action<List<TW>> handlers)
    {
        var type = CheckBeforeHandler<TW>();
        lock (Handlers[type])
        {
            if (Handlers[type].All(h => h.Method != handlers.Method))
                Handlers[type].Add(handlers);
        }
    }

    public override void SubscribeFunc<TW>(Func<TW, CancellationToken, Task> funcHandler)
    {
        var type = CheckBeforeHandlerFunc<TW>();

        lock (HandlersFunc[type])
        {
            HandlersFunc[type].Add(funcHandler);
        }
    }

    public override void SubscribeListFunc<TW>(Func<List<TW>, CancellationToken, Task> funcHandlers)
    {
        var type = CheckBeforeHandlerFunc<List<TW>>();

        lock (HandlersFunc[type])
        {
            HandlersFunc[type].Add(funcHandlers);
        }
    }

    public override void Publish<TW>(TW eventData)
    {
        var type = typeof(TW);
        if (!Handlers.TryGetValue(type, out var handlers)) return;

        foreach (var handler in handlers.ToList())
            ((Action<TW>)handler)(eventData);
    }

    public override void Publish<TW>(List<TW> eventData)
    {
        var type = typeof(List<TW>);
        if (!Handlers.TryGetValue(type, out var handlers)) return;

        foreach (var handler in handlers.ToList())
            ((Action<List<TW>>)handler)(eventData);
    }

    public override async Task PublishAsync<TW>(TW eventData, CancellationToken cts = default)
    {
        var type = typeof(TW);
        if (!HandlersFunc.TryGetValue(type, out var handlers)) return;

        foreach (var handler in handlers.Cast<Func<TW, CancellationToken, Task>>())
        {
            await handler(eventData, cts);
        }
    }

    public override async Task PublishAsync<TW>(List<TW> eventData, CancellationToken cts = default)
    {
        var type = typeof(List<TW>);

        if (!HandlersFunc.TryGetValue(type, out var handlers)) return;

        foreach (var handler in handlers.Cast<Func<List<TW>, CancellationToken, Task>>())
        {
            await handler(eventData, cts);
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

    public override void Unsubscribe<TW>(Action<List<TW>> handlers)
    {
        var type = typeof(List<TW>);
        if (!Handlers.TryGetValue(type, out var handler)) return;

        lock (handler)
        {
            handler.Remove(handlers);
        }
    }

    public override void UnsubscribeFunc<TW>(Action<TW> handler)
    {
        var type = typeof(TW);

        if (!HandlersFunc.TryGetValue(type, out var handlers)) return;

        lock (HandlersFunc)
        {
            handlers.Remove(handler);
        }
    }

    public override void UnsubscribeListFunc<TW>(Action<List<TW>> handlers)
    {
        var type = typeof(List<TW>);

        if (!Handlers.TryGetValue(type, out var handlersThis)) return;

        lock (handlersThis)
        {
            handlersThis.Remove(handlers);
        }
    }

    // Methods with two generics types
    public override void Subscribe<TW, T>(Action<Tuple<TW, T>> handler)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.ContainsKey(key))
            MultiHandlers[key] = [];

        lock (MultiHandlers[key])
        {
            MultiHandlers[key].Add(handler);
        }
    }

    public override void Subscribe<TW, T>(Action<List<Tuple<TW, T>>> handler)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.ContainsKey(key))
            MultiHandlers[key] = [];

        lock (MultiHandlers[key])
        {
            MultiHandlers[key].Add(handler);
        }
    }

    public override void SubscribeFunc<TW, T>(Func<TW, T, CancellationToken, Task> handler)
    {
        var key = GetKey(typeof(TW), typeof(T));

        if (!MultiHandlersFunc.ContainsKey(key))
            MultiHandlersFunc[key] = [];

        lock (MultiHandlersFunc[key])
        {
            MultiHandlersFunc[key].Add(handler);
        }
    }

    public override void SubscribeListFunc<TW, T>(Func<List<Tuple<TW, T>>, CancellationToken, Task> handler)
    {
        var key = GetKey(typeof(TW), typeof(T));

        if (!MultiHandlersFunc.ContainsKey(key))
            MultiHandlersFunc[key] = [];

        lock (MultiHandlersFunc[key])
        {
            MultiHandlersFunc[key].Add(handler);
        }
    }

    public override void Publish<TW, T>(TW data1, T data2)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.TryGetValue(key, out var handlers)) return;

        foreach (var handler in handlers.ToList())
            ((Action<TW, T>)handler)(data1, data2);
    }

    public override void Publish<TW, T>(List<Tuple<TW, T>> handlers)
    {
        var key = GetKey(typeof(TW), typeof(T));

        if (!MultiHandlers.TryGetValue(key, out var handlersThis)) return;

        foreach (var handler in handlersThis)
            ((Action<List<Tuple<TW, T>>>)handler)(handlers);
    }

    public override async Task PublishAsync<TW, T>(TW eventData, T eventData1,
        CancellationToken cts = default)
    {
        var key = GetKey(typeof(TW), typeof(T));

        if (!MultiHandlersFunc.TryGetValue(key, out var handlers)) return;
        
        foreach (var handler in handlers.Cast<Func<TW, T, CancellationToken, Task>>())
        {
            await handler(eventData, eventData1, cts);
        }
    }

    public override async Task PublishAsync<TW, T>(List<Tuple<TW, T>> eventDataList, CancellationToken cts = default)
    {
        var key = GetKey(typeof(TW), typeof(T));
        
        if (!MultiHandlersFunc.TryGetValue(key, out var handlers)) return;

        foreach (var handler in handlers.Cast<Func<List<Tuple<TW, T>>,
                     CancellationToken, Task>>())
        {
            await handler(eventDataList, cts);
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

    public override void Unsubscribe<TW, T>(Action<List<TW>> handlers)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.TryGetValue(key, out var handlersThis)) return;

        lock (handlersThis)
        {
            handlersThis.Remove(handlers);
        }
    }

    public override void UnsubscribeFunc<TW, T>(Func<TW, T, CancellationToken, Task> funcHandler)
    {
        var key = GetKey(typeof(TW), typeof(T));

        if (!MultiHandlersFunc.TryGetValue(key, out var handlers)) return;

        lock (handlers)
        {
            handlers.Remove(funcHandler);
        }
    }
    
    public override void UnsubscribeListFunc<TW, T>(Func<List<Tuple<TW, T>>, CancellationToken, Task> funcHandler)
    {
        var key = GetKey(typeof(TW), typeof(T));

        if (!MultiHandlersFunc.TryGetValue(key, out var handlers)) return;

        lock (handlers)
        {
            handlers.Remove(funcHandler);
        }
    }

    public override void ClearSubscribers()
    {
        Handlers.Clear();
        MultiHandlers.Clear();
    }

    public override void ResetInstance()
    {
        _instance = new GlobalEventBus();
    }
}