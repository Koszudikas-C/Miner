using System.Reflection;
using LibRemoteAndClient.Entities.Remote.Client;
using Microsoft.EntityFrameworkCore;

namespace ApiRemoteWorkClientBlockChain.Data;

public class RemoteWorkClientDbContext(DbContextOptions<RemoteWorkClientDbContext> options) : DbContext(options)
{
    public DbSet<GuidTokenAuth> GuidTokenAuths { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GuidTokenAuth>(entity =>
        {
            entity.ToTable("nonce_used_auth");
            entity.HasKey(e => e.Id);
        });
    }
}