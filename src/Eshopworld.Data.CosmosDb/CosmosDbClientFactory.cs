using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Eshopworld.Data.CosmosDb
{
    public class CosmosDbClientFactory : ICosmosDbClientFactory, IDisposable
    {
        private readonly object _sync = new object();

        private CosmosClient _client;

        public CosmosClient InitialiseClient(CosmosDbConfiguration config, CosmosClientOptions clientOptions = null)
        {
            if (_client != null) return _client;

            ValidateConfiguration(config);

            lock (_sync)
            {
                if (_client != null) return _client;

                var client = new CosmosClient(config.DatabaseEndpoint, config.DatabaseKey, clientOptions);
                foreach (var (databaseName, containerSettings) in config.Databases)
                {
                    CreateDatabaseIfNotExistsAsync(config, client, databaseName, containerSettings).Wait();
                }

                _client = client;
            }

            return _client;
        }

        private static async Task CreateDatabaseIfNotExistsAsync(
            CosmosDbConfiguration config,
            CosmosClient client,
            string databaseName,
            IEnumerable<CosmosDbCollectionSettings> containerSettings)
        {
            IEnumerable<Task<ContainerResponse>> CreateContainersIfNotExistAsync(Microsoft.Azure.Cosmos.Database alreadyCreatedDatabase)
            {
                foreach (var container in containerSettings)
                {
                    var containerProperties = new ContainerProperties
                    {
                        Id = container.CollectionName,
                        PartitionKeyPath = container.PartitionKey,
                        UniqueKeyPolicy = GetUniqueKeyPolicy(container),
                        DefaultTimeToLive = config.DefaultTimeToLive
                    };
                    yield return alreadyCreatedDatabase.CreateContainerIfNotExistsAsync(containerProperties);
                }
            }

            var createDatabaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseName, config.Throughput);
            var database = createDatabaseResponse.Database;

            var createContainersTasks = CreateContainersIfNotExistAsync(database);
            await Task.WhenAll(createContainersTasks);
        }

        public void Invalidate()
        {
            lock (_sync)
            {
                _client?.Dispose();
                _client = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Invalidate();
            }
        }

        private void ValidateConfiguration(CosmosDbConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            if (string.IsNullOrWhiteSpace(config.DatabaseEndpoint)) throw new ArgumentException($"Cosmos DB database endpoint ({nameof(config.DatabaseEndpoint)}) is not defined");
            if (string.IsNullOrWhiteSpace(config.DatabaseKey)) throw new ArgumentException($"Cosmos DB database key ({nameof(config.DatabaseKey)}) is not defined");
            if (config.Databases == null || !config.Databases.Any()) throw new ArgumentException($"Cosmos DB Databases ({nameof(config.Databases)}) are not specified");

            var invalidDb = config.Databases.Where(kv => kv.Value == null || !kv.Value.Any()).Select(kv => kv.Key).FirstOrDefault();
            if (invalidDb != null) throw new ArgumentException($"The database '{invalidDb}' has no collections defined");
        }

        private static UniqueKeyPolicy GetUniqueKeyPolicy(CosmosDbCollectionSettings container)
        {
            var uniqueKeyPolicy = new UniqueKeyPolicy();
            foreach (var paths in container.UniqueKeys ?? Enumerable.Empty<string>())
            {
                var uniqueKey = new UniqueKey();

                foreach (var path in paths.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    uniqueKey.Paths.Add(path.Trim());
                }

                uniqueKeyPolicy.UniqueKeys.Add(uniqueKey);
            }

            return uniqueKeyPolicy;
        }
    }
}
