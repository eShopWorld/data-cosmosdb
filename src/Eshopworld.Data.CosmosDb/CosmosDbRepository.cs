using Eshopworld.Data.CosmosDb.Exceptions;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Options;
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
        private readonly CosmosDbConfiguration _dbSetup;
        private string _databaseId;
        private string _collectionName;

        private IDocumentClient dbClient;
        public IDocumentClient DbClient => dbClient ?? (dbClient = _clientFactory.InitialiseClient(_dbSetup));

        public CosmosDbRepository(IOptions<CosmosDbConfiguration> setup, ICosmosDbClientFactory factory)
            : this(setup.Value, factory)
        { }

        public CosmosDbRepository(CosmosDbConfiguration setup, ICosmosDbClientFactory factory)
        {
            _dbSetup = setup ?? throw new ArgumentNullException(nameof(setup));
            _clientFactory = factory ?? throw new ArgumentNullException(nameof(factory));

            if (_dbSetup.TryGetDefaults(out var dbId, out var collectionName))
            {
                UseCollection(collectionName, dbId);
            }
        }

        public void UseCollection(string collectionName, string databaseId = null)
        {
            if (databaseId != null && !_dbSetup.Databases.ContainsKey(databaseId))
            {
                throw new ArgumentException($"The database id '{databaseId}' is not configured");
            }

            this._databaseId = databaseId ?? this._databaseId;

            if (_dbSetup.Databases[this._databaseId].All(c => c.CollectionName != collectionName))
            {
                throw new ArgumentException($"The collection '{collectionName}' is not configured for '{this._databaseId}' database");
            }

            this._collectionName = collectionName;
        }

        public async Task<Document> CreateAsync<T>(T data)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName);

            return await ExecuteFunction(async () =>
            {
                var response = await DbClient.CreateDocumentAsync(collectionUri, data);

                return response.Resource;
            });
        }

        public async Task<Document> UpsertAsync<T>(T data)
        {
            var collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName);

            return await ExecuteFunction(async () =>
            {
                var response = await DbClient.UpsertDocumentAsync(collectionUri, data);
                return response.Resource;
            });
        }

        public async Task<Document> ReplaceAsync<T>(string id, T data, string etag)
        {
            return await ExecuteFunction(async () =>
            {
                var options = new RequestOptions();

                if (etag != null)
                    options.AccessCondition = new AccessCondition
                    {
                        Type = AccessConditionType.IfMatch,
                        Condition = etag
                    };

                var documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionName, id);
                var response = await DbClient.ReplaceDocumentAsync(documentUri, data, options);
                return response.Resource;
            });
        }

        public async Task<bool> DeleteAsync<TPartitionKey>(string id, TPartitionKey partitionKey)
        {
            var documentUri = UriFactory.CreateDocumentUri(_databaseId, _collectionName, id);

            return await ExecuteFunction(async () =>
            {
                try
                {
                    await DbClient.DeleteDocumentAsync(documentUri, new RequestOptions
                    {
                        PartitionKey = new PartitionKey(partitionKey)
                    });

                    return true;
                }
                catch (DocumentClientException exception)
                {
                    if (exception.StatusCode == HttpStatusCode.NotFound && IsErrorMessageForDocumentResource(exception.Error.Message))
                    {
                        return false;
                    }

                    throw;
                }
            });
        }

        public Task<IEnumerable<T>> QueryAsync<T>(QueryDefinition queryDef)
        {
            if (queryDef == null) throw new ArgumentNullException(nameof(queryDef));

            return QueryInternalAsync<T>(queryDef);
        }

        private async Task<IEnumerable<T>> QueryInternalAsync<T>(QueryDefinition queryDef)
        {
            return await ExecuteFunction(async () =>
            {
                var items = new List<T>();

                var collectionUri = UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionName);
                var feedOptions = new FeedOptions { EnableCrossPartitionQuery = queryDef.RequiresCrossPartitions };
                var result = DbClient
                    .CreateDocumentQuery<T>(collectionUri, queryDef.QuerySpec, feedOptions)
                    .AsDocumentQuery();

                while (result.HasMoreResults)
                {
                    var batch = await result.ExecuteNextAsync<T>();
                    items.AddRange(batch);
                }

                return items;
            });
        }

        public async Task<IEnumerable<DocumentContainer<T>>> QueryWithContainerAsync<T>(QueryDefinition queryDef)
        {
            var items = await QueryAsync<dynamic>(queryDef);
            return items.Select(MapInstance<T>);
        }

        private DocumentContainer<T> MapInstance<T>(object instance)
        {
            var jToken = JToken.FromObject(instance);
            var eTag = jToken["_etag"]?.ToString();

            if (eTag == null)
            {
                throw new ArgumentException("The query provided does not return ETag information");
            }

            return new DocumentContainer<T>(jToken.ToObject<T>(), eTag);
        }

        private async Task<TResult> ExecuteFunction<TResult>(Func<Task<TResult>> func)
        {
            while (true)
            {
                try
                {
                    return await func();
                }
                catch (DocumentClientException ex) when (ex.StatusCode == HttpStatusCode.PreconditionFailed)
                {
                    throw new StaleDataException();
                }
                catch (DocumentClientException ex) when ((ex.StatusCode == HttpStatusCode.NotFound ||
                                                          ex.StatusCode == HttpStatusCode.Gone) &&
                                                         !IsErrorMessageForDocumentResource(ex.Error.Message))
                {
                    // this seems like an error - status code in exception is 404 but in the message details it is 410
                    // when DB/collection are deleted for some operations
                    InvalidateClient();
                }
                catch (DocumentClientException ex) when (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    await Task.Delay(ex.RetryAfter);
                }
                catch (DocumentClientException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new MissingDocumentException();
                }
            }
        }

        // there is no easy way to determine if a DB/collection is not found or simply document could be found
        // from error message inspection, the below should be present only for missing document (client ok)
        // when collection/DB are deleted, status code in message text is 404 but resource type is 'collection'.
        // however, that is not the case when Delete is performed - in tha case, StatusCode is 410 in the message text
        // and the resource type is 'document'
        private bool IsErrorMessageForDocumentResource(string message) => 
            message.Contains("StatusCode: 404") && message.Contains("ResourceType: Document");

        private void InvalidateClient()
        {
            dbClient = null;
            _clientFactory.Invalidate();
        }
    }
}
