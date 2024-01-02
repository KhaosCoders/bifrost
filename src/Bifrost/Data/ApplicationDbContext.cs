using Bifrost.Features.Identity.Model;
using Bifrost.Features.Portals.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Data;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<PortalDefinition> Portals { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PortalDefinition>(b =>
        {
            b.HasKey(r => r.Id);
            b.HasIndex(r => r.Name).HasDatabaseName("PortalNameIndex").IsUnique();
            b.ToTable("PortalDefinitions");
            b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

            b.Property(u => u.Name).HasMaxLength(256);

            b.HasMany(p => p.Instances)
                .WithOne(p => p.Portal)
                .HasForeignKey(p => p.PortalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PortalInstance>(b =>
        {
            b.HasKey(r => r.Id);
            b.ToTable("PortalInstances");
            b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

            b.HasMany(p => p.Mappings)
                .WithOne()
                .HasForeignKey(p => p.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(p => p.History)
                .WithOne(p => p.Instance)
                .HasForeignKey(p => p.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<PortalHistory>(b =>
        {
            b.HasKey(r => r.Id);
            b.ToTable("PortalHistory");
        });

        builder.Entity<PortMapping>(b =>
        {
            b.HasKey(r => new { r.InstanceId, r.MappedPort });
            b.ToTable("PortMappings");
        });
    }
}
