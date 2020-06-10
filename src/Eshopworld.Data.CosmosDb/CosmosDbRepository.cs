using Eshopworld.Core;
using Eshopworld.Data.CosmosDb.Exceptions;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Eshopworld.Data.CosmosDb
{
    public class CosmosDbRepository : ICosmosDbRepository
    {
        private readonly ICosmosDbClientFactory _clientFactory;
        private readonly IBigBrother _bigBrother;
        private readonly CosmosDbConfiguration _dbSetup;
        private string _databaseId;
        private string _containerName;

        private CosmosClient _dbClient;
        private Container _container;

        public CosmosDbRepository(
            CosmosDbConfiguration setup,
            ICosmosDbClientFactory factory,
            IBigBrother bigBrother)
        {
            _dbSetup = setup ?? throw new ArgumentNullException(nameof(setup));
            _clientFactory = factory ?? throw new ArgumentNullException(nameof(factory));
            _bigBrother = bigBrother ?? throw new ArgumentNullException(nameof(bigBrother));

            if (_dbSetup.TryGetDefaults(out var dbId, out var collectionName))
                UseCollection(collectionName, dbId);
        }

        private CosmosClient DbClient => _dbClient ??= _clientFactory.InitialiseClient(_dbSetup);

        public Container DbContainer => _container ??= DbClient.GetContainer(_databaseId, _containerName);

        public void UseCollection(string collectionName, string databaseId = null)
        {
            if (databaseId != null && !_dbSetup.Databases.ContainsKey(databaseId))
                throw new ArgumentException($"The database id '{databaseId}' is not configured");

            _databaseId = databaseId ?? _databaseId;

            if (_dbSetup.Databases[_databaseId].All(c => c.CollectionName != collectionName))
                throw new ArgumentException($"The collection '{collectionName}' is not configured for '{_databaseId}' database");

            _containerName = collectionName;
        }

        public async Task<T> CreateAsync<T>(T data)
        {
            return await ExecuteFunction(async () =>
            {
                var response = await DbContainer.CreateItemAsync(data);
                return response.Resource;
            });
        }

        public async Task<T> UpsertAsync<T>(T data)
        {
            return await ExecuteFunction(async () =>
            {
                var response = await DbContainer.UpsertItemAsync(data);
                return response.Resource;
            });
        }

        public async Task<T> ReplaceAsync<T>(string id, T data, string etag = null)
        {
            return await ExecuteFunction(async () =>
            {
                var itemRequestOptions = etag != null
                    ? new ItemRequestOptions { IfMatchEtag = etag }
                    : null;
                var response = await DbContainer.ReplaceItemAsync(
                    data,
                    id,
                    requestOptions: itemRequestOptions);

                return response.Resource;
            });
        }

        public async Task<bool> DeleteAsync<T>(string id, string partitionKey)
        {
            return await ExecuteFunction(async () =>
            {
                try
                {
                    await DbContainer.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
                    return true;
                }
                catch (CosmosException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        return false;
                    }

                    throw;
                }
            });
        }

        public Task<IEnumerable<T>> QueryAsync<T>(CosmosQuery cosmosQueryDef)
        {
            if (cosmosQueryDef == null) throw new ArgumentNullException(nameof(cosmosQueryDef));

            return QueryInternalAsync<T>(cosmosQueryDef);
        }

        private async Task<IEnumerable<T>> QueryInternalAsync<T>(CosmosQuery cosmosQueryDef)
        {
            return await ExecuteFunction(async () =>
            {
                QueryRequestOptions requestOptions = null;
                if (!string.IsNullOrEmpty(cosmosQueryDef.PartitionKey))
                {
                    requestOptions = new QueryRequestOptions
                    {
                        PartitionKey = new PartitionKey(cosmosQueryDef.PartitionKey)
                    };
                }

                var iterator = DbContainer.GetItemQueryIterator<T>(cosmosQueryDef.QueryDefinition, null, requestOptions);

                var items = new List<T>();
                while (iterator.HasMoreResults)
                {
                    var batch = await iterator.ReadNextAsync();
                    items.AddRange(batch);
                }

                return items;
            });
        }

        public Task<IEnumerable<T>> QueryAsync<T>(QueryDefinition queryDefinition, string partitionKey = null)
        {
            return QueryAsync<T>(new CosmosQuery(queryDefinition, partitionKey));
        }

        /// <summary>
        /// Replaces existing document with given identifier with the provided data
        /// </summary>
        /// <typeparam name="T">Type of document data</typeparam>
        /// <param name="id">Identifier of the document that is to be replaced</param>
        /// <param name="data">New version of the document data</param>
        /// <returns></returns>
        public Task<T> ReplaceAsync<T>(string id, T data)
        {
            return ReplaceAsync(id, data, null);
        }

        public async Task<IEnumerable<DocumentContainer<T>>> QueryWithContainerAsync<T>(CosmosQuery cosmosQueryDef)
        {
            var items = await QueryAsync<dynamic>(cosmosQueryDef);
            return items.Select(MapInstance<T>);
        }

        private DocumentContainer<T> MapInstance<T>(object instance)
        {
            var jToken = JToken.FromObject(instance);
            var eTag = jToken["_etag"]?.ToString();

            if (eTag == null)
                throw new ArgumentException("Provided query does not return eTag information");

            return new DocumentContainer<T>(jToken.ToObject<T>(), eTag);
        }

        private async Task<TResult> ExecuteFunction<TResult>(Func<Task<TResult>> func)
        {
            while (true)
                try
                {
                    return await func();
                }
                catch (CosmosException ex) when (IsCollectionOrDatabaseMissing(ex))
                {
                    InvalidateClient();
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    throw new StaleDataException();
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(ex.RetryAfter ?? TimeSpan.FromSeconds(1.0));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new MissingDocumentException();
                }
                catch (CosmosException exception)
                {
                    _bigBrother.Publish(exception);
                    throw;
                }
        }

        private bool IsCollectionOrDatabaseMissing(CosmosException exception) => exception.StatusCode == HttpStatusCode.NotFound && exception.Message.Contains("ResourceType: Collection");

        private void InvalidateClient()
        {
            _dbClient = null;
            _container = null;
            _clientFactory.Invalidate();
        }
    }
}
