using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Fleet.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Fleet.Infrastructure.Persistence;

public class FleetDbContext(DbContextOptions<FleetDbContext> options) : DbContext(options)
{
    public const string Schema = "fleet";

    public DbSet<Brand> Brands { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Car> Cars { get; set; }
    public DbSet<CarImage> CarImages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.UseTimestampWithoutTimeZone();
    }
}
