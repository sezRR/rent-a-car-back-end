using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Identity.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Identity.Infrastructure.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
{
    public const string Schema = "identity";

    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.UseTimestampWithoutTimeZone();
    }
}
