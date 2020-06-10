namespace Eshopworld.Data.CosmosDb
{
    public class CosmosDbCollectionSettings
    {
        public string CollectionName { get; set; }
        public string PartitionKey { get; set; }
        public string[] UniqueKeys { get; set; }

        // default constructor needed
        public CosmosDbCollectionSettings() : this(null) { }
        public CosmosDbCollectionSettings(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}
