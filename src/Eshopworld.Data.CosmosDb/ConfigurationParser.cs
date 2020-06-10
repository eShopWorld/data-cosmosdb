using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Eshopworld.Data.CosmosDb.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Eshopworld.Data.CosmosDb
{
    /// <summary>
    /// Helper to encapsulate logic around fetching DB Connection string settings
    /// <remarks>
    /// Will throw a <see cref="CosmosDbConfigurationException"/> if cannot correctly parse connection string
    /// </remarks>
    /// </summary>
    public static class ConfigurationParser
    {
        private const string ConnectionStringKey = "CosmosDB:ConnectionString";

        private const string EndPointKey = "AccountEndpoint";

        private const string AccountKey = "AccountKey";

        public static (string dbEndpoint, string dbKey) GetCosmosSettings(IConfiguration configuration, string connectionStringKey = ConnectionStringKey)
        {
            var connectionString = configuration[connectionStringKey];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new CosmosDbConfigurationException("Missing connection string");
            }

            var connectionStringBuilder = new DbConnectionStringBuilder { ConnectionString = connectionString };

            var dbEndpoint = GetDbEndpoint(connectionStringBuilder);
            var dbKey = GetDatabaseKey(connectionStringBuilder);

            return (dbEndpoint, dbKey);
        }

        private static string GetDatabaseKey(DbConnectionStringBuilder connectionStringBuilder)
        {
            if (!connectionStringBuilder.ContainsKey(AccountKey))
            {
                throw new CosmosDbConfigurationException("Missing DB account key");
            }

            var databaseKey = connectionStringBuilder[AccountKey]?.ToString();
            if (string.IsNullOrEmpty(databaseKey))
            {
                throw new CosmosDbConfigurationException("Missing DB account key");
            }

            return databaseKey;
        }

        private static string GetDbEndpoint(DbConnectionStringBuilder connectionStringBuilder)
        {
            if (!connectionStringBuilder.ContainsKey(EndPointKey))
            {
                throw new CosmosDbConfigurationException("Missing DB endpoint");
            }

            var dbEndpoint = connectionStringBuilder[EndPointKey]?.ToString();
            if (string.IsNullOrEmpty(dbEndpoint))
            {
                throw new CosmosDbConfigurationException("Missing DB endpoint");
            }

            return dbEndpoint;
        }
    }
}
