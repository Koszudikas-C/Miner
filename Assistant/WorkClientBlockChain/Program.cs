using LibSend.Interface;
using LibSend.Service;
using LibSocket.Service;
using LibSocketAndSslStream.Interface;
using LibSsl.Service;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Middleware;
using WorkClientBlockChain.Middleware.Interface;
using WorkClientBlockChain.Service;
using WorkClientBlockChain.Utils;
using WorkClientBlockChain.Utils.Interface;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WorkService>();

builder.Services.AddSingleton<IConnectionAndAuth, ConnectionAndAuth>()
    .AddSingleton(typeof(ISend<>), typeof(SendService<>))
    .AddSingleton<ISocketMiring, SocketMiringService>()
    .AddSingleton<IAuthSsl, AuthSslService>()
    .AddSingleton<IConnectionMiddleware, ConnectionMiddleware>()
    .AddSingleton<IPortOpen, PortOpen>()
    .AddSingleton<IConnectionValidation, ConnectionValidation>()
    .AddSingleton(ClientContext.Instance);

var host = builder.Build();
host.Run();
