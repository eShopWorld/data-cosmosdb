using System.Collections.Generic;
using Autofac;
using Eshopworld.Data.CosmosDb.Extensions;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests.Extensions
{
    public class ContainerBuilderCosmosExtensionsTests
    {
        private readonly ContainerBuilder _builder = new ContainerBuilder();

        [Fact, IsUnit]
        public void ConfigureCosmosDb_ConfigAbsentConnectionStringPresent_GetsRegistered()
        {
            // Arrange
            var configuration = GetConfigurationWithEntries(new Dictionary<string, string>
            {
                { "CosmosDB:ConnectionString", "AccountEndpoint=Endpoint-abc;AccountKey=Key-abc;" }
            });

            // Act
            var resultBuilder = _builder.ConfigureCosmosDb(configuration);

            // Assert
            var expected = new CosmosDbConfiguration
            {
                DatabaseEndpoint = "Endpoint-abc",
                DatabaseKey = "Key-abc"
            };
            resultBuilder.Build().Resolve<CosmosDbConfiguration>().Should().BeEquivalentTo(expected);
        }

        [Fact, IsUnit]
        public void ConfigureCosmosDb_ConfigPresent_GetsRegistered()
        {
            // Arrange
            var configuration = GetConfigurationWithEntries(new Dictionary<string, string>
            {
                { "DbConfiguration:DatabaseEndpoint", "Endpoint-abc" },
                { "DbConfiguration:DatabaseKey", "Key-abc" }
            });

            // Act
            var resultBuilder = _builder.ConfigureCosmosDb(configuration);

            // Assert
            var expected = new CosmosDbConfiguration
            {
                DatabaseEndpoint = "Endpoint-abc",
                DatabaseKey = "Key-abc"
            };
            resultBuilder.Build().Resolve<CosmosDbConfiguration>().Should().BeEquivalentTo(expected);
        }

        [Fact, IsUnit]
        public void ConfigureCosmosDb_ConfigPresentUnderCustomKey_GetsRegistered()
        {
            // Arrange
            var configuration = GetConfigurationWithEntries(new Dictionary<string, string>
            {
                { "CustomKey:DatabaseEndpoint", "Endpoint-abc" },
                { "CustomKey:DatabaseKey", "Key-abc" },
                { "CustomKey:DefaultTimeToLive", "500" }
            });

            // Act
            var resultBuilder = _builder.ConfigureCosmosDb(configuration, "CustomKey");

            // Assert
            var expected = new CosmosDbConfiguration
            {
                DatabaseEndpoint = "Endpoint-abc",
                DatabaseKey = "Key-abc",
                DefaultTimeToLive = 500
            };
            resultBuilder.Build().Resolve<CosmosDbConfiguration>().Should().BeEquivalentTo(expected);
        }

        private static IConfigurationRoot GetConfigurationWithEntries(IDictionary<string, string> configurationInput) =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(configurationInput)
                .Build();
    }
}