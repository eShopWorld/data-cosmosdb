using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Eshopworld.Data.CosmosDb
{
    public class DocumentDBRepository<T> where T : class
    {
        private string _collectionId;
        private string _databaseId;

        public DocumentClient Client { get; private set; }

        public virtual void Initialize(string endpoint, string authKey, string databaseId, string collectionId)
        {
            _databaseId = databaseId;
            _collectionId = collectionId;
            Client = new DocumentClient(new Uri(endpoint), authKey); // Todo: Implement direct TCP connectivity.
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync(1000).Wait();
            // Todo: Consider performance-tuning: https://docs.microsoft.com/en-us/azure/cosmos-db/performance-tips
        }

        public virtual async Task<Document> CreateItemAsync(T item)
        {
            return await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId),
                item);
        }

        public virtual async Task<Document> UpdateItemAsync(string id, T item, RequestOptions options = null)
        {
            return await Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id),
                item, options);
        }

        public virtual async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await GetDocumentAsync(id);
                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public virtual async Task<Document> GetDocumentAsync(string id)
        {
            try
            {
                Document document =
                    await Client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
                return document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    return null;
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            var query = Client.CreateDocumentQuery<T>(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId))
                .Where(predicate)
                .AsDocumentQuery();

            var results = new List<T>();
            while (query.HasMoreResults) results.AddRange(await query.ExecuteNextAsync<T>());

            return results;
        }

        public virtual async Task<bool> DeleteItemAsync(string id)
        {
            try
            {
                await Client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseId, _collectionId, id));
            }
            catch (DocumentClientException)
            {
                return false;
            }

            return true;
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    await Client.CreateDatabaseAsync(new Database {Id = _databaseId});
                else
                    throw;
            }
        }

        private async Task CreateCollectionIfNotExistsAsync(int? offerThroughput)
        {
            try
            {
                await Client.ReadDocumentCollectionAsync(
                    UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    await Client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_databaseId),
                        new DocumentCollection {Id = _collectionId},
                        new RequestOptions {OfferThroughput = offerThroughput});
                else
                    throw;
            }
        }
    }
}