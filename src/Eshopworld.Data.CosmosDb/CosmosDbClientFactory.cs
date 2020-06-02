using Eshopworld.Core;
using Eshopworld.Data.CosmosDb.Telemetry;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Eshopworld.Data.CosmosDb
{
    public class CosmosDbClientFactory : ICosmosDbClientFactory, IDisposable
    {
        private readonly object _sync = new object();
        private readonly IBigBrother _bb;

        private DocumentClient _client;

        public CosmosDbClientFactory(IBigBrother bb)
        {
            _bb = bb ?? throw new ArgumentNullException(nameof(bb));
        }

        public IDocumentClient InitialiseClient(CosmosDbConfiguration config)
        {
            if (_client != null) return _client;

            LogEvent("Initialising Cosmos DB client");

            ValidateConfiguration(config);

            lock (_sync)
            {
                if (_client != null) return _client;

                var client = new DocumentClient(new Uri(config.DatabaseEndpoint), config.DatabaseKey);

                var databaseTasks = config.Databases
                    .Select(kv => client.CreateDatabaseIfNotExistsAsync(
                       new Database { Id = kv.Key },
                       new RequestOptions { OfferThroughput = config.Throughput }))
                    .ToArray();

                Task.WhenAll(databaseTasks)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                // this should be executed after the DB is created
                var collectionTasks = config.Databases
                    .SelectMany(kv =>
                       kv.Value.Select(col =>
                              client.CreateDocumentCollectionIfNotExistsAsync(
                                  UriFactory.CreateDatabaseUri(kv.Key),
                                  SetupCollection(col)
                              )
                       ))
                    .ToArray();

                Task.WhenAll(collectionTasks)
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult();

                _client = client;

                LogEvent("Cosmos Db Client initialisation completed");
            }

            return _client;
        }

        public void Invalidate()
        {
            LogEvent("Invalidating Cosmos Db Client");

            lock (_sync)
            {
                _client.Dispose();
                _client = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
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

            LogEvent("Configuration verified");
        }

        private DocumentCollection SetupCollection(CosmosDbCollectionSettings setup)
        {

            var collection = new DocumentCollection
            {
                Id = setup.CollectionName,
            };

            if (!string.IsNullOrWhiteSpace(setup.PartitionKey))
            {
                collection.PartitionKey.Paths.Add(setup.PartitionKey);
            }

            if (setup.UniqueKeys != null && setup.UniqueKeys.Any())
            {
                foreach (var paths in setup.UniqueKeys)
                {
                    var uniqueKey = new UniqueKey();

                    foreach (var path in paths.Split(",", StringSplitOptions.RemoveEmptyEntries))
                    {
                        uniqueKey.Paths.Add(path.Trim());
                    }

                    collection.UniqueKeyPolicy.UniqueKeys.Add(uniqueKey);
                }
            }

            return collection;
        }

        private void LogEvent(string message)
        {
            _bb.Publish(new CosmosDbClientFactoryEvent(message));
        }
    }
}
