using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Rentar.Modules.Fleet.Infrastructure.Persistence.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "fleet");

        migrationBuilder.CreateTable(
            name: "Brands",
            schema: "fleet",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                BrandName = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Brands", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CarImages",
            schema: "fleet",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                CarId = table.Column<int>(type: "integer", nullable: false),
                ImagePath = table.Column<string>(type: "text", nullable: true),
                Date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CarImages", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Cars",
            schema: "fleet",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                BrandId = table.Column<int>(type: "integer", nullable: false),
                ColorId = table.Column<int>(type: "integer", nullable: false),
                ModelYear = table.Column<int>(type: "integer", nullable: false),
                DailyPrice = table.Column<decimal>(type: "numeric", nullable: false),
                Description = table.Column<string>(type: "text", nullable: true),
                MinimumFindeksRating = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Cars", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Colors",
            schema: "fleet",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                ColorName = table.Column<string>(type: "text", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Colors", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Brands",
            schema: "fleet");

        migrationBuilder.DropTable(
            name: "CarImages",
            schema: "fleet");

        migrationBuilder.DropTable(
            name: "Cars",
            schema: "fleet");

        migrationBuilder.DropTable(
            name: "Colors",
            schema: "fleet");
    }
}
