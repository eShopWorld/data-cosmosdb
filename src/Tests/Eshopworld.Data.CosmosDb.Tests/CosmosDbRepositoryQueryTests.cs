using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Eshopworld.Core;
using Eshopworld.Data.CosmosDb.Exceptions;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class CosmosDbRepositoryQueryTests
    {
        private readonly CosmosDbRepository _repository;
        private readonly Mock<Container> _containerMock = new Mock<Container>();
        private readonly Mock<ICosmosDbClientFactory> _clientFactoryMock = new Mock<ICosmosDbClientFactory>();

        public CosmosDbRepositoryQueryTests()
        {
            var configuration = new CosmosDbConfiguration
            {
                DatabaseEndpoint = string.Empty,
                DatabaseKey = string.Empty,
                Databases = new Dictionary<string, CosmosDbCollectionSettings[]>
                {
                    [string.Empty] = new[]
                    {
                        new CosmosDbCollectionSettings(string.Empty)
                    }
                }
            };

            var clientMock = new Mock<CosmosClient>();
            clientMock
                .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_containerMock.Object);

            _clientFactoryMock
                .Setup(x => x.InitialiseClient(configuration, It.IsAny<CosmosClientOptions>()))
                .Returns(clientMock.Object);

            _repository = new CosmosDbRepository(configuration, _clientFactoryMock.Object);
        }

        [Fact, IsUnit]
        public void QueryAsync_NullRequest_ThrowsException()
        {
            // Act
            Func<Task> func = async () => await _repository.QueryAsync<TestData>(null);

            // Assert
            func.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact, IsUnit]
        public async Task QueryAsync_ValidRequest_ReturnsExpectedData()
        {
            // Arrange
            var expectedQueryResult = new List<TestData> { new TestData(), new TestData() };
            var query = new CosmosQuery("select * from aCollection");
            SetUpItemQueryIteratorWithResult(expectedQueryResult);

            // Act
            var result = await _repository.QueryAsync<TestData>(query);

            // Assert
            result.Should().BeEquivalentTo(expectedQueryResult);
        }

        [Fact, IsUnit]
        public async Task QueryAsync_ValidRequestWithPartitionKey_ReturnsExpectedData()
        {
            // Arrange
            var expectedQueryResult = new List<TestData> { new TestData(), new TestData() };
            var query = new CosmosQuery("select * from aCollection", "partition-key");
            SetUpItemQueryIteratorWithResult(expectedQueryResult);

            // Act
            var result = await _repository.QueryAsync<TestData>(query);

            // Assert
            result.Should().BeEquivalentTo(expectedQueryResult);
        }

        [Fact, IsUnit]
        public async Task QueryAsync_QueryDefinitionProvided_CanQuerySuccessfully()
        {
            // Arrange
            var expectedQueryResult = new List<TestData> { new TestData(), new TestData() };
            var queryDefinition = new QueryDefinition("select * from aCollection");
            SetUpItemQueryIteratorWithResult(expectedQueryResult);

            // Act
            var result = await _repository.QueryAsync<TestData>(queryDefinition, null);

            // Assert
            result.Should().BeEquivalentTo(expectedQueryResult);
        }

        [Fact, IsUnit]
        public async Task QueryAsync_QueryCannotFindDbOrCollection_ForcesClientInvalidation()
        {
            // Arrange
            var query = new CosmosQuery("select * from aCollection");

            var feedIteratorMock = new Mock<FeedIterator<TestData>>();
            feedIteratorMock.Setup(x => x.HasMoreResults).Returns(false);

            _containerMock
                .SetupSequence(x => x.GetItemQueryIterator<TestData>(query.QueryDefinition, null, null))
                .Throws(new CosmosException(
                    "Some message and ResourceType: Collection",
                    HttpStatusCode.NotFound,
                    default,
                    default,
                    default))
                .Returns(feedIteratorMock.Object);

            // Act
            await _repository.QueryAsync<TestData>(query);

            // Assert
            _clientFactoryMock.Verify(f => f.Invalidate(), Times.Once);
        }

        [Fact, IsUnit]
        public async Task QueryWithContainerAsync_QueriesWithCosmosQuery_ReturnsDataWithETag()
        {
            // Arrange
            dynamic expectedQueryResult = new { Id = "id", Pk = "pk", _etag = "abc" };
            var testList = new List<dynamic> { expectedQueryResult, expectedQueryResult };

            var query = new CosmosQuery("select * from aCollection");
            SetUpItemQueryIteratorWithResult(testList);

            // Act
            var result = await _repository.QueryWithContainerAsync<TestData>(query);

            // Assert
            var expected = new[]
            {
                new DocumentContainer<TestData>(new TestData { Id = "id", Pk = "pk" }, "abc"),
                new DocumentContainer<TestData>(new TestData { Id = "id", Pk = "pk" }, "abc"),
            };

            result.Should().BeEquivalentTo(
                expected,
                opt => opt.ComparingByMembers<DocumentContainer<TestData>>());
        }

        [Fact, IsUnit]
        public void QueryWithContainerAsync_QueriesWithCosmosQueryNoEtag_ThrowsException()
        {
            // Arrange
            dynamic expectedQueryResult = new { Id = "id", Pk = "pk" };
            var testList = new List<dynamic> { expectedQueryResult, expectedQueryResult };

            var query = new CosmosQuery("select * from aCollection");
            SetUpItemQueryIteratorWithResult(testList);

            // Act
            Func<Task> func = async () =>
            {
                var result = await _repository.QueryWithContainerAsync<TestData>(query);
                var _ = result.ToArray();
            };

            // Assert
            func.Should().ThrowExactly<ArgumentException>()
                .Which.Message.Should().Be("Provided query does not return eTag information");
        }

        [Fact]
        [IsUnit]
        public async Task QueryAsync_ItemDoesNotExist_ThorwsException()
        {
            // Arrange
            var query = new CosmosQuery("select * from c where c.id == 'abc'");

            _containerMock
                .Setup(x => x.GetItemQueryIterator<TestData>(query.QueryDefinition, null, null))
                .Throws(new CosmosException(
                    default,
                    HttpStatusCode.NotFound,
                    default,
                    default,
                    default));

            // Act
            Func<Task> act = () => _repository.QueryAsync<TestData>(query);

            // Assert
            await act.Should().ThrowAsync<MissingDocumentException>();
        }

        [Fact]
        [IsUnit]
        public async Task QueryAsync_StaleData_ThrowsException()
        {
            // Arrange
            var query = new CosmosQuery("select * from aCollection");

            _containerMock
                .Setup(x => x.GetItemQueryIterator<TestData>(query.QueryDefinition, null, null))
                .Throws(new CosmosException(
                    default,
                    HttpStatusCode.PreconditionFailed,
                    default,
                    default,
                    default));

            // Act
            Func<Task> act = () => _repository.QueryAsync<TestData>(query);

            // Assert
            await act.Should().ThrowAsync<StaleDataException>();
        }

        private void SetUpItemQueryIteratorWithResult<T>(List<T> testList)
        {
            var feedIteratorMock = new Mock<FeedIterator<T>>();
            _containerMock
                .Setup(x => x.GetItemQueryIterator<T>(It.IsAny<QueryDefinition>(), null, It.IsAny<QueryRequestOptions>()))
                .Returns(feedIteratorMock.Object);

            var feedResponseMock = new Mock<FeedResponse<T>>();
            feedResponseMock
                .Setup(x => x.GetEnumerator())
                .Returns(testList.GetEnumerator());

            feedIteratorMock
                .SetupSequence(x => x.HasMoreResults)
                .Returns(true)
                .Returns(false);

            feedIteratorMock
                .Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(feedResponseMock.Object);
        }
    }
}