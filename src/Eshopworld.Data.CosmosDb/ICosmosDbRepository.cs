using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshopworld.Data.CosmosDb
{
    /// <summary>
    /// Defines basic interface for cosmos db-based repository
    /// </summary>
    public interface ICosmosDbRepository
    {
        /// <summary>
        /// Defines the collection (and from which database) that should be used by the repository.
        /// </summary>
        /// <exception cref="ArgumentException">When the collection name or database id is incorrect</exception>
        void UseCollection(string collectionName, string databaseId = null);

        /// <summary>
        /// Customize behaviour with CosmosClientOptions
        /// </summary>
        /// <param name="action">Action to configure CosmosClientOptions</param>
        void WithCosmosClientOptions(Action<CosmosClientOptions> action);

        /// <summary>
        /// Creates new instance of a document in the database.
        /// </summary>
        /// <typeparam name="T">Type of the document data</typeparam>
        /// <param name="data">Document data</param>
        /// <returns>An instance of <see cref="DocumentContainer{TDoc}"/></returns>
        Task<DocumentContainer<T>> CreateAsync<T>(T data);

        /// <summary>
        /// Creates or replaces existing document in the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns>An instance of <see cref="DocumentContainer{TDoc}"/></returns>
        Task<DocumentContainer<T>> UpsertAsync<T>(T data);

        /// <summary>
        /// Replaces existing document with given identifier with the provided data
        /// as long as the provided etag matches the one in the stored document.
        /// If the eTag is null, no condition checking is performed
        /// </summary>
        /// <typeparam name="T">Type of document data</typeparam>
        /// <param name="id">Identifier of the document that is to be replaced</param>
        /// <param name="data">New version of the document data</param>
        /// <param name="etag">Optional - matching etag condition for the update to take place</param>
        /// <returns>An instance of <see cref="DocumentContainer{TDoc}"/></returns>
        /// <exception cref="StaleDataException">When provided etag does not match the one record in the document being updated</exception>
        Task<DocumentContainer<T>> ReplaceAsync<T>(string id, T data, string etag = null);

        /// <summary>
        /// Deletes a document with a given identifier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="partitionKey"></param>
        /// <returns>True if document was deleted, False if not found</returns>
        Task<bool> DeleteAsync<T>(string id, string partitionKey);

        /// <summary>
        /// Queries for documents based on the given query.
        /// Returns full list of retrieved documents.
        /// </summary>
        /// <typeparam name="T">Type of document data</typeparam>
        /// <param name="cosmosQueryDef">Query definition</param>
        /// <returns>Collection of documents matching given query</returns>
        Task<IEnumerable<T>> QueryAsync<T>(CosmosQuery cosmosQueryDef);

        /// <summary>
        /// Queries for documents based on the given query.
        /// Returns full list of retrieved documents wrapped with document meta-data
        /// </summary>
        /// <typeparam name="T">Type of document data</typeparam>
        /// <param name="cosmosQueryDef">Query definition</param>
        /// <returns>Collection of documents matching given query with extended meta-data for each document</returns>
        Task<IEnumerable<DocumentContainer<T>>> QueryWithContainerAsync<T>(CosmosQuery cosmosQueryDef);

        /// <summary>
        /// Returns a document by the given identifier
        /// </summary>
        /// <typeparam name="T">Type of document data</typeparam>
        /// <param name="id">The identifier of the document</param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        Task<DocumentContainer<T>> GetByIdAsync<T>(string id, string partitionKey);
    }
}