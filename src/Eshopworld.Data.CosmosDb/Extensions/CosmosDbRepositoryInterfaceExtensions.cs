using Microsoft.Azure.Documents;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshopworld.Data.CosmosDb.Extensions
{
    public static class CosmosDbRepositoryInterfaceExtensions
    {
        public static Task<IEnumerable<T>> QueryAsync<T>(this ICosmosDbRepository repo, string query, bool crossPartitionQuery)
        {
            return repo.QueryAsync<T>(new QueryDefinition(query, crossPartitionQuery));
        }

        /// <summary>
        /// Replaces existing document with given identifier with the provided data
        /// </summary>
        /// <typeparam name="T">Type of document data</typeparam>
        /// <param name="id">Identifier of the document that is to be replaced</param>
        /// <param name="data">New version of the document data</param>
        /// <returns></returns>
        public static Task<Document> ReplaceAsync<T>(this ICosmosDbRepository repo, string id, T data)
        {
            return repo.ReplaceAsync(id, data, null);
        }
    }
}
