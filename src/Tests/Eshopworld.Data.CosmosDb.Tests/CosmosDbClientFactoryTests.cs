using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Eshopworld.Core;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class CosmosDbClientFactoryTests
    {
        private readonly CosmosDbClientFactory _factory = new CosmosDbClientFactory(Mock.Of<IBigBrother>());

        [Fact, IsUnit]
        public void Constructor_InitializesInstance()
        {
            // Act
            var instance = new CosmosDbClientFactory(Mock.Of<IBigBrother>());

            // Assert
            instance.Should().NotBeNull();
        }

        [Fact, IsUnit]
        public void Constructor_BigBrotherNull_ThrowsException()
        {
            // Act
            Func<CosmosDbClientFactory> func = () => new CosmosDbClientFactory(null);

            // Assert
            func.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact, IsUnit]
        public void Invalidate_CallForInvalidationWithNoClient_CompletesSuccessfully()
        {
            // Act
            _factory.Invalidate();
        }

        [Fact, IsUnit]
        public void Invalidate_CallForDisposeWithNoClient_CompletesSuccessfully()
        {
            // Act
            _factory.Dispose();
        }

        [Fact, IsUnit]
        public void InitialiseClient_ConfigNull_ThrowsException()
        {
            // Act
            Action action = () => _factory.InitialiseClient(null, new CosmosClientOptions());

            // Assert
            action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact, IsUnit]
        public void InitialiseClient_DatabaseEndpointConfigMissing_ThrowsException()
        {
            // Arrange
            var config = new CosmosDbConfiguration();

            // Act
            Action action = () => _factory.InitialiseClient(config, new CosmosClientOptions());

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.Message.Should().Be("Cosmos DB database endpoint (DatabaseEndpoint) is not defined");
        }

        [Fact, IsUnit]
        public void InitialiseClient_DatabaseKeyConfigMissing_ThrowsException()
        {
            // Arrange
            var config = new CosmosDbConfiguration
            {
                DatabaseEndpoint = "test-db-endpoint"
            };

            // Act
            Action action = () => _factory.InitialiseClient(config, new CosmosClientOptions());

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.Message.Should().Be("Cosmos DB database key (DatabaseKey) is not defined");
        }

        [Fact, IsUnit]
        public void InitialiseClient_DatabasesConfigMissing_ThrowsException()
        {
            // Arrange
            var config = new CosmosDbConfiguration
            {
                DatabaseEndpoint = "test-db-endpoint",
                DatabaseKey = "test-db-key"
            };

            // Act
            Action action = () => _factory.InitialiseClient(config, new CosmosClientOptions());

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.Message.Should().Be("Cosmos DB Databases (Databases) are not specified");
        }

        [Fact, IsUnit]
        public void InitialiseClient_DatabasesConfigEmpty_ThrowsException()
        {
            // Arrange
            var config = new CosmosDbConfiguration
            {
                DatabaseEndpoint = "test-db-endpoint",
                DatabaseKey = "test-db-key",
                Databases = new Dictionary<string, CosmosDbCollectionSettings[]>()
            };

            // Act
            Action action = () => _factory.InitialiseClient(config, new CosmosClientOptions());

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.Message.Should().Be("Cosmos DB Databases (Databases) are not specified");
        }

        [Fact, IsUnit]
        public void InitialiseClient_DatabasesCollectionSettingsMissing_ThrowsException()
        {
            // Arrange
            var config = new CosmosDbConfiguration
            {
                DatabaseEndpoint = "test-db-endpoint",
                DatabaseKey = "test-db-key",
                Databases = new Dictionary<string, CosmosDbCollectionSettings[]>
                {
                    ["db1"] = new CosmosDbCollectionSettings[0]
                }
            };

            // Act
            Action action = () => _factory.InitialiseClient(config, new CosmosClientOptions());

            // Assert
            action.Should().ThrowExactly<ArgumentException>()
                .Which.Message.Should().Be("The database 'db1' has no collections defined");
        }
    }
}