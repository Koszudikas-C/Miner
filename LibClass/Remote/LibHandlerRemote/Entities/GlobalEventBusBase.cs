using System.Collections.Concurrent;
using System.Reflection.Metadata;

namespace LibHandlerRemote.Entities;

public abstract class GlobalEventBusBase<T> where T : class
{
    protected static T? _instance;

    public static T Instance
    {
        get
        {
            _instance ??= Activator.CreateInstance(typeof(T), true) as T;
            return _instance!;
        }
    }
    
    protected readonly ConcurrentDictionary<Type, List<Delegate>> Handlers = [];
    protected readonly ConcurrentDictionary<Type, List<Delegate>> HandlersFunc = [];
    protected readonly ConcurrentDictionary<string, List<Delegate>> MultiHandlers = [];
    protected readonly ConcurrentDictionary<string, List<Delegate>> MultiHandlersFunc = [];
    
    protected static string GetKey(Type t1, Type t2) => $"{t1.FullName}_{t2.FullName}";
    
    public abstract void Subscribe<TW>(Action<TW> handler);
    public abstract void Subscribe<TW>(Action<List<TW>> handlers);
    public abstract void SubscribeFunc<TW>(Func<TW, CancellationToken, Task> funcHandler);
    public abstract void SubscribeListFunc<TW>(Func<List<TW>, CancellationToken , Task> handlers);
    
    public abstract void Publish<TW>(TW eventData);
    public abstract void Publish<TW>(List<TW> handlers);
    public abstract Task PublishAsync<TW>(TW eventData, CancellationToken cts = default);
    public abstract Task PublishAsync<TW>(List<TW> eventData, CancellationToken cts = default);
    
    public abstract void Unsubscribe<TW>(Action<TW> handler);
    public abstract void Unsubscribe<TW>(Action<List<TW>> handlers);
    public abstract void UnsubscribeFunc<TW>(Action<TW> handler);
    public abstract void UnsubscribeListFunc<TW>(Action<List<TW>> handlers);

    // Methods with two generic types
    public abstract void Subscribe<TW, T>(Action<Tuple<TW, T>> handler);
    public abstract void Subscribe<TW, T>(Action<List<Tuple<TW, T>>> handlers);
    public abstract void SubscribeFunc<TW, T>(Func<TW, T, CancellationToken, Task> handler);
    public abstract void SubscribeListFunc<TW, T>(Func<List<Tuple<TW, T>>, CancellationToken, Task> handlers);
    
    public abstract void Publish<TW, T>(TW eventData, T eventData1);
    public abstract void Publish<TW, T>(List<Tuple<TW, T>> handlers);
    public abstract Task PublishAsync<TW, T>(TW eventData, T eventData1, CancellationToken cts = default);
    public abstract Task PublishAsync<TW, T>(List<Tuple<TW, T>> eventDataList, CancellationToken cts = default);
    
    public abstract void Unsubscribe<TW, T>(Action<TW, T> handler);
    public abstract void Unsubscribe<TW, T>(Action<List<TW>> handlers);
    public abstract void UnsubscribeFunc<TW, T>(Func<TW, T, CancellationToken, Task> funcHandler);
    public abstract void UnsubscribeListFunc<TW, T>(Func<List<Tuple<TW, T>>, CancellationToken, Task> funcHandler);


    protected Type CheckBeforeHandler<TW>()
    {
        var type = typeof(TW);

        if (!Handlers.ContainsKey(type))
            Handlers[type] = [];

        return type;
    }
    
    protected Type CheckBeforeHandlerFunc<TW>()
    {
        var type = typeof(TW);

        if (!HandlersFunc.ContainsKey(type))
            HandlersFunc[type] = [];

        return type;
    }
    
    public abstract void ClearSubscribers();
    public abstract void ResetInstance();
}
