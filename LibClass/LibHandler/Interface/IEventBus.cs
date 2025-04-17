namespace LibHandler.Interface;

public interface IEventBus
{
    void Subscribe<TW>(Action<TW> handler);
    void SubscribeList<TW>(Action<List<TW>> handlers);
    
    void Publish<TW>(TW eventData);
    void PublishList<TW>(List<TW> handlers);

    void Unsubscribe<TW>(Action<TW> handler);
    void UnsubscribeList<TW>(Action<List<TW>> handlers);
    
    void ClearSubscribers();
    
    void ResetInstance();
}