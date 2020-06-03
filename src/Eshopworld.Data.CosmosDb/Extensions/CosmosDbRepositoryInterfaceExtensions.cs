using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Eshopworld.Data.CosmosDb.Extensions
{
    public static class CosmosDbRepositoryExtensions
    {
        public static Task<IEnumerable<T>> QueryAsync<T>(
            this ICosmosDbRepository repo,
            QueryDefinition queryDefinition,
            string partitionKey = null)
        {
            return repo.QueryAsync<T>(new CosmosQuery(queryDefinition, partitionKey));
        }

        /// <summary>
        /// Replaces existing document with given identifier with the provided data
        /// </summary>
        /// <typeparam name="T">Type of document data</typeparam>
        /// <param name="id">Identifier of the document that is to be replaced</param>
        /// <param name="data">New version of the document data</param>
        /// <returns></returns>
        public static Task<T> ReplaceAsync<T>(this ICosmosDbRepository repo, string id, T data)
        {
            return repo.ReplaceAsync(id, data, null);
        }
    }
}
