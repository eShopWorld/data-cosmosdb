using System;
using Eshopworld.Data.CosmosDb.Exceptions;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class ConfigurationParserTests
    {
        private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();

        [Fact, IsUnit]
        public void GetCosmosSettings_ConnectionStringMissing_ThrowsException()
        {
            // Arrange
            _configuration.Setup(c => c["connectionString1"]).Returns("");

            // Act
            Func<(string dbEndpoint, string dbKey)> func = () => ConfigurationParser.GetCosmosSettings(_configuration.Object, "connectionString1");

            // Assert
            func.Should().ThrowExactly<CosmosDbConfigurationException>()
                .Which.Message.Should().Be("Missing connection string for key 'connectionString1'");
        }

        [Fact, IsUnit]
        public void GetCosmosSettings_ConnectionStringMissingEndpoint_ThrowsException()
        {
            // Arrange
            _configuration.Setup(c => c["connectionString1"]).Returns("Abc=d;");

            // Act
            Func<(string dbEndpoint, string dbKey)> func = () => ConfigurationParser.GetCosmosSettings(_configuration.Object, "connectionString1");

            // Assert
            func.Should().ThrowExactly<CosmosDbConfigurationException>()
                .Which.Message.Should().Be("Missing DB endpoint");
        }

        [Fact, IsUnit]
        public void GetCosmosSettings_ConnectionStringEndpointEmpty_ThrowsException()
        {
            // Arrange
            _configuration.Setup(c => c["connectionString1"]).Returns("AccountEndpoint='';");

            // Act
            Func<(string dbEndpoint, string dbKey)> func = () => ConfigurationParser.GetCosmosSettings(_configuration.Object, "connectionString1");

            // Assert
            func.Should().ThrowExactly<CosmosDbConfigurationException>()
                .Which.Message.Should().Be("Missing DB endpoint");
        }

        [Fact, IsUnit]
        public void GetCosmosSettings_ConnectionStringMissingAccountKey_ThrowsException()
        {
            // Arrange
            _configuration.Setup(c => c["connectionString1"]).Returns("AccountEndpoint=ae-test;");

            // Act
            Func<(string dbEndpoint, string dbKey)> func = () => ConfigurationParser.GetCosmosSettings(_configuration.Object, "connectionString1");

            // Assert
            func.Should().ThrowExactly<CosmosDbConfigurationException>()
                .Which.Message.Should().Be("Missing DB account key");
        }

        [Fact, IsUnit]
        public void GetCosmosSettings_ConnectionStringAccountKeyEmpty_ThrowsException()
        {
            // Arrange
            _configuration.Setup(c => c["connectionString1"]).Returns("AccountEndpoint=ae-test;AccountKey='';");

            // Act
            Func<(string dbEndpoint, string dbKey)> func = () => ConfigurationParser.GetCosmosSettings(_configuration.Object, "connectionString1");

            // Assert
            func.Should().ThrowExactly<CosmosDbConfigurationException>()
                .Which.Message.Should().Be("Missing DB account key");
        }

        [Fact, IsUnit]
        public void GetCosmosSettings_ConnectionStringValid_ReturnsResult()
        {
            // Arrange
            _configuration.Setup(c => c["connectionString1"]).Returns("AccountEndpoint=ae-test;AccountKey=ak-test;");

            // Act
            var result = ConfigurationParser.GetCosmosSettings(_configuration.Object, "connectionString1");

            // Assert
            result.Should().Be(("ae-test", "ak-test"));
        }
    }
}