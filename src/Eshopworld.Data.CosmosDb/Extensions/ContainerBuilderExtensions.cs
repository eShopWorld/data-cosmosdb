using Autofac;
using Microsoft.Extensions.Configuration;

namespace Eshopworld.Data.CosmosDb.Extensions
{
    public static class ContainerBuilderExtensions
    {
        private const string DbConfigurationSection = "DbConfiguration";

        /// <summary>
        /// Configures Cosmos DB from existing config store.
        /// </summary>
        public static ContainerBuilder ConfigureCosmosDb(this ContainerBuilder builder, IConfiguration configuration)
        {
            var cosmosDbConfiguration = configuration.GetSection(DbConfigurationSection).Get<CosmosDbConfiguration>() ?? new CosmosDbConfiguration();
            var hasCosmosConnectionDetails = cosmosDbConfiguration.HasCosmosEndpointAndKey();

            if (!hasCosmosConnectionDetails)
            {
                var (dbEndpoint, dbKey) = ConfigurationParser.GetCosmosSettings(configuration);
                cosmosDbConfiguration.DatabaseEndpoint = dbEndpoint;
                cosmosDbConfiguration.DatabaseKey = dbKey;
            }

            builder.RegisterInstance(cosmosDbConfiguration);
            return builder;
        }
    }
}
