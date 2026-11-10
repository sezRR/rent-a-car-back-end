using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Rentals.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Rentals.Infrastructure.Persistence;

public class RentalsDbContext(DbContextOptions<RentalsDbContext> options) : DbContext(options)
{
    public const string Schema = "rentals";

    public DbSet<Rental> Rentals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.UseTimestampWithoutTimeZone();
    }
}
