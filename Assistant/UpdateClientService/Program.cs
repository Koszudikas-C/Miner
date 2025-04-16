using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSend.Entities;
using LibSend.Interface;
using LibSend.Service;
using LibSocket.Interface;
using LibSocket.Service;
using LibSsl.Interface;
using LibSsl.Service;
using UpdateClientService;
using UpdateClientService.Connection;
using UpdateClientService.Interface;
using UpdateClientService.Service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton<IConnectionAndAuth, ConnectionAndAuth>()
    .AddSingleton(typeof(ISend<>), typeof(SendService<>))
    .AddSingleton<ISocketMiring, SocketMiringService>()
    .AddSingleton<IAuthSsl, AuthSslService>()
    .AddSingleton(ClientContext.Instance);

var host = builder.Build();
host.Run();
