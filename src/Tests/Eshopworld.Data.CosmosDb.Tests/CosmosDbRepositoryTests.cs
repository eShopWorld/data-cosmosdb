using Eshopworld.Core;
using Eshopworld.Tests.Core;
using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class CosmosDbRepositoryTests
    {
        private readonly CosmosDbRepository _repository;
        private readonly Mock<ItemResponse<TestData>> _responseMock = new Mock<ItemResponse<TestData>>();
        private readonly Mock<Container> _containerMock = new Mock<Container>();
        private readonly Mock<ICosmosDbClientFactory> _clientFactoryMock;
        private readonly IBigBrother _bigBrother = Mock.Of<IBigBrother>();
        private readonly CosmosDbConfiguration _configuration;

        public CosmosDbRepositoryTests()
        {
            _configuration = new CosmosDbConfiguration
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

            _clientFactoryMock = new Mock<ICosmosDbClientFactory>();
            _clientFactoryMock
                .Setup(x => x.InitialiseClient(_configuration, It.IsAny<CosmosClientOptions>()))
                .Returns(clientMock.Object);

            _repository = new CosmosDbRepository(_configuration, _clientFactoryMock.Object, _bigBrother);
        }

        [Fact, IsUnit]
        public void Constructor_SetupNull_ThrowsException()
        {
            // Act
            Func<CosmosDbRepository> func = () => new CosmosDbRepository(
                null,
                Mock.Of<ICosmosDbClientFactory>(),
                Mock.Of<IBigBrother>());

            // Assert
            func.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact, IsUnit]
        public void Constructor_FactoryNull_ThrowsException()
        {
            // Act
            Func<CosmosDbRepository> func = () => new CosmosDbRepository(
                _configuration,
                null,
                Mock.Of<IBigBrother>());

            // Assert
            func.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact, IsUnit]
        public void Constructor_BigBrotherNull_ThrowsException()
        {
            // Act
            Func<CosmosDbRepository> func = () => new CosmosDbRepository(
                _configuration,
                Mock.Of<ICosmosDbClientFactory>(),
                null);

            // Assert
            func.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact, IsUnit]
        public void WhenCosmosClientOptionsIsSet_ThenFactoryInitialiseClientIsCalled()
        {
            // Arrange
            var options = new CosmosClientOptions();

            // Act
            var cosmosRepository = new CosmosDbRepository(_configuration, _clientFactoryMock.Object, _bigBrother, options);
            var _ = cosmosRepository.DbContainer;

            // Assert
            _clientFactoryMock.Verify(x => x.InitialiseClient(It.IsAny<CosmosDbConfiguration>(), options));
        }

        [Fact, IsUnit]
        public void UseCollection_DatabaseNotConfigured_ThrowsException()
        {
            // Arrange
            var cosmosRepository = new CosmosDbRepository(_configuration, _clientFactoryMock.Object, _bigBrother, new CosmosClientOptions());

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
            var cosmosRepository = new CosmosDbRepository(config, _clientFactoryMock.Object, _bigBrother, new CosmosClientOptions());
            
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
            var cosmosRepository = new CosmosDbRepository(config, _clientFactoryMock.Object, _bigBrother, new CosmosClientOptions());

            // Act
            Action action = () => cosmosRepository.UseCollection("col1", "db1");

            // Assert
            action.Should().NotThrow();
        }

        [Fact, IsUnit]
        public async Task CreateAsync_ValidRequest_CreatesItem()
        {
            // Arrange
            var expectedCreateItem = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(expectedCreateItem);
            _containerMock
                .Setup(x => x.CreateItemAsync(expectedCreateItem, null, null, default))
                .ReturnsAsync(_responseMock.Object);

            // Act
            var result = await _repository.CreateAsync(expectedCreateItem);

            // Assert
            result.Document.Should().BeSameAs(expectedCreateItem);
        }

        [Fact, IsUnit]
        public async Task UpsertAsync_ValidRequest_UpsertsItem()
        {
            // Arrange
            var expectedUpsertItem = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(expectedUpsertItem);
            _containerMock
                .Setup(x => x.UpsertItemAsync(expectedUpsertItem, null, null, default))
                .ReturnsAsync(_responseMock.Object);

            // Act
            var result = await _repository.UpsertAsync(expectedUpsertItem);

            // Assert
            result.Document.Should().BeSameAs(expectedUpsertItem);
        }

        [Fact, IsUnit]
        public async Task ReplaceAsync_ValidRequest_ReplacesItem()
        {
            // Arrange
            var expectedReplaceItem = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(expectedReplaceItem);
            _containerMock
                .Setup(x => x.ReplaceItemAsync(expectedReplaceItem, expectedReplaceItem.Id, null, null, default))
                .ReturnsAsync(_responseMock.Object);

            // Act
            var result = await _repository.ReplaceAsync(expectedReplaceItem.Id, expectedReplaceItem);

            // Assert
            result.Document.Should().BeSameAs(expectedReplaceItem);
        }

        [Fact, IsUnit]
        public async Task DeleteAsync_ItemExists_ReturnsTrue()
        {
            // Arrange
            var expectedDeleteItem = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(expectedDeleteItem);
            _containerMock
                .Setup(x => x.DeleteItemAsync<TestData>(expectedDeleteItem.Id, new PartitionKey(expectedDeleteItem.Pk), null, default))
                .ReturnsAsync(_responseMock.Object);

            // Act
            var result = await _repository.DeleteAsync<TestData>(expectedDeleteItem.Id, expectedDeleteItem.Pk);

            // Assert
            result.Should().BeTrue("because the item exists and can be deleted");
        }

        [Fact, IsUnit]
        public async Task DeleteAsync_ItemDoesNotExist_ReturnsFalse()
        {
            // Arrange
            var expectedDeleteItem = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(expectedDeleteItem);
            _containerMock
                .Setup(x => x.DeleteItemAsync<TestData>(expectedDeleteItem.Id, new PartitionKey(expectedDeleteItem.Pk), null, default))
                .ThrowsAsync(new CosmosException(
                    default,
                    HttpStatusCode.NotFound,
                    default,
                    default,
                    default));

            // Act
            var result = await _repository.DeleteAsync<TestData>(expectedDeleteItem.Id, expectedDeleteItem.Pk);

            // Assert
            result.Should().BeFalse("because the item does not exist");
        }

        [Fact, IsUnit]
        public void DeleteAsync_ExceptionHappensWhichIsDifferentThanObjectNotFound_ThrowsException()
        {
            // Arrange
            var expectedDeleteItem = new TestData();
            _responseMock
                .SetupGet(x => x.Resource)
                .Returns(expectedDeleteItem);
            _containerMock
                .Setup(x => x.DeleteItemAsync<TestData>(expectedDeleteItem.Id, new PartitionKey(expectedDeleteItem.Pk), null, default))
                .ThrowsAsync(new CosmosException(default, HttpStatusCode.BadGateway, default, default, default))
                .Verifiable();

            // Act
            Func<Task> func = async () => await _repository.DeleteAsync<TestData>(expectedDeleteItem.Id, expectedDeleteItem.Pk);

            // Assert
            func.Should().ThrowExactly<CosmosException>().Which.StatusCode.Should().Be(HttpStatusCode.BadGateway);
        }
    }
}