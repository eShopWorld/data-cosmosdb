using System;
using Microsoft.Azure.Documents.Client;

namespace Eshopworld.Data.CosmosDb.Tests
{
    /// <summary>
    /// Using a XUnit Fixture allows for a one time create and dispose of resources for the integration test when a class is annotated with an appropriate collection definition
    /// </summary>
    public class CosmosDbFixture : IDisposable
    {
        //todo these should come from a keyvault so they can be tested locally and remotely.
        private readonly string _endPoint = Environment.GetEnvironmentVariable("endPoint");
        private readonly string _authKey = Environment.GetEnvironmentVariable("authKey");
        private readonly string _databaseId = Environment.GetEnvironmentVariable("databaseId");
        private readonly string _collectionId = Environment.GetEnvironmentVariable("collectionId");

        public CosmosDbFixture()
        {
            DocumentDbRepository = new DocumentDBRepository<Dummy>();
            DocumentDbRepository.Initialize(_endPoint, _authKey, _databaseId, _collectionId);
        }

        internal readonly DocumentDBRepository<Dummy> DocumentDbRepository;

        public void Dispose()
        {
            DocumentDbRepository.Client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId)).Wait();
            DocumentDbRepository.Client?.Dispose();
        }
    }
}
