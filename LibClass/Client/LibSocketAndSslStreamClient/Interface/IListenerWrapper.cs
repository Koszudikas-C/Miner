using LibSocketAndSslStreamClient.Entities;

namespace LibSocketAndSslStreamClient.Interface;

public interface IListenerWrapper
{
    Listener Listener { get; }
}