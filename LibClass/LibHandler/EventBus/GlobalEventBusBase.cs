using System.Collections.Concurrent;

namespace LibHandler.EventBus;

public abstract class GlobalEventBusBase<T>
{
    protected static T? _instance;

    public static T? Instance
    {
        get
        {
            _instance ??= Activator.CreateInstance<T>();
            return _instance!;
        }
    }
    
    protected ConcurrentDictionary<Type, List<object>> Handlers = new();
    
    public abstract void Subscribe<TW>(Action<TW> handler);
    public abstract void SubscribeList<TW>(Action<List<TW>> handlers);

    public abstract void Publish<TW>(TW eventData);
    public abstract void PublishList<TW>(List<TW> handlers);
    
    public abstract void Unsubscribe<TW>(Action<TW> handler);
    public abstract void UnsubscribeList<TW>(Action<List<TW>> handlers);

    public abstract void ResetInstance();
}