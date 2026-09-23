using Xunit;

namespace WebAPI.IntegrationTests;

[CollectionDefinition(Name)]
public class ApiCollection : ICollectionFixture<RentarApiFactory>
{
    public const string Name = "api";
}
