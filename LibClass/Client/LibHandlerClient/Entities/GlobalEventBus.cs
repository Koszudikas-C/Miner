using LibHandlerClient.Interface;

namespace LibHandlerClient.Entities;

public class GlobalEventBus : GlobalEventBusBase<GlobalEventBus>, IEventBus
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

    public override void SubscribeAsync<TW>(Func<TW, CancellationToken, Task> handlerAsync)
    {
        var type = typeof(TW);
        if (!HandlersAsync.ContainsKey(type))
            HandlersAsync[type] = [];

        lock (HandlersAsync[type])
        {
            HandlersAsync[type].Add(handlerAsync);
            QueueHandlers.Enqueue(HandlersAsync);
            Console.WriteLine($"Total Signature of Functions {QueueHandlers.Count}");
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

    public override void SubscribeListAsync<TW>(Func<List<TW>, CancellationToken, Task> handlerListAsync)
    {
        var type = typeof(List<TW>);
        if (!HandlersAsync.ContainsKey(type))
            Handlers[type] = [];

        lock (HandlersAsync[type])
        {
            HandlersAsync[type].Add(handlerListAsync);
        }
    }

    public override void Publish<TW>(TW eventData)
    {
        var type = typeof(TW);
        if (!Handlers.TryGetValue(type, out var handlers)) return;

        lock (Handlers[type])
        {
            foreach (var handler in handlers.ToList())
                ((Action<TW>)handler)(eventData);
        }
    }

    public override void Publish<TW, T>(TW data1, T data2)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.TryGetValue(key, out var handlers)) return;

        lock (MultiHandlers[key])
        {
            foreach (var handler in handlers.ToList())
                ((Action<TW, T>)handler)(data1, data2);
        }
    }

    public override async Task PublishAsync<TW>(TW eventDataAsync, CancellationToken cts = default)
    {
        try
        {
            await SemaphoreSlim.WaitAsync(cts);
            var type = typeof(TW);
            if (!HandlersAsync.TryGetValue(type, out var handlers)) return;

            foreach (var handler in handlers.Cast<Func<TW, CancellationToken, Task>>())
            {
                await handler(eventDataAsync, cts);
            }
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    public override void PublishList<TW>(List<TW> eventData)
    {
        var type = typeof(List<TW>);
        if (!Handlers.TryGetValue(type, out var handlers)) return;

        lock (Handlers[type])
        {
            foreach (var handler in handlers.ToList())
                ((Action<List<TW>>)handler)(eventData);
        }
    }

    public override async Task PublishListAsync<TW>(List<TW> eventData, CancellationToken cts = default)
    {
        try
        {
            await SemaphoreSlim.WaitAsync(cts);
            var type = typeof(List<TW>);
            if (!Handlers.TryGetValue(type, out var handlers)) return;

            foreach (var handler in handlers.ToList())
                await ((Func<List<TW>, CancellationToken, Task>)handler)(eventData, cts);
        }
        finally
        {
            SemaphoreSlim.Release();
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

    public override void Unsubscribe<TW, T>(Action<TW, T> handler)
    {
        var key = GetKey(typeof(TW), typeof(T));
        if (!MultiHandlers.TryGetValue(key, out var handlers)) return;

        lock (handlers)
        {
            handlers.Remove(handler);
        }
    }

    public override void UnsubscribeAsync<TW>(Func<TW, CancellationToken, Task> handlerAsync)
    {
        var type = typeof(TW);
        if (!Handlers.TryGetValue(type, out var handlers)) return;

        lock (handlers)
        {
            handlers.Remove(handlerAsync);
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
        _instance = new GlobalEventBus();
    }
}