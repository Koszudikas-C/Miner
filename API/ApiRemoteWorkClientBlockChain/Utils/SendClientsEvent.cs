using System.Collections.Concurrent;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using ApiRemoteWorkClientBlockChain.Utils.Interface;
using LibEntitiesRemote.Entities.Client;
using LibHandlerRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Utils;

public class SendClientsEvent : ISendClientsEvent
{
    private readonly IClient _clientRepository;
    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

    public SendClientsEvent(IClient client)
    {
        _clientRepository = client;
        GetClientsNotify();
    }

    private void GetClientsNotify()
    {
        var dict = new ConcurrentDictionary<string, Client>();
        var apiResponse = _clientRepository.GetAll(0,0, true);

        if (apiResponse.Success)
        {
            foreach (var data in apiResponse.Data!)
            {
                foreach (var client in data)
                {
                    dict[client.Ip] = client;
                }
            }
        }
        _globalEventBus.Publish(dict);
    }
}