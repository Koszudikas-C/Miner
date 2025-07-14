using System.Collections.Concurrent;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using LibCommunicationStateRemote.Entities;
using LibEntitiesRemote.Entities.Client;
using LibEntitiesRemote.Entities.Client.Enum;
using LibEntitiesRemote.Interface;
using LibHandlerRemote.Entities;
using LibSocketAndSslStreamRemote.Entities;
using LibSocketAndSslStreamRemote.Interface;

namespace LibSocketRemote.Service;

public class ManagerSocketConnectedService : IManagerSocketConnected
{
    private readonly ConcurrentDictionary<string, Client> _clientRegistered = [];
    private readonly ConcurrentDictionary<string, Socket> _listSocketConnected = [];

    private readonly GlobalEventBus _globalEventBus = GlobalEventBus.Instance;

    public ManagerSocketConnectedService()
    {
        Console.WriteLine($"Construtor ManagerSocket. {_clientRegistered.Count}");
        SubscribeEventLoad();
    }

    private void SubscribeEventLoad()
    {
        _globalEventBus.Subscribe<ConcurrentDictionary<string, Client>>(OnAddClientRegistered);
    }

    public bool CheckStateSocket(ISocketWrapper socket)
    {
        if (!socket.Connected)
            return false;

        if (_clientRegistered.Count == 0)
            GetClientAll(1, 50).GetAwaiter().GetResult();

        var client = CheckRegisteredClient(socket);

        SubscribeEvent();

        if (client is null)
        {
            RegisterClient(socket);
            ApplyNewAttempt(socket.InnerSocket, 0, false);
            UnsubscribeEvent();

            return true;
        }

        Console.WriteLine("Cliente já cadastrado");
        ApplyNewAttempt(socket.InnerSocket, client.AttemptsConnection, true);
        UnsubscribeEvent();

        return true;
    }

    private Client CheckRegisteredClient(ISocketWrapper socket)
    {
        return _clientRegistered.TryGetValue(socket.RemoteEndPoint, out var client) ? client : null!;
    }

    private void RegisterClient(ISocketWrapper socket)
    {
        _listSocketConnected.TryAdd(socket.RemoteEndPoint, socket.InnerSocket);
    }

    private void ApplyNewAttempt(Socket socket, int attempt, bool registered)
    {
        SocketsConnectedEvent socketsConnectedEvent = new(socket,
            ConnectionStates.Attempt, attempt, registered);

        var ctsSource = new CancellationTokenSource();

        if (attempt >= 3)
        {
            socketsConnectedEvent.ClientState = ConnectionStates.Blocked;
            _globalEventBus.Publish(socketsConnectedEvent, ctsSource.Token);
            OnAddListSocketConnectedAct();
            return;
        }

        socketsConnectedEvent.ClientState = ConnectionStates.Attempt;
        OnAddListSocketConnectedAct();
        _globalEventBus.Publish(socketsConnectedEvent, ctsSource.Token);
    }


    private void OnAddListSocketConnectedAct() =>
        ListSocketConnectedAct?.Invoke(_listSocketConnected);


    private void OnAddClientRegistered(ConcurrentDictionary<string, Client> dict)
    {
        foreach (var client in dict)
        {
            _clientRegistered.TryAdd(client.Key, client.Value);
        }
    }

    private void SubscribeEvent()
    {
        _globalEventBus.Subscribe<Client>((handle)
            =>
        {
            _clientRegistered[handle.Ip] = handle;
        });
    }

    private async Task GetClientAll(int page, int pageSize)
    {
        while (true)
        {
            var apiResponse = await GetRequest(page, pageSize);
            if (apiResponse.Success)
            {
                var countClients = 0;
                
                foreach (var data in apiResponse.Data!)
                {
                    countClients = data.Count();
                    foreach (var client in data)
                    {
                        _clientRegistered.TryAdd(client.Ip, client);
                    }
                }
                Console.WriteLine(countClients);
                
                if (countClients < 50)
                    break;
                
                page++;
                pageSize = 50;
                continue;
            }

            break;
        }
    }

    private async Task<ApiResponse<IEnumerable<Client>>> GetRequest(int page, int pageSize)
    {
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "FrxYFCeT6DtP74YnwyvzC76cX8EdjuAOv5KHkjnEXZ0CMUmfmwx7LFPcjCdhWZQLj" +
            "bbsjNypODujOkKmUazFtEqcYAjsJZnv3Y9VWFpvogyncJKHiwNLhnS1gDNRvWEd");
        
        var apiResponse =
            await httpClient.GetFromJsonAsync<ApiResponse<IEnumerable<Client>>>(
                $"http://localhost:5050/api/v1/ManagerClient/GetAllClient?page={page}&pageSize={pageSize}");

        return apiResponse!;
    }

    private void UnsubscribeEvent() => _globalEventBus.UnsubscribeFunc<Client>((handle) =>
        _clientRegistered[handle.Ip] = handle);

    public event Action<ConcurrentDictionary<string, Socket>>? ListSocketConnectedAct;
    public event Action<ConcurrentDictionary<string, int>>? DictionaryClientConnectedAct;
}