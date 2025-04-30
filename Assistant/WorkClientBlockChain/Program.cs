using LibCryptography.Interface;
using LibCryptography.Service;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibMapperObj.Service;
using LibReceive.Interface;
using LibReceive.Service;
using LibSaveFile.Service;
using LibSearchFile.Service;
using LibSend.Interface;
using LibSend.Service;
using LibSocket.Service;
using LibSocketAndSslStream.Interface;
using LibSocks5.Interface;
using LibSocks5.Service;
using LibSsl.Service;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Middleware;
using WorkClientBlockChain.Middleware.Interface;
using WorkClientBlockChain.Service;
using WorkClientBlockChain.Utils;
using WorkClientBlockChain.Utils.Interface;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WorkService>();

builder.Services.AddSingleton<IConnectionAndAuth, ConnectionAndAuth>()
    .AddSingleton(typeof(ISend<>), typeof(SendServiceClient<>))
    .AddSingleton<IReceive, ReceiveServiceClient>()
    .AddSingleton<ISocketMiring, SocketClientService>()
    .AddSingleton<IAuthSsl, AuthSslClientService>()
    .AddSingleton<IAuthClient, AuthClientService>()
    .AddSingleton<IConnectionMiddleware, ConnectionMiddleware>()
    .AddSingleton<ICryptographFile, CryptographFileService>()
    .AddSingleton<IListenerClient, ListenerClientService>()
    .AddSingleton<IConfigVariable, ConfigVariableService>()
    .AddSingleton<ISocks5Options, Socks5OptionsService>()
    .AddSingleton<ISocks5, Socks5Service>()
    .AddSingleton<IPosAuth, PosAuth>()
    .AddSingleton<ISaveFile, SaveFileService>()
    .AddSingleton<ISearchFile, SearchFileService>()
    .AddSingleton<IMapperObj, MapperObjService>()
    .AddSingleton<IPosAuth, PosAuth>()
    .AddSingleton<IClientContext, ClientContext>();

var host = builder.Build();
host.Run();
