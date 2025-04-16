using LibHandler.EventBus;
using LibLayout.Service;
using LibLayout.Interface;
using LibReceive.Interface;
using LibReceive.Service;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSend.Interface;
using LibSend.Service;
using LibSocket.Interface;
using LibSocket.Service;
using LibSsl.Interface;
using LibSsl.Service;
using Microsoft.Extensions.DependencyInjection;
using UpdateRemoteService.Entities;
using UpdateRemoteService.Interface;
using UpdateRemoteService.Service;

namespace UpdateRemoteService;

internal static class Program
{
    private static void Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddSingleton<IMenu, MenuService>()
            .AddSingleton<ILayoutInteraction, LayoutInteractionService>()
            .AddSingleton<ISocketMiring, SocketMiringService>()
            .AddSingleton<IAuthSsl, AuthSslService>()
            .AddSingleton(typeof(ISend<>), typeof(SendService<>))
            .AddSingleton<IReceive, ReceiveService>()
            .AddSingleton<IManagerConnection, ManagerConnectionService>()
            .AddSingleton<GlobalEventBusRemote>()
            .AddSingleton(ClientConnected.Instance)
            .BuildServiceProvider();
        
       _= ManagerServiceCollection(serviceProvider);
        Console.ReadLine();
    }
    
    private static async Task ManagerServiceCollection(ServiceProvider serviceProvider)
    {
        serviceProvider.GetRequiredService<IAuthSsl>();
        var managerConnectionService = serviceProvider.GetRequiredService<IManagerConnection>();
        await managerConnectionService!.InitializeAsync();
        
    }
}