namespace Eshopworld.Data.CosmosDb
{
    public class CosmosDbCollectionSettings
    {
        public string CollectionName { get; set; }
        public string PartitionKey { get; set; }
        public string[] UniqueKeys { get; set; }

        public CosmosDbCollectionSettings() : this(null) { } // default constructor needed
        public CosmosDbCollectionSettings(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
