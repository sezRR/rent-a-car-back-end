using Core.Entities.Concrete;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace DataAccess.Concrete.EntityFramework
{
    public class RentingCarContext : DbContext
    {
        public RentingCarContext()
        {
        }

        public RentingCarContext(DbContextOptions<RentingCarContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(DbConnectionString.Resolve());
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Npgsql maps DateTime to "timestamp with time zone", which rejects values whose Kind is not Utc.
            // The entities hold local wall-clock dates, so store them without a time zone.
            var dateProperties = modelBuilder.Model.GetEntityTypes()
                .SelectMany(entityType => entityType.GetProperties())
                .Where(property => property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?));

            foreach (var property in dateProperties)
            {
                property.SetColumnType("timestamp without time zone");
            }
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<CarImage> CarImages { get; set; }
        public DbSet<Core.Entities.Concrete.User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        public DbSet<OperationClaim> OperationClaims { get; set; }
    }
}
