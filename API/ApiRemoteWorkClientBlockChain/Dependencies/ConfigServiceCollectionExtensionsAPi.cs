using System.Net;
using ApiRemoteWorkClientBlockChain.Data;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Interface.Repository;
using ApiRemoteWorkClientBlockChain.Repository;
using ApiRemoteWorkClientBlockChain.Service;
using ApiRemoteWorkClientBlockChain.Service.LibClass;
using ApiRemoteWorkClientBlockChain.Service.ProcessOptions;
using LibAuthSecurityConnectionRemote.Interface;
using LibCertificateRemote.Interface;
using LibCertificateRemote.Service;
using LibCryptographyRemote.Interface;
using LibCryptographyRemote.Service;
using LibDirectoryFileRemote.Interface;
using LibHandlerRemote.Entities;
using LibManagerFileRemote.Interface;
using LibMapperObjRemote.Interface;
using LibMapperObjRemote.Service;
using LibPreparationFileRemote.Interface;
using LibReceiveRemote.Interface;
using LibReceiveRemote.Service;
using LibSaveFileRemote.Service;
using LibSearchFileRemote.Service;
using LibSendRemote.Interface;
using LibSendRemote.Service;
using LibSocketAndSslStreamRemote.Interface;
using LibSocketRemote.Service;
using LibSslRemote.Service;
using LibUploadRemote.Interface;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;

namespace ApiRemoteWorkClientBlockChain.Dependencies;

public static class ConfigServiceCollectionExtensionsAPi
{
    private static readonly ILoggerFactory LoggerFactory = new LoggerFactory();

    private static readonly ILogger<object> _logger =
        new Logger<object>(LoggerFactory);

    public static IServiceCollection AddConfigServiceCollection(this IServiceCollection services,
        IConfiguration configuration)
    {
        
        //DataBaseConnection
        services.AddDbContext<RemoteWorkClientDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<ISocketMiring, SocketService>()
            .AddScoped<IAuthSsl, AuthSslService>()
            .AddScoped<IAuth, AuthService>()
            .AddScoped(typeof(ISend<>), typeof(SendService<>))
            .AddScoped<IReceive, ReceiveServiceRemote>()
            .AddScoped<IManagerConnection, ManagerConnectionService>()
            .AddScoped<IManagerClient, ManagerClientService>()
            .AddSingleton<IClientConnected>(ClientConnected.Instance)
            .AddScoped<IProcessOptions, AuthSocks5OptionsService>()
            .AddScoped<IListener, ListenerService>()
            .AddScoped<ICryptographFile, CryptographFileService>()
            .AddScoped<ISearchFile, SearchFileService>()
            .AddScoped<ISaveFile, SaveFileService>()
            .AddScoped<ICertificate, CertificateService>()
            .AddScoped<IOperationFactory, OperationFactory>()
            .AddScoped<IMapperObj, MapperObjService>()
            .AddScoped<IUpload, UploadService>()
            .AddScoped(typeof(IUploadSend<,>), typeof(UploadSendService<,>))
            .AddScoped<IDirectoryFile, DirectoryFileService>()
            .AddScoped<IManagerUpload, ManagerUploadService>()
            .AddScoped<IPreparationFile, PreparationFileService>()
            .AddScoped<IPosAuth, PosAuthService>()
            .AddScoped(typeof(IManagerOptions<>), typeof(ManagerOptionsAutomaticService<>))
            .AddScoped<IAuthConnection, AuthConnectionService>()
            .AddSingleton<GlobalEventBus>()
            .AddSingleton(ClientConnected.Instance)
            
            //Repositories
            .AddScoped(typeof(IRepositoryBase<,>), typeof(BaseRepository<,>))
            .AddScoped<INonceToken, NonceTokenRepository>()
            .AddScoped<IClientNotAuthorized, ClientNotAuthorizedRepository>();

        InitializeConstructorOrService(services);

        //Version of endpoints
        services.AddControllers();
        services.AddProblemDetails();
        services.AddApiVersioning();

        //Debug Headers
        services.AddHttpLogging(options => { options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders; });

        //Middleware Headers
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
        });
        
        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static void InitializeConstructorOrService(IServiceCollection service)
    {
        service.BuildServiceProvider().GetRequiredService<IManagerUpload>();
        service.BuildServiceProvider().GetRequiredService<IAuthConnection>();
    }
}
