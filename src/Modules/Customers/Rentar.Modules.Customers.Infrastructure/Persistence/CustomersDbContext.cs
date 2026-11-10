using Microsoft.EntityFrameworkCore;
using Rentar.Modules.Customers.Domain;
using Rentar.Shared.Infrastructure.Persistence;

namespace Rentar.Modules.Customers.Infrastructure.Persistence;

public class CustomersDbContext(DbContextOptions<CustomersDbContext> options) : DbContext(options)
{
    public const string Schema = "customers";

    public DbSet<Customer> Customers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.UseTimestampWithoutTimeZone();
    }
}
