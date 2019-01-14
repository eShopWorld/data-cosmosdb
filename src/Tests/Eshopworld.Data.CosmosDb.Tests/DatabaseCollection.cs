namespace Eshopworld.Data.CosmosDb.Tests
{
    using Xunit;

    /// <summary>
    /// Need the definition to wire the collection to the fixture
    /// </summary>
    [CollectionDefinition("Database Collection")]
    public class DatabaseCollection : ICollectionFixture<CosmosDbFixture>
    {

    }
}
