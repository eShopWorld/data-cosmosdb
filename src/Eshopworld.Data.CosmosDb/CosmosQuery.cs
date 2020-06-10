using Microsoft.Azure.Cosmos;

namespace Eshopworld.Data.CosmosDb
{
    public class CosmosQuery
    {
        public readonly QueryDefinition QueryDefinition;
        public readonly string PartitionKey;

        public CosmosQuery(QueryDefinition queryDefinition, string partitionKey = null)
        {
            QueryDefinition = queryDefinition;
            PartitionKey = partitionKey;
        }

        public CosmosQuery(string sql, string partitionKey = null)
            : this(new QueryDefinition(sql), partitionKey)
        {
        }
    }
}
