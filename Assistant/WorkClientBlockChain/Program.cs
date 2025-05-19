using LibClassGetProcessInfo.Interface;
using LibClassManagerOptions.Interface;
using LibClassProcessOperations.Interface;
using LibCryptography.Interface;
using LibCryptography.Service;
using LibDownload.Interface;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibMapperObj.Service;
using LibProcess.Interface;
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
    .AddSingleton<IPosAuth, PosAuthService>()
    .AddSingleton<ISaveFile, SaveFileService>()
    .AddSingleton<ISearchFile, SearchFileService>()
    .AddSingleton<IMapperObj, MapperObjService>()
    .AddSingleton<IDownload, DownloadService>()
    .AddSingleton(typeof(IManagerOptions<>), typeof(ManagerOptionsService<>))
    .AddSingleton<IProcessOptionsClient, AuthSocks5OptionsService>()
    .AddSingleton<IGetProcessInfo, GetProcessInfoService>()
    .AddSingleton<IProcessKill, ProcessKillService>()
    .AddSingleton<IClientConnected, ClientConnected>();

var host = builder.Build();
host.Run();
