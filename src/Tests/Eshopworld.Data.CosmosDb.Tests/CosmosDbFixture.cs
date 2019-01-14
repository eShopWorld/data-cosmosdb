namespace Eshopworld.Data.CosmosDb.Tests
{
    using System;
    using Microsoft.Azure.Documents.Client;

    /// <summary>
    /// Using a XUnit Fixture allows for a one time create and dispose of resources for the integration test when a class is annotated with an appropriate collection definition
    /// </summary>
    public class CosmosDbFixture : IDisposable
    {
        //todo these should come from a keyvault so they can be tested locally and remotely.
        private readonly string endPoint = Environment.GetEnvironmentVariable("endPoint");
        private readonly string authKey = Environment.GetEnvironmentVariable("authKey");
        private readonly string databaseId = Environment.GetEnvironmentVariable("databaseId");
        private readonly string collectionId = Environment.GetEnvironmentVariable("collectionId");

        public CosmosDbFixture()
        {
            DocumentDbRepository = new DocumentDBRepository<Dummy>();
            DocumentDbRepository.Initialize(endPoint, authKey, databaseId, collectionId);
        }

        internal readonly DocumentDBRepository<Dummy> DocumentDbRepository;

        public void Dispose()
        {
            DocumentDbRepository.Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId)).Wait();
            DocumentDbRepository.Client?.Dispose();
        }
    }
}
