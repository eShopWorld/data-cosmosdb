using System.Collections.Generic;
using System.Linq;

namespace Eshopworld.Data.CosmosDb
{
    public class CosmosDbConfiguration
    {
        public string DatabaseEndpoint { get; set; }
        public string DatabaseKey { get; set; }
        public int Throughput { get; set; } = 400;
        public int? DefaultTimeToLive { get; set; }

        /// <summary>
        /// Defines list of databases with their settings.
        /// The key represents the database name.
        /// </summary>
        public IDictionary<string, CosmosDbCollectionSettings[]> Databases { get; set; }

        public bool TryGetDefaults(out string databaseId, out string collectionName)
        {
            var defaults = Databases.FirstOrDefault();
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
