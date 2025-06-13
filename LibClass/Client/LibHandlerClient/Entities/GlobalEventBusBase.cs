using System.Collections.Concurrent;

namespace LibHandlerClient.Entities;

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
    
    protected readonly ConcurrentDictionary<Type, List<Delegate>> Handlers = new();
    protected readonly ConcurrentDictionary<Type, List<Delegate>> HandlersAsync = new();
    protected readonly ConcurrentDictionary<string, List<Delegate>> MultiHandlers = new();
    
    public abstract void Subscribe<TW>(Action<TW> handler);
    public abstract void SubscribeAsync<TW>(Func<TW, CancellationToken, Task> handlerAsync);
    public abstract void SubscribeList<TW>(Action<List<TW>> handlers);
    public abstract void SubscribeListAsync<TW>(Func<List<TW>, CancellationToken, Task> handlerListAsync);
    
    public abstract void Publish<TW>(TW eventData);
    public abstract Task PublishAsync<TW>(TW eventDataAsync, CancellationToken cts = default);
    public abstract void PublishList<TW>(List<TW> eventListData);
    public abstract Task PublishListAsync<TW>(List<TW> eventListDataAsync, CancellationToken cts = default);

    public abstract void Unsubscribe<TW>(Action<TW> handler);
    public abstract void UnsubscribeList<TW>(Action<List<TW>> handlers);
    public abstract void UnsubscribeAsync<TW>(Func<TW, CancellationToken, Task> handlerAsync);

    // Métodos com dois tipos genéricos
    public abstract void Subscribe<TW, T>(Action<TW, T> handler);
    public abstract void Publish<TW, T>(TW eventData1, T eventData2);
    public abstract void Unsubscribe<TW, T>(Action<TW, T> handler);

    public abstract void ClearSubscribers();
    public abstract void ResetInstance();
}
