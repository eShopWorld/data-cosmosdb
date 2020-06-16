using Autofac;
using Microsoft.Extensions.Configuration;

namespace Eshopworld.Data.CosmosDb.Extensions
{
    public static class ContainerBuilderCosmosExtensions
    {
        private const string DefaultConfigurationSection = "DbConfiguration";

        private const string DefaultConnectionStringKey = "CosmosDB:ConnectionString";

        /// <summary>
        /// Configures Cosmos DB from existing config store.
        /// </summary>
        /// <param name="builder">Extension of a builder</param>
        /// <param name="configuration">Instance of the configuration holding the database settings</param>
        /// <param name="configurationSectionKey">Optional key for entire configuration section</param>
        /// <param name="connectionStringKey">Optional key for connection string entry in the configuration.
        /// If database URI and key are not provided as part of the configuration section,
        /// connectionStringKey can be specified to retrieve the database URI and key.
        /// The key is ignored configuration section contains the right settings</param>
        public static ContainerBuilder ConfigureCosmosDb (this ContainerBuilder builder, IConfiguration configuration, 
            string configurationSectionKey = DefaultConfigurationSection, string connectionStringKey = null)
        {
            var cosmosDbConfiguration = configuration.GetSection (configurationSectionKey).Get<CosmosDbConfiguration>() ?? new CosmosDbConfiguration();
            var hasCosmosConnectionDetails = cosmosDbConfiguration.HasCosmosEndpointAndKey ();

            if (!hasCosmosConnectionDetails)
            {
                var (dbEndpoint, dbKey) = ConfigurationParser.GetCosmosSettings(configuration, connectionStringKey ?? DefaultConnectionStringKey);
                cosmosDbConfiguration.DatabaseEndpoint = dbEndpoint;
                cosmosDbConfiguration.DatabaseKey = dbKey;
            }

            builder.RegisterInstance(cosmosDbConfiguration);
            return builder;
        }
    }
}
