using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Rentar.Shared.Infrastructure.Persistence;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Npgsql maps DateTime to "timestamp with time zone", which rejects values whose Kind is not Utc.
    /// The entities hold local wall-clock dates, so they are stored without a time zone.
    /// </summary>
    public static ModelBuilder UseTimestampWithoutTimeZone(this ModelBuilder modelBuilder)
    {
        var dateProperties = modelBuilder.Model.GetEntityTypes()
            .SelectMany(entityType => entityType.GetProperties())
            .Where(property => property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?));

        foreach (var property in dateProperties)
        {
            property.SetColumnType("timestamp without time zone");
        }

        return modelBuilder;
    }
}
