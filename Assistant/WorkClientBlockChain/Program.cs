using LibHandler.EventBus;
using LibRemoteAndClient.Entities.Remote.Client;
using LibSend.Entities;
using LibSend.Interface;
using LibSend.Service;
using LibSocket.Interface;
using LibSocket.Service;
using LibSsl.Interface;
using LibSsl.Service;
using WorkClientBlockChain;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Service;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WorkService>();

builder.Services.AddSingleton<IConnectionAndAuth, ConnectionAndAuth>()
    .AddSingleton(typeof(ISend<>), typeof(SendService<>))
    .AddSingleton<ISocketMiring, SocketMiringService>()
    .AddSingleton<IAuthSsl, AuthSslService>()
    .AddSingleton(ClientContext.Instance);

var host = builder.Build();
host.Run();
