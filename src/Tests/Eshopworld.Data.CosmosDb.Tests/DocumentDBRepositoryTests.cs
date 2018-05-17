using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Eshopworld.Data.CosmosDb.Tests
{
    public class DocumentDBRepositoryTests
    {
        public DocumentDBRepositoryTests()
        {
            var endPoint = Environment.GetEnvironmentVariable("endPoint");
            var authKey = Environment.GetEnvironmentVariable("authKey");
            var databaseId = Environment.GetEnvironmentVariable("databaseId");
            var collectionId = Environment.GetEnvironmentVariable("collectionId");

            _documentDBRepository = new DocumentDBRepository<Dummy>();
            _documentDBRepository.Initialize(endPoint, authKey, databaseId, collectionId);
        }

        public void Dispose()
        {
            _documentDBRepository.Client?.Dispose();
        }

        private readonly DocumentDBRepository<Dummy> _documentDBRepository;

        [Fact]
        public void ItemIsCreated()
        {
            var id = Guid.NewGuid();
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY01"
            }).Wait();

            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Equal(id, item.Id);
            Assert.Equal("DUMMY01", item.Name);
            _documentDBRepository.DeleteItemAsync(id.ToString()).Wait();
        }

        [Fact]
        public async Task ItemIsDeleted()
        {
            var id = Guid.Parse("85eabd4c-3fd6-4f23-868c-2ad3621ea332");
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY03"
            }).Wait();

            var response = await _documentDBRepository.DeleteItemAsync(id.ToString());

            Assert.True(response);
            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Null(item);
        }

        [Fact]
        public async Task NonExistingItemIsNotDeleted()
        {
            var id = Guid.Parse("e5390dc0-d330-48e3-9fa1-36ab43286354");

            var response = await _documentDBRepository.DeleteItemAsync(id.ToString());

            Assert.False(response);
        }

        [Fact]
        public void ItemIsRetrieved()
        {
            var id = Guid.NewGuid();
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY02"
            }).Wait();

            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Equal(id, item.Id);
            Assert.Equal("DUMMY02", item.Name);
            _documentDBRepository.DeleteItemAsync(id.ToString()).Wait();
        }

        [Fact]
        public void ItemIsUpdated()
        {
            var id = Guid.NewGuid();
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY04"
            }).Wait();

            var updatedDummy = new Dummy { Id = id, Name = "UPDATED" };
            _documentDBRepository.UpdateItemAsync(id.ToString(), updatedDummy).Wait();

            var item = _documentDBRepository.GetItemAsync(id.ToString()).Result;
            Assert.Equal("UPDATED", item.Name);
            _documentDBRepository.DeleteItemAsync(id.ToString()).Wait();
        }

        [Fact]
        public async Task ShouldThrowForInvalidETag()
        {
            var id = Guid.Parse("6f04d6ae-6b03-44a9-837f-8c5141618060");
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = id,
                Name = "DUMMY04"
            }).Wait();

            var document = await _documentDBRepository.GetDocumentAsync(id.ToString());

            var updatedDummy = (Dummy)(dynamic)document;
            updatedDummy.Name = "UPDATED";

            // Using Access Conditions gives us the ability to use the ETag from our fetched document for optimistic concurrency.
            var ac = new AccessCondition { Condition = document.ETag, Type = AccessConditionType.IfMatch };

            // Update our document, which will succeed with the correct ETag 
            await _documentDBRepository.UpdateItemAsync(id.ToString(), updatedDummy, new RequestOptions { AccessCondition = ac });

            var item = await _documentDBRepository.GetItemAsync(id.ToString());
            Assert.Equal("UPDATED", item.Name);

            // Update our document again, which will fail since our (same) ETag is now invalid 
            var ex = await Assert.ThrowsAsync<DocumentClientException>(async () =>
            {
                await _documentDBRepository.UpdateItemAsync(id.ToString(), updatedDummy, new RequestOptions { AccessCondition = ac });
            });

            Assert.Equal(HttpStatusCode.PreconditionFailed, ex?.StatusCode);

            await _documentDBRepository.DeleteItemAsync(id.ToString());
        }

        [Fact]
        public void ItemsAreRetrieved()
        {
            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = Guid.NewGuid(),
                Name = "DUMMY05"
            }).Wait();

            _documentDBRepository.CreateItemAsync(new Dummy
            {
                Id = Guid.NewGuid(),
                Name = "DUMMY05"
            }).Wait();

            var items = _documentDBRepository.GetItemsAsync(d => d.Name == "DUMMY05").Result.ToList();
            Assert.Equal(2, items.Count);
            foreach (var item in items) _documentDBRepository.DeleteItemAsync(item.Id.ToString()).Wait();
        }
    }
}