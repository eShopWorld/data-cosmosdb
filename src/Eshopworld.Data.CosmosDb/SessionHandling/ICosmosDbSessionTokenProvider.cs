namespace Eshopworld.Data.CosmosDb.SessionHandling
{
    internal interface ICosmosDbSessionTokenProvider
    {
        string SessionToken { get; set; }
    }
}
