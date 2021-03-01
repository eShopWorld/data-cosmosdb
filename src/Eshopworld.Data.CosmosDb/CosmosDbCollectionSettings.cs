using System.Diagnostics.CodeAnalysis;

namespace Eshopworld.Data.CosmosDb
{
    [ExcludeFromCodeCoverage]
    public class CosmosDbCollectionSettings
    {
        /// <summary>
        /// Defines collection name. Collection will be created if does not exist
        /// </summary>
        public string CollectionName { get; set; }

        /// <summary>
        /// Defines partition key. Has no effect for an existing collection
        /// </summary>
        public string PartitionKey { get; set; }

        /// <summary>
        /// List of unique constrain keys defined on the document fields
        /// </summary>
        public string[] UniqueKeys { get; set; }

        // default constructor needed
        public CosmosDbCollectionSettings(): this(null)
        {
        }

        public CosmosDbCollectionSettings (string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
