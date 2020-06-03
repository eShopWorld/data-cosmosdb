using Microsoft.Azure.Cosmos;

namespace Eshopworld.Data.CosmosDb
{
    public interface ICosmosDbClientFactory
    {
        /// <summary>
        /// Connects to the database, creates the db and requested collections (if not created yet)
        /// and caches the client internally.
        /// When calling for already cached client, the settings has no effect and previously cached version is returned.
        /// </summary>
        /// <param name="config">Database and collection settings</param>
        /// <returns>Instance of a db client</returns>
        CosmosClient InitialiseClient(CosmosDbConfiguration config);

        /// <summary>
        /// If any client is cached, this will invalidate (dispose) the client connection and subsequent call to InitialiseClient
        /// will perform full re-connect operation
        /// </summary>
        void Invalidate();
    }
}