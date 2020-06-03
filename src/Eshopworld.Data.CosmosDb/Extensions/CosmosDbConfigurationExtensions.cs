using Autofac;
using Microsoft.Extensions.Configuration;

namespace Eshopworld.Data.CosmosDb.Extensions
{
    /// <summary>
    /// Extension class to wrap configuration of CosmosDB
    /// </summary>
    public static class CosmosDbConfigurationExtensions
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

        private static bool HasCosmosEndpointAndKey(this CosmosDbConfiguration cosmosConfig)
        {
            return !string.IsNullOrEmpty(cosmosConfig.DatabaseEndpoint) &&
                   !string.IsNullOrEmpty(cosmosConfig.DatabaseKey);
        }
    }
}
