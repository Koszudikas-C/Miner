using LibSocketAndSslStreamClient.Interface;

namespace LibSocketAndSslStreamClient.Entities;

public class ListenerWrapper: IListenerWrapper
{
    public Listener Listener { get; } = new Listener();
}