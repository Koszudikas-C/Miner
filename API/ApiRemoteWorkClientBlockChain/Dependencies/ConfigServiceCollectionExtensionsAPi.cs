using System.Net;
using ApiRemoteWorkClientBlockChain.Entities;
using ApiRemoteWorkClientBlockChain.Interface;
using ApiRemoteWorkClientBlockChain.Service;
using LibHandler.EventBus;
using LibReceive.Service;
using LibReceive.Interface;
using LibSend.Service;
using LibSend.Interface;
using LibSocket.Connection;
using LibSocket.Interface;
using LibSocket.Service;
using LibSsl.Interface;
using LibSsl.Service;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpLogging;

namespace ApiRemoteWorkClientBlockChain.Dependencies;

public static class ConfigServiceCollectionExtensionsAPi
{
    public static IServiceCollection AddConfigServiceCollection(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped<ISocketMiring, SocketMiringService>()
            .AddScoped<IAuthSsl, AuthSslService>()
            .AddScoped(typeof(ISend<>), typeof(SendService<>))
            .AddScoped<IReceive, ReceiveService>()
            .AddScoped<IManagerConnection, ManagerConnectionService>()
            .AddScoped<GlobalEventBusRemote>()
            .AddSingleton(ClientConnected.Instance);


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
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        return services;
    }

    private static async Task<string> Host()
    {
        var addresses = await Dns.GetHostAddressesAsync(TargetHost.Host);

        return addresses[0].ToString();
    }
}