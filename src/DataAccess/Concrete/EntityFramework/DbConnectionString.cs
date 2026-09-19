using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DataAccess.Concrete.EntityFramework
{
    public static class DbConnectionString
    {
        // Matches the defaults in docker-compose.yml, so migrations run without extra configuration.
        private const string Default = "Host=localhost;Port=5432;Database=rentar;Username=rentar;Password=rentar";

        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static string Resolve()
        {
            return Configuration.GetConnectionString("RentarDb") ?? Default;
        }
    }
}
