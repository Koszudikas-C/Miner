namespace LibHandler.Interface;

public interface IEventBus
{
    void Subscribe<TW>(Action<TW> handler);
    void SubscribeList<TW>(Action<List<TW>> handlers);
    void Publish<TW>(TW eventData);
    void PublishList<TW>(List<TW> handlers);
    void Unsubscribe<TW>(Action<TW> handler);
    void UnsubscribeList<TW>(Action<List<TW>> handlers);
    
    void Subscribe<TW, T>(Action<TW, T> handler);
    void Publish<TW, T>(TW eventData1, T eventData2);
    void Unsubscribe<TW, T>(Action<TW, T> handler);

    void ClearSubscribers();
    void ResetInstance();
}