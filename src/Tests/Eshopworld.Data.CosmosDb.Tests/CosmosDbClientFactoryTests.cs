using Eshopworld.Core;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Moq;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class CosmosDbClientFactoryTests
    {
        private readonly IBigBrother _bigBrother;
        private readonly CosmosDbConfiguration _dbConfiguration;

        public CosmosDbClientFactoryTests(CosmosDbFixture fixture)
        {
            _bigBrother = Mock.Of<IBigBrother>();
            _dbConfiguration = fixture.Configuration;
        }

        [Fact, IsIntegration]
        public void CanInitialiseClient()
        {
            // Arrange
            var dbFactory = new CosmosDbClientFactory(_bigBrother);

            // Act
            var result = dbFactory.InitialiseClient(_dbConfiguration);
            
            // Asser
            result.Should().NotBeNull();
        }
    }
}
