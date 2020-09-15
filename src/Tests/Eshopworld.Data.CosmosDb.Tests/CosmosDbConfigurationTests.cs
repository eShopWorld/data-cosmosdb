using Eshopworld.Tests.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class CosmosDbConfigurationTests
    {
        [Fact, IsUnit]
        public void TryGetDefaults_NoConfigAvailable_OutputsSetToNull()
        {
            // Act
            new CosmosDbConfiguration().TryGetDefaults(out var databaseId, out var collectionName);

            // Assert
            using var _ = new AssertionScope();
            databaseId.Should().BeNull("because no configuration available");
            collectionName.Should().BeNull("because no configuration available");
        }
    }
}