using Xunit;

namespace Rentar.Api.IntegrationTests;

[CollectionDefinition(Name)]
public class ApiCollection : ICollectionFixture<RentarApiFactory>
{
    public const string Name = "api";
}
