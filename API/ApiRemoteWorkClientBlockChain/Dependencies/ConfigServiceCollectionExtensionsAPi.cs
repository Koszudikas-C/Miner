using System.Net;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Entities.Interface;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Service;
using LibCertificate.Interface;
using LibCertificate.Service;
using LibClassProcessOperations.Interface;
using LibCryptography.Interface;
using LibCryptography.Service;
using LibHandler.EventBus;
using LibManagerFile.Interface;
using LibMapperObj.Interface;
using LibMapperObj.Service;
using LibReceive.Service;
using LibReceive.Interface;
using LibSaveFile.Service;
using LibSearchFile.Service;
using LibSend.Service;
using LibSend.Interface;
using LibSocket.Service;
using LibSocketAndSslStream.Interface;
using LibSocks5.Interface;
using LibSocks5.Service;
using LibSsl.Service;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpLogging;

namespace ApiRemoteWorkClientBlockChain.Dependencies;

public static class ConfigServiceCollectionExtensionsAPi
{
    public static IServiceCollection AddConfigServiceCollection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISocketMiring, SocketRemoteService>()
            .AddScoped<IAuthSsl, AuthSslRemoteService>()
            .AddScoped<IAuthRemote, AuthRemoteService>()
            .AddScoped(typeof(ISend<>), typeof(SendServiceRemote<>))
            .AddScoped<IReceive, ReceiveServiceRemote>()
            .AddScoped<IManagerConnection, ManagerConnectionService>()
            .AddScoped<IManagerClient, ManagerClientService>()
            .AddSingleton<IClientConnected>(ClientConnected.Instance)
            .AddScoped<IProcessOptions, ProcessOptionsService>()
            .AddScoped<IListenerRemote, ListenerRemoteService>()
            .AddScoped<ICryptographFile, CryptographFileService>()
            .AddScoped<ISearchFile, SearchFileService>()
            .AddScoped<ISaveFile, SaveFileService>()
            .AddScoped<ICertificate, CertificateService>()
            .AddScoped<IMapperObj, MapperObjService>()
            .AddSingleton<GlobalEventBusRemote>()
            .AddSingleton(ClientConnected.Instance);
        
        //Version of endpoints
        services.AddControllers();
        services.AddProblemDetails();
        services.AddApiVersioning();
        
        //Debug Headers
        services.AddHttpLogging(options =>
        {
            options.LoggingFields = HttpLoggingFields.RequestPropertiesAndHeaders;
        });
        
        //Middleware Headers
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownProxies.Add(IPAddress.Parse("127.0.0.1"));
        });

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        // services.AddEndpointsApiExplorer();
        // services.AddSwaggerGen();

        return services;
    }
}