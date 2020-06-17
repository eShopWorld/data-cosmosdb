using Eshopworld.Core;
using Eshopworld.Data.CosmosDb.Exceptions;
using Eshopworld.Tests.Core;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
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

        public CosmosDbRepositoryTests()
        {
            var bigBrother = Mock.Of<IBigBrother>();
            var configuration = new CosmosDbConfiguration
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

            var clientFactoryMock = new Mock<ICosmosDbClientFactory>();
            clientFactoryMock
                .Setup(x => x.InitialiseClient(configuration))
                .Returns(clientMock.Object)
                .Verifiable();

            _repository = new CosmosDbRepository(configuration, clientFactoryMock.Object, bigBrother);
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