using LibHandlerRemote.Entities;

namespace LibHandlerRemote.Interface;

public interface IEventBus
{
    void Subscribe<TW>(Action<TW> handler);
    void Subscribe<TW>(Action<List<TW>> handlers);
    void SubscribeFunc<TW>(Func<TW, CancellationToken, Task> funcHandler);
    void SubscribeListFunc<TW>(Func<List<TW>, CancellationToken, Task> handlers);

    void Publish<TW>(TW eventData);
    void Publish<TW>(List<TW> handlers);
    Task PublishAsync<TW>(TW eventData, CancellationToken cts = default);
    Task PublishAsync<TW>(List<TW> eventData, CancellationToken cts = default);

    void Unsubscribe<TW>(Action<TW> handler);
    void Unsubscribe<TW>(Action<List<TW>> handlers);
    void UnsubscribeFunc<TW>(Action<TW> handler);
    void UnsubscribeListFunc<TW>(Action<List<TW>> handlers);

    // Methods with two generic types
    void Subscribe<TW, T>(Action<Tuple<TW, T>> handler);
    void Subscribe<TW, T>(Action<List<Tuple<TW, T>>> handlers);
    void SubscribeFunc<TW, T>(Func<TW, T, CancellationToken, Task> handler);
    void SubscribeListFunc<TW, T>(Func<List<Tuple<TW, T>>, CancellationToken, Task> handler);

    void Publish<TW, T>(TW eventData, T eventData1);
    void Publish<TW, T>(List<Tuple<TW, T>> handlers);
    Task PublishAsync<TW, T>(TW eventData, T eventData1, CancellationToken cts = default);
    Task PublishAsync<TW, T>(List<Tuple<TW, T>> eventDataList, CancellationToken cts = default);

    void Unsubscribe<TW, T>(Action<TW, T> handler);
    void Unsubscribe<TW, T>(Action<List<TW>> handlers);
    void UnsubscribeFunc<TW, T>(Func<TW, T, CancellationToken, Task> funcHandler);
    void UnsubscribeListFunc<TW, T>(Func<List<Tuple<TW, T>>, CancellationToken, Task> funcHandler);

    void ClearSubscribers();
    void ResetInstance();
}