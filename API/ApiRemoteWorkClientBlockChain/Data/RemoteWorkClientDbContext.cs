using System.Reflection;
using ApiRemoteWorkClientBlockChain.Entities;
using LibEntitiesRemote.Entities.Client;
using LibRemoteAndClient.Entities.Remote.Client;
using Microsoft.EntityFrameworkCore;

namespace ApiRemoteWorkClientBlockChain.Data;

public class RemoteWorkClientDbContext(DbContextOptions<RemoteWorkClientDbContext> options) : DbContext(options)
{
    public DbSet<GuidTokenAuth> GuidTokenAuths { get; set; }
    public DbSet<Client> Client { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GuidTokenAuth>(entity =>
        {
            entity.ToTable("nonce_used_auth");
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.ToTable("client_primary_contact_server");
            entity.HasIndex(e => e.Id);
            entity.HasIndex(i => i.Ip);
        });
    }
}