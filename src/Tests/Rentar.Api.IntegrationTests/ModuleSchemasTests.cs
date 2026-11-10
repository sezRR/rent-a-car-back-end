using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Rentar.Api.IntegrationTests;

public class ModuleSchemasTests(RentarApiFactory factory) : IntegrationTestBase(factory)
{
    [Theory]
    [InlineData("identity", new[] { "OperationClaims", "UserOperationClaims", "Users", "__EFMigrationsHistory" })]
    [InlineData("fleet", new[] { "Brands", "CarImages", "Cars", "Colors", "__EFMigrationsHistory" })]
    [InlineData("customers", new[] { "Customers", "__EFMigrationsHistory" })]
    [InlineData("rentals", new[] { "Rentals", "__EFMigrationsHistory" })]
    public async Task EachModule_KeepsItsTablesAndMigrationHistoryInItsOwnSchema(string schema, string[] expectedTables)
    {
        var tables = await TablesInSchemaAsync(schema);

        Assert.Equal(expectedTables, tables);
    }

    [Fact]
    public async Task NoModule_CreatesTablesInThePublicSchema()
    {
        Assert.Empty(await TablesInSchemaAsync("public"));
    }

    private async Task<List<string>> TablesInSchemaAsync(string schema)
    {
        await using var connection = new NpgsqlConnection(Factory.ConnectionString);
        await connection.OpenAsync(TestContext.Current.CancellationToken);

        await using var command = new NpgsqlCommand(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = @schema ORDER BY table_name COLLATE \"C\"",
            connection);
        command.Parameters.AddWithValue("schema", schema);

        var tables = new List<string>();
        await using var reader = await command.ExecuteReaderAsync(TestContext.Current.CancellationToken);
        while (await reader.ReadAsync(TestContext.Current.CancellationToken))
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }
}
