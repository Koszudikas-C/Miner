using LibAuthSecurityConnectionClient.Interface;
using LibCryptographyClient.Interface;
using LibCryptographyClient.Service;
using LibDownloadClient.Interface;
using LibManagerFileClient.Interface;
using LibMapperObjClient.Interface;
using LibMapperObjClient.Service;
using LibProcessClient.Interface;
using LibReceiveClient.Interface;
using LibReceiveClient.Service;
using LibSaveFileClient.Service;
using LibSearchFileClient.Service;
using LibSendClient.Interface;
using LibSendClient.Service;
using LibSocketAndSslStreamClient.Interface;
using LibSocketClient.Service;
using LibSocks5Client.Interface;
using LibSocks5Client.Service;
using LibSslClient.Service;
using WorkClientBlockChain.Connection;
using WorkClientBlockChain.Connection.Interface;
using WorkClientBlockChain.Interface;
using WorkClientBlockChain.Middleware;
using WorkClientBlockChain.Middleware.Interface;
using WorkClientBlockChain.Service;
using WorkClientBlockChain.Service.LibClass;
using WorkClientBlockChain.Utils.Interface;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<WorkService>();

builder.Services.AddSingleton<IConnectionAndAuth, ConnectionAndAuth>()
    .AddSingleton(typeof(ISend<>), typeof(SendService<>))
    .AddSingleton<IReceive, ReceiveService>()
    .AddSingleton<ISocketMiring, SocketService>()
    .AddSingleton<IAuthSsl, AuthSslService>()
    .AddSingleton<IAuth, AuthService>()
    .AddSingleton<IConnectionMiddleware, ConnectionMiddleware>()
    .AddSingleton<ICryptographFile, CryptographFileService>()
    .AddSingleton<IListener, ListenerService>()
    .AddSingleton<IConfigVariable, ConfigVariableService>()
    .AddSingleton<ISocks5Options, Socks5OptionsService>()
    .AddSingleton<ISocks5, Socks5Service>()
    .AddSingleton<IPosAuth, PosAuthService>()
    .AddSingleton<ISaveFile, SaveFileService>()
    .AddSingleton<ISearchFile, SearchFileService>()
    .AddSingleton<IMapperObj, MapperObjService>()
    .AddSingleton<IDownload, DownloadService>()
    .AddSingleton(typeof(IManagerOptions<>), typeof(ManagerOptionsService<>))
    .AddSingleton<IProcessOptions, AuthSocks5OptionsService>()
    .AddSingleton<IGetProcessInfo, GetProcessInfoService>()
    .AddSingleton<IProcessKill, ProcessKillService>()
    .AddSingleton<IClientConnected, ClientConnected>()
    .AddSingleton<IAuthConnectionClient, AuthConnectionClientService>();

builder.Services.BuildServiceProvider().GetRequiredService<IClientConnected>();
builder.Services.BuildServiceProvider().GetRequiredService<IAuthSsl>();
builder.Services.BuildServiceProvider().GetRequiredService<IAuthConnectionClient>();

var host = builder.Build();
host.Run();
