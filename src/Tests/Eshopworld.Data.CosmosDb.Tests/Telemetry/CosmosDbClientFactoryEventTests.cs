using Eshopworld.Data.CosmosDb.Telemetry;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests.Telemetry
{
    public class CosmosDbClientFactoryEventTests
    {
        [Fact, IsUnit]
        public void Constructor_CreatesInstance_MessagePropertyMatches()
        {
            // Act
            var testEvent = new CosmosDbClientFactoryEvent("test-message");

            // Assert
            testEvent.Should().BeOfType<CosmosDbClientFactoryEvent>().Which.Message.Should().Be("test-message");
        }
    }
}