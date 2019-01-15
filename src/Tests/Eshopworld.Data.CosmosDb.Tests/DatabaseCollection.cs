using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    /// <summary>
    /// Need the definition to wire the collection to the fixture
    /// </summary>
    [CollectionDefinition(nameof(DatabaseCollection))]
    public class DatabaseCollection : ICollectionFixture<CosmosDbFixture>
    {

    }
}
