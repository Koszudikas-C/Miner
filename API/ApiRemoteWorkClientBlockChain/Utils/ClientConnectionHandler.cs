using System.Collections.Concurrent;
using System.Net;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using ApiRemoteWorkClientBlockChain.Utils.Interface;
using LibEntitiesRemote.Entities.Client;
using LibHandlerRemote.Entities;
using LibSocketAndSslStreamRemote.Entities;

namespace ApiRemoteWorkClientBlockChain.Utils
{
    public class ClientConnectionHandler : IClientConnectionHandler
    {
        private readonly IClient _clientRepository;
        private readonly ILogger<ClientConnectionHandler> _logger;
        private readonly ConcurrentQueue<Client> _clientQueueRecent = [];
        private readonly ConcurrentDictionary<string, Client> _listClientBd = [];
        private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

        public ClientConnectionHandler(IClient client, ILogger<ClientConnectionHandler> logger)
        {
            _clientRepository = client;
            _logger = logger;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            _globalEventBus.Subscribe<SocketsConnectedEvent, CancellationToken>((handler, cts) =>
                OnReceiveSocketConnectedEvent(handler, cts).GetAwaiter());
        }

        public async Task OnReceiveSocketConnectedEvent(SocketsConnectedEvent socketsConnectedEvent,
            CancellationToken cts = default)
        {
            var client = CreateClient(socketsConnectedEvent);
            AddToQueue(client);
            UpdateOrInsertClientFromConsultation(client);
            
            if (_clientQueueRecent.Count < 2) return;
            
            await PersistClientAsync(socketsConnectedEvent.Registered, cts);
        }

        private static Client CreateClient(SocketsConnectedEvent evt)
        {
            var ip = ((IPEndPoint)evt.SocketClient.RemoteEndPoint!).Address.ToString();
            var ipLocal = ((IPEndPoint)evt.SocketClient.LocalEndPoint!).Address.ToString();
            var port = ((IPEndPoint)evt.SocketClient.RemoteEndPoint!).Port.ToString();

            return new Client(ip, ipLocal, port)
            {
                AttemptsConnection = evt.Attempts,
                StateClient = evt.ClientState,
                TimeoutReceive = evt.SocketClient.ReceiveTimeout,
                TimeoutSend = evt.SocketClient.SendTimeout
            };
        }

        private void AddToQueue(Client client) => _clientQueueRecent.Enqueue(client);

        private void UpdateOrInsertClientFromConsultation(Client client)
        {
            if (_listClientBd.TryGetValue(client.Ip, out var oldClient))
            {
                if (!_listClientBd.TryUpdate(client.Ip, client, oldClient))
                {
                    _logger.LogWarning("Failed to update the client with IP {Ip}", client.Ip);
                }

                return;
            }

            _listClientBd.TryAdd(client.Ip, client);
        }

        private async Task PersistClientAsync(bool registered, CancellationToken cts)
        {
            try
            {
                var cacheClient = new List<Client>();

                while (cacheClient.Count < 2 && _clientQueueRecent.TryDequeue(out var client))
                {
                    cacheClient.Add(client);
                }

                if (cacheClient.Count == 0)
                    return;

                if (registered)
                {
                    await _clientRepository.UpdateListAsync(cacheClient, cts);
                    return;
                }

                await _clientRepository.AddListAsync(cacheClient, cts);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while persisting clients.");
                throw new Exception(e.Message);
            }
        }
    }
}