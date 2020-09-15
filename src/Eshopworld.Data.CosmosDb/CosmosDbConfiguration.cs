using System.Collections.Generic;
using System.Linq;

namespace Eshopworld.Data.CosmosDb
{
    public class CosmosDbConfiguration
    {
        /// <summary>
        /// Defines the URI for the cosmos DB account
        /// </summary>
        public string DatabaseEndpoint { get; set; }

        /// <summary>
        /// Defines a key for the account
        /// </summary>
        public string DatabaseKey { get; set; }

        /// <summary>
        /// Defines a throughput for the databases and collections
        /// </summary>
        public int Throughput { get; set; } = 400;

        /// <summary>
        /// Defines TTL on database level
        /// </summary>
        public int? DefaultTimeToLive { get; set; }

        /// <summary>
        /// Defines list of databases with their settings.
        /// The key represents the database name.
        /// </summary>
        public IDictionary<string, CosmosDbCollectionSettings[]> Databases { get; set; }

        public bool TryGetDefaults(out string databaseId, out string collectionName)
        {
            var defaults = Databases?.FirstOrDefault() ?? new KeyValuePair<string, CosmosDbCollectionSettings[]>();
            if (defaults.Key != null && defaults.Value != null && defaults.Value.Any())
            {
                databaseId = defaults.Key;
                collectionName = defaults.Value[0].CollectionName;
                return !string.IsNullOrWhiteSpace(databaseId) && !string.IsNullOrWhiteSpace(collectionName);
            }

            databaseId = null;
            collectionName = null;
            return false;
        }

        public bool HasCosmosEndpointAndKey()
        {
            return !string.IsNullOrEmpty(DatabaseEndpoint) &&
                   !string.IsNullOrEmpty(DatabaseKey);
        }
    }
}
