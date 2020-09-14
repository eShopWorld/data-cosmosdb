using Eshopworld.Core;
using Eshopworld.Data.CosmosDb.Exceptions;
using Eshopworld.Tests.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class CosmosDbRepositoryTests
    {
        private readonly CosmosDbRepository _repository;
        private readonly Mock<ItemResponse<TestData>> _responseMock;
        private readonly Mock<Container> _containerMock;
        private readonly Mock<ICosmosDbClientFactory> _clientFactoryMock;
        private readonly Mock<IBigBrother> _bigBrotherMock;
        private readonly CosmosDbConfiguration _configuration;

        public CosmosDbRepositoryTests()
        {
            _bigBrotherMock = new Mock<IBigBrother>();
            _configuration = new CosmosDbConfiguration
            {
                DatabaseEndpoint = string.Empty,
                DatabaseKey = string.Empty,
                Databases = new Dictionary<string, CosmosDbCollectionSettings[]>()
                {
                    [string.Empty] = new CosmosDbCollectionSettings[]
                    {
                        new CosmosDbCollectionSettings(string.Empty)
                    }
                }
            };

            _responseMock = new Mock<ItemResponse<TestData>>();

            _containerMock = new Mock<Container>();

            var clientMock = new Mock<CosmosClient>();
            clientMock
                .Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(_containerMock.Object)
                .Verifiable();

            _clientFactoryMock = new Mock<ICosmosDbClientFactory>();
            _clientFactoryMock
                .Setup(x => x.InitialiseClient(_configuration, It.IsAny<CosmosClientOptions>()))
                .Returns(clientMock.Object)
                .Verifiable();

            _repository = new CosmosDbRepository(_configuration, _clientFactoryMock.Object, _bigBrotherMock.Object);
        }

        [Fact]
        [IsUnit]
        public void WhenCosmosClientOptionsIsSet_ThenFactoryInitialiseClientIsCalled()
        {
            // Arrange
            var options = new CosmosClientOptions();

            // Act
            var cosmosRepository = new CosmosDbRepository(_configuration, _clientFactoryMock.Object, _bigBrotherMock.Object, options);
            var dbContainer = cosmosRepository.DbContainer;

            // Assert
            _clientFactoryMock.Verify(x => x.InitialiseClient(It.IsAny<CosmosDbConfiguration>(), options));
        }

        [Fact, IsUnit]
        public void UseCollection_DatabaseNotConfigured_ThrowsException()
        {
            // Arrange
            var cosmosRepository = new CosmosDbRepository(_configuration, _clientFactoryMock.Object, _bigBrotherMock.Object, new CosmosClientOptions());
            
            // Act
            Action action = () => cosmosRepository.UseCollection("abc", "db1");

            // Assert
            action.Should().ThrowExactly<ArgumentException>().Which.Message.Should()
                .Be("The database id 'db1' is not configured");
        }

        [Fact, IsUnit]
        public void UseCollection_CollectionNotConfigured_ThrowsException()
        {
            // Arrange
            var config = new CosmosDbConfiguration
            {
                DatabaseEndpoint = string.Empty,
                DatabaseKey = string.Empty,
                Databases = new Dictionary<string, CosmosDbCollectionSettings[]>
                {
                    ["db1"] = new[] { new CosmosDbCollectionSettings(string.Empty) }
                }
            };
            var cosmosRepository = new CosmosDbRepository(config, _clientFactoryMock.Object, _bigBrotherMock.Object, new CosmosClientOptions());
            
            // Act
            Action action = () => cosmosRepository.UseCollection("col1", "db1");

            // Assert
            action.Should().ThrowExactly<ArgumentException>().Which.Message.Should()
                .Be("The collection 'col1' is not configured for 'db1' database");
        }

        [Fact, IsUnit]
        public void UseCollection_DatabaseAndCollectionConfigured_DoesNotThrow()
        {
            // Arrange
            var config = new CosmosDbConfiguration
            {
                DatabaseEndpoint = string.Empty,
                DatabaseKey = string.Empty,
                Databases = new Dictionary<string, CosmosDbCollectionSettings[]>
                {
                    ["db1"] = new[] { new CosmosDbCollectionSettings("col1") }
                }
            };
            var cosmosRepository = new CosmosDbRepository(config, _clientFactoryMock.Object, _bigBrotherMock.Object, new CosmosClientOptions());

            // Act
            Action action = () => cosmosRepository.UseCollection("col1", "db1");

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        [IsUnit]
        public async Task CanCreateItem()
        {
            // Arrange
            var testData = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.CreateItemAsync(testData, null, null, default))
                .ReturnsAsync(_responseMock.Object)
                .Verifiable();

            // Act
            var result = await _repository.CreateAsync(testData);

            // Assert
            using (new AssertionScope())
            {
                result.Document.Should().BeSameAs(testData);
                _containerMock.Verify();
            }
        }

        [Fact]
        [IsUnit]
        public async Task CanUpsertItem()
        {
            // Arrange
            var testData = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.UpsertItemAsync(testData, null, null, default))
                .ReturnsAsync(_responseMock.Object)
                .Verifiable();

            // Act
            var result = await _repository.UpsertAsync(testData);

            // Assert
            using (new AssertionScope())
            {
                result.Document.Should().BeSameAs(testData);
                _containerMock.Verify();
            }
        }

        [Fact]
        [IsUnit]
        public async Task CanReplaceItem()
        {
            // Arrange
            var testData = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.ReplaceItemAsync(testData, testData.Id, null, null, default))
                .ReturnsAsync(_responseMock.Object)
                .Verifiable();

            // Act
            var result = await _repository.ReplaceAsync(testData.Id, testData);

            // Assert
            using (new AssertionScope())
            {
                result.Document.Should().BeSameAs(testData);
                _containerMock.Verify();
            }
        }

        [Fact]
        [IsUnit]
        public async Task CanDeleteExistingItem()
        {
            // Arrange
            var testData = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.DeleteItemAsync<TestData>(testData.Id, new PartitionKey(testData.Pk), null, default))
                .ReturnsAsync(_responseMock.Object)
                .Verifiable();

            // Act
            var result = await _repository.DeleteAsync<TestData>(testData.Id, testData.Pk);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                _containerMock.Verify();
            }
        }

        [Fact]
        [IsUnit]
        public async Task CannotDeleteNonExistentItem()
        {
            // Arrange
            var testData = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.DeleteItemAsync<TestData>(testData.Id, new PartitionKey(testData.Pk), null, default))
                .ThrowsAsync(new CosmosException(default, HttpStatusCode.NotFound, default, default, default))
                .Verifiable();

            // Act
            var result = await _repository.DeleteAsync<TestData>(testData.Id, testData.Pk);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeFalse();
                _containerMock.Verify();
            }
        }

        [Fact, IsUnit]
        public void DeleteAsync_ExceptionHappensWhichIsDifferentThanObjectNotFound_ThrowsException()
        {
            // Arrange
            var testData = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.DeleteItemAsync<TestData>(testData.Id, new PartitionKey(testData.Pk), null, default))
                .ThrowsAsync(new CosmosException(default, HttpStatusCode.BadGateway, default, default, default))
                .Verifiable();

            // Act
            Func<Task> func = async () => await _repository.DeleteAsync<TestData>(testData.Id, testData.Pk);

            // Assert
            func.Should().ThrowExactly<CosmosException>().Which.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        }

        [Fact, IsUnit]
        public void QueryAsync_NullRequest_ThrowsException()
        {
            // Act
            Func<Task> func = async () => await _repository.QueryAsync<TestData>(null);

            // Assert
            func.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        [IsUnit]
        public async Task CanQueryItem()
        {
            // Arrange
            var testData = new TestData();
            var testList = new List<TestData> {testData, testData};

            var query = new CosmosQuery("select * from aCollection");
            var feedIteratorMock = new Mock<FeedIterator<TestData>>();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.GetItemQueryIterator<TestData>(query.QueryDefinition, null, null))
                .Returns(feedIteratorMock.Object)
                .Verifiable();

            var feedResponseMock = new Mock<FeedResponse<TestData>>();
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

            // Act
            var result = await _repository.QueryAsync<TestData>(query);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(testList);
                _containerMock.Verify();
            }
        }

        [Fact, IsUnit]
        public async Task QueryAsync_QueryDefinitionProvided_CanQuerySuccessfully()
        {
            // Arrange
            var testData = new TestData();
            var testList = new List<TestData> { testData, testData };

            var queryDefinition = new QueryDefinition("select * from aCollection");
            var feedIteratorMock = new Mock<FeedIterator<TestData>>();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.GetItemQueryIterator<TestData>(queryDefinition, null, null))
                .Returns(feedIteratorMock.Object)
                .Verifiable();

            var feedResponseMock = new Mock<FeedResponse<TestData>>();
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

            // Act
            var result = await _repository.QueryAsync<TestData>(queryDefinition, null);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeEquivalentTo(testList);
                _containerMock.Verify();
            }
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
            dynamic testData = new { Id = "id", Pk = "pk", _etag = "abc" };
            var testList = new List<dynamic> { testData, testData };

            var query = new CosmosQuery("select * from aCollection");
            var feedIteratorMock = new Mock<FeedIterator<dynamic>>();
            var responseMock = new Mock<ItemResponse<dynamic>>();
            responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.GetItemQueryIterator<dynamic>(query.QueryDefinition, null, null))
                .Returns(feedIteratorMock.Object)
                .Verifiable();

            var feedResponseMock = new Mock<FeedResponse<dynamic>>();
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
            dynamic testData = new { Id = "id", Pk = "pk" };
            var testList = new List<dynamic> { testData, testData };

            var query = new CosmosQuery("select * from aCollection");
            var feedIteratorMock = new Mock<FeedIterator<dynamic>>();
            var responseMock = new Mock<ItemResponse<dynamic>>();
            responseMock
                .SetupGet(x => x.Resource)
                .Returns(testData);
            _containerMock
                .Setup(x => x.GetItemQueryIterator<dynamic>(query.QueryDefinition, null, null))
                .Returns(feedIteratorMock.Object)
                .Verifiable();

            var feedResponseMock = new Mock<FeedResponse<dynamic>>();
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
        public async Task CannotQueryNonExistentItem()
        {
            // Arrange
            var query = new CosmosQuery("select * from aCollection");

            _containerMock
                .Setup(x => x.GetItemQueryIterator<TestData>(query.QueryDefinition, null, null))
                .Throws(new CosmosException(default, HttpStatusCode.NotFound, default, default, default))
                .Verifiable();

            // Act
            Func<Task> act = () => _repository.QueryAsync<TestData>(query);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<MissingDocumentException>();
                _containerMock.Verify();
            }
        }

        [Fact]
        [IsUnit]
        public async Task CannotQueryStaleData()
        {
            // Arrange
            var query = new CosmosQuery("select * from aCollection");

            _containerMock
                .Setup(x => x.GetItemQueryIterator<TestData>(query.QueryDefinition, null, null))
                .Throws(new CosmosException(default, HttpStatusCode.PreconditionFailed, default, default, default))
                .Verifiable();

            // Act
            Func<Task> act = () => _repository.QueryAsync<TestData>(query);

            // Assert
            using (new AssertionScope())
            {
                await act.Should().ThrowAsync<StaleDataException>();
                _containerMock.Verify();
            }
        }
    }

    public class TestData
    {
        public string Id { get; set; } = "my_id";
        public string Pk { get; set; } = "my_pk";
    }
}