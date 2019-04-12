using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Eshopworld.Data.CosmosDb.SessionHandling
{
    internal sealed class SessionProxyDocumentClient : IDocumentClient
    {
        private readonly IDocumentClient _documentClient;
        private readonly ICosmosDbSessionTokenProvider _sessionProvider;

        public SessionProxyDocumentClient(IDocumentClient documentClient, ICosmosDbSessionTokenProvider sessionProvider)
        {
            _documentClient = documentClient ?? throw new ArgumentNullException(nameof(documentClient));
            _sessionProvider = sessionProvider;
        }

        public object Session { get => _documentClient.Session; set => _documentClient.Session = value; }

        public Uri ServiceEndpoint => _documentClient.ServiceEndpoint;

        public Uri WriteEndpoint => _documentClient.WriteEndpoint;

        public Uri ReadEndpoint => _documentClient.ReadEndpoint;

        public ConnectionPolicy ConnectionPolicy => _documentClient.ConnectionPolicy;

        public SecureString AuthKey => _documentClient.AuthKey;

        public ConsistencyLevel ConsistencyLevel => _documentClient.ConsistencyLevel;

        private void PrepareInputSession(ref FeedOptions options)
        {
            var token = _sessionProvider.SessionToken;
            if (token != null)
            {
                if (options == null)
                    options = new FeedOptions();
                options.SessionToken = token;
            }
        }

        private void PrepareInputSession(ref RequestOptions options)
        {
            var token = _sessionProvider.SessionToken;
            if (token != null)
            {
                if (options == null)
                    options = new RequestOptions();
                options.SessionToken = token;
            }
        }

        private void PrepareOutputSession(ResourceResponseBase response)
        {
            _sessionProvider.SessionToken = response.SessionToken;
        }

        private void PrepareOutputSession<T>(FeedResponse<T> response)
        {
            _sessionProvider.SessionToken = response.SessionToken;
        }

        #region Generated
        public async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(string storedProcedureLink, RequestOptions options, CancellationToken cancellationToken, Object[] procedureParams)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ExecuteStoredProcedureAsync<TValue>(storedProcedureLink, options, cancellationToken, procedureParams);
            return returnValue;
        }

        public async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(Uri storedProcedureUri, RequestOptions options, CancellationToken cancellationToken, Object[] procedureParams)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri, options, cancellationToken, procedureParams);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(string documentLink, Object attachment, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertAttachmentAsync(documentLink, attachment, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(string attachmentsLink, Stream mediaStream, MediaOptions options, RequestOptions requestOptions, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref requestOptions);
            var returnValue = await _documentClient.UpsertAttachmentAsync(attachmentsLink, mediaStream, options, requestOptions, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(Uri documentUri, Object attachment, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertAttachmentAsync(documentUri, attachment, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> UpsertAttachmentAsync(Uri documentUri, Stream mediaStream, MediaOptions options, RequestOptions requestOptions, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref requestOptions);
            var returnValue = await _documentClient.UpsertAttachmentAsync(documentUri, mediaStream, options, requestOptions, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> UpsertDocumentAsync(string collectionLink, Object document, RequestOptions options, bool disableAutomaticIdGeneration, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertDocumentAsync(collectionLink, document, options, disableAutomaticIdGeneration, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> UpsertDocumentAsync(Uri documentCollectionUri, Object document, RequestOptions options, bool disableAutomaticIdGeneration, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertDocumentAsync(documentCollectionUri, document, options, disableAutomaticIdGeneration, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(string collectionLink, StoredProcedure storedProcedure, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertStoredProcedureAsync(collectionLink, storedProcedure, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> UpsertStoredProcedureAsync(Uri documentCollectionUri, StoredProcedure storedProcedure, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertStoredProcedureAsync(documentCollectionUri, storedProcedure, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> UpsertTriggerAsync(string collectionLink, Trigger trigger, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertTriggerAsync(collectionLink, trigger, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> UpsertTriggerAsync(Uri documentCollectionUri, Trigger trigger, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertTriggerAsync(documentCollectionUri, trigger, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(string collectionLink, UserDefinedFunction function, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertUserDefinedFunctionAsync(collectionLink, function, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> UpsertUserDefinedFunctionAsync(Uri documentCollectionUri, UserDefinedFunction function, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertUserDefinedFunctionAsync(documentCollectionUri, function, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> UpsertPermissionAsync(string userLink, Permission permission, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertPermissionAsync(userLink, permission, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> UpsertPermissionAsync(Uri userUri, Permission permission, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertPermissionAsync(userUri, permission, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> UpsertUserAsync(string databaseLink, User user, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertUserAsync(databaseLink, user, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> UpsertUserAsync(Uri databaseUri, User user, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.UpsertUserAsync(databaseUri, user, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public IOrderedQueryable<T> CreateAttachmentQuery<T>(Uri documentUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery<T>(documentUri, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateAttachmentQuery<T>(Uri documentUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery<T>(documentUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateAttachmentQuery<T>(Uri documentUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery<T>(documentUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Attachment> CreateAttachmentQuery(Uri documentUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery(documentUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateAttachmentQuery(Uri documentUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery(documentUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateAttachmentQuery(Uri documentUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery(documentUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<DocumentCollection> CreateDocumentCollectionQuery(Uri databaseUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentCollectionQuery(databaseUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentCollectionQuery(Uri databaseUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentCollectionQuery(databaseUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentCollectionQuery(Uri databaseUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentCollectionQuery(databaseUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<StoredProcedure> CreateStoredProcedureQuery(Uri documentCollectionUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateStoredProcedureQuery(documentCollectionUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateStoredProcedureQuery(Uri documentCollectionUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateStoredProcedureQuery(documentCollectionUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateStoredProcedureQuery(Uri documentCollectionUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateStoredProcedureQuery(documentCollectionUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Trigger> CreateTriggerQuery(Uri documentCollectionUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateTriggerQuery(documentCollectionUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateTriggerQuery(Uri documentCollectionUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateTriggerQuery(documentCollectionUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateTriggerQuery(Uri documentCollectionUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateTriggerQuery(documentCollectionUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<UserDefinedFunction> CreateUserDefinedFunctionQuery(Uri documentCollectionUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserDefinedFunctionQuery(documentCollectionUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserDefinedFunctionQuery(Uri documentCollectionUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserDefinedFunctionQuery(documentCollectionUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserDefinedFunctionQuery(Uri documentCollectionUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserDefinedFunctionQuery(documentCollectionUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Conflict> CreateConflictQuery(Uri documentCollectionUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateConflictQuery(documentCollectionUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateConflictQuery(Uri documentCollectionUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateConflictQuery(documentCollectionUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateConflictQuery(Uri documentCollectionUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateConflictQuery(documentCollectionUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<T> CreateDocumentQuery<T>(Uri documentCollectionUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery<T>(documentCollectionUri, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateDocumentQuery<T>(Uri documentCollectionUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery<T>(documentCollectionUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateDocumentQuery<T>(Uri documentCollectionUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery<T>(documentCollectionUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Document> CreateDocumentQuery(Uri documentCollectionUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery(documentCollectionUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentQuery(Uri documentCollectionUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery(documentCollectionUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentQuery(Uri documentCollectionUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery(documentCollectionUri, querySpec, feedOptions);
            return returnValue;
        }

        public IDocumentQuery<Document> CreateDocumentChangeFeedQuery(Uri collectionLink, ChangeFeedOptions feedOptions)
        {
            var returnValue = _documentClient.CreateDocumentChangeFeedQuery(collectionLink, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<User> CreateUserQuery(Uri documentCollectionUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserQuery(documentCollectionUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserQuery(Uri documentCollectionUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserQuery(documentCollectionUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserQuery(Uri documentCollectionUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserQuery(documentCollectionUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Permission> CreatePermissionQuery(Uri userUri, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreatePermissionQuery(userUri, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreatePermissionQuery(Uri userUri, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreatePermissionQuery(userUri, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreatePermissionQuery(Uri userUri, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreatePermissionQuery(userUri, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<T> CreateAttachmentQuery<T>(string documentLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery<T>(documentLink, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateAttachmentQuery<T>(string documentLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery<T>(documentLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateAttachmentQuery<T>(string documentLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery<T>(documentLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Attachment> CreateAttachmentQuery(string documentLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery(documentLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateAttachmentQuery(string documentLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery(documentLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateAttachmentQuery(string documentLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateAttachmentQuery(documentLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Database> CreateDatabaseQuery(FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDatabaseQuery(feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDatabaseQuery(string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDatabaseQuery(sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDatabaseQuery(SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDatabaseQuery(querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<DocumentCollection> CreateDocumentCollectionQuery(string databaseLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentCollectionQuery(databaseLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentCollectionQuery(string databaseLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentCollectionQuery(databaseLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentCollectionQuery(string databaseLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentCollectionQuery(databaseLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<StoredProcedure> CreateStoredProcedureQuery(string collectionLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateStoredProcedureQuery(collectionLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateStoredProcedureQuery(string collectionLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateStoredProcedureQuery(collectionLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateStoredProcedureQuery(string collectionLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateStoredProcedureQuery(collectionLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Trigger> CreateTriggerQuery(string collectionLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateTriggerQuery(collectionLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateTriggerQuery(string collectionLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateTriggerQuery(collectionLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateTriggerQuery(string collectionLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateTriggerQuery(collectionLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<UserDefinedFunction> CreateUserDefinedFunctionQuery(string collectionLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserDefinedFunctionQuery(collectionLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserDefinedFunctionQuery(string collectionLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserDefinedFunctionQuery(collectionLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserDefinedFunctionQuery(string collectionLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserDefinedFunctionQuery(collectionLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Conflict> CreateConflictQuery(string collectionLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateConflictQuery(collectionLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateConflictQuery(string collectionLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateConflictQuery(collectionLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateConflictQuery(string collectionLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateConflictQuery(collectionLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<T> CreateDocumentQuery<T>(string collectionLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery<T>(collectionLink, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateDocumentQuery<T>(string collectionLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery<T>(collectionLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<T> CreateDocumentQuery<T>(string collectionLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery<T>(collectionLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Document> CreateDocumentQuery(string collectionLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery(collectionLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentQuery(string collectionLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery(collectionLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateDocumentQuery(string collectionLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateDocumentQuery(collectionLink, querySpec, feedOptions);
            return returnValue;
        }

        public IDocumentQuery<Document> CreateDocumentChangeFeedQuery(string collectionLink, ChangeFeedOptions feedOptions)
        {
            var returnValue = _documentClient.CreateDocumentChangeFeedQuery(collectionLink, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<User> CreateUserQuery(string usersLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserQuery(usersLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserQuery(string usersLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserQuery(usersLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateUserQuery(string usersLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateUserQuery(usersLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Permission> CreatePermissionQuery(string permissionsLink, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreatePermissionQuery(permissionsLink, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreatePermissionQuery(string permissionsLink, string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreatePermissionQuery(permissionsLink, sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreatePermissionQuery(string permissionsLink, SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreatePermissionQuery(permissionsLink, querySpec, feedOptions);
            return returnValue;
        }

        public IOrderedQueryable<Offer> CreateOfferQuery(FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateOfferQuery(feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateOfferQuery(string sqlExpression, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateOfferQuery(sqlExpression, feedOptions);
            return returnValue;
        }

        public IQueryable<Object> CreateOfferQuery(SqlQuerySpec querySpec, FeedOptions feedOptions)
        {
            PrepareInputSession(ref feedOptions);
            var returnValue = _documentClient.CreateOfferQuery(querySpec, feedOptions);
            return returnValue;
        }

        public async Task<DatabaseAccount> GetDatabaseAccountAsync()
        {
            var returnValue = await _documentClient.GetDatabaseAccountAsync();
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> CreateAttachmentAsync(string documentLink, Object attachment, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateAttachmentAsync(documentLink, attachment, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> CreateAttachmentAsync(string attachmentsLink, Stream mediaStream, MediaOptions options, RequestOptions requestOptions, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref requestOptions);
            var returnValue = await _documentClient.CreateAttachmentAsync(attachmentsLink, mediaStream, options, requestOptions, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> CreateAttachmentAsync(Uri documentUri, Object attachment, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateAttachmentAsync(documentUri, attachment, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> CreateAttachmentAsync(Uri documentUri, Stream mediaStream, MediaOptions options, RequestOptions requestOptions, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref requestOptions);
            var returnValue = await _documentClient.CreateAttachmentAsync(documentUri, mediaStream, options, requestOptions, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Database>> CreateDatabaseAsync(Database database, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDatabaseAsync(database, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Database>> CreateDatabaseIfNotExistsAsync(Database database, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDatabaseIfNotExistsAsync(database, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(string databaseLink, DocumentCollection documentCollection, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDocumentCollectionAsync(databaseLink, documentCollection, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(string databaseLink, DocumentCollection documentCollection, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseLink, documentCollection, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionAsync(Uri databaseUri, DocumentCollection documentCollection, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDocumentCollectionAsync(databaseUri, documentCollection, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> CreateDocumentCollectionIfNotExistsAsync(Uri databaseUri, DocumentCollection documentCollection, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDocumentCollectionIfNotExistsAsync(databaseUri, documentCollection, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> CreateDocumentAsync(string collectionLink, Object document, RequestOptions options, bool disableAutomaticIdGeneration, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDocumentAsync(collectionLink, document, options, disableAutomaticIdGeneration, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> CreateDocumentAsync(Uri documentCollectionUri, Object document, RequestOptions options, bool disableAutomaticIdGeneration, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateDocumentAsync(documentCollectionUri, document, options, disableAutomaticIdGeneration, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(string collectionLink, StoredProcedure storedProcedure, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateStoredProcedureAsync(collectionLink, storedProcedure, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> CreateStoredProcedureAsync(Uri documentCollectionUri, StoredProcedure storedProcedure, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateStoredProcedureAsync(documentCollectionUri, storedProcedure, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> CreateTriggerAsync(string collectionLink, Trigger trigger, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateTriggerAsync(collectionLink, trigger, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> CreateTriggerAsync(Uri documentCollectionUri, Trigger trigger, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateTriggerAsync(documentCollectionUri, trigger, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(string collectionLink, UserDefinedFunction function, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateUserDefinedFunctionAsync(collectionLink, function, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> CreateUserDefinedFunctionAsync(Uri documentCollectionUri, UserDefinedFunction function, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateUserDefinedFunctionAsync(documentCollectionUri, function, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> CreateUserAsync(string databaseLink, User user, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateUserAsync(databaseLink, user, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> CreateUserAsync(Uri databaseUri, User user, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreateUserAsync(databaseUri, user, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> CreatePermissionAsync(string userLink, Permission permission, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreatePermissionAsync(userLink, permission, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> CreatePermissionAsync(Uri userUri, Permission permission, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.CreatePermissionAsync(userUri, permission, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> DeleteAttachmentAsync(string attachmentLink, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteAttachmentAsync(attachmentLink, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> DeleteAttachmentAsync(Uri attachmentUri, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteAttachmentAsync(attachmentUri, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Database>> DeleteDatabaseAsync(string databaseLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteDatabaseAsync(databaseLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Database>> DeleteDatabaseAsync(Uri databaseUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteDatabaseAsync(databaseUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(string documentCollectionLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteDocumentCollectionAsync(documentCollectionLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> DeleteDocumentCollectionAsync(Uri documentCollectionUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteDocumentCollectionAsync(documentCollectionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> DeleteDocumentAsync(string documentLink, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteDocumentAsync(documentLink, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> DeleteDocumentAsync(Uri documentUri, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteDocumentAsync(documentUri, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(string storedProcedureLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteStoredProcedureAsync(storedProcedureLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> DeleteStoredProcedureAsync(Uri storedProcedureUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteStoredProcedureAsync(storedProcedureUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> DeleteTriggerAsync(string triggerLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteTriggerAsync(triggerLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> DeleteTriggerAsync(Uri triggerUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteTriggerAsync(triggerUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(string functionLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteUserDefinedFunctionAsync(functionLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> DeleteUserDefinedFunctionAsync(Uri functionUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteUserDefinedFunctionAsync(functionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> DeleteUserAsync(string userLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteUserAsync(userLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> DeleteUserAsync(Uri userUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteUserAsync(userUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> DeletePermissionAsync(string permissionLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeletePermissionAsync(permissionLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> DeletePermissionAsync(Uri permissionUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeletePermissionAsync(permissionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Conflict>> DeleteConflictAsync(string conflictLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteConflictAsync(conflictLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Conflict>> DeleteConflictAsync(Uri conflictUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.DeleteConflictAsync(conflictUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(Attachment attachment, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceAttachmentAsync(attachment, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> ReplaceAttachmentAsync(Uri attachmentUri, Attachment attachment, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceAttachmentAsync(attachmentUri, attachment, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(DocumentCollection documentCollection, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceDocumentCollectionAsync(documentCollection, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> ReplaceDocumentCollectionAsync(Uri documentCollectionUri, DocumentCollection documentCollection, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceDocumentCollectionAsync(documentCollectionUri, documentCollection, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> ReplaceDocumentAsync(string documentLink, Object document, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceDocumentAsync(documentLink, document, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> ReplaceDocumentAsync(Uri documentUri, Object document, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceDocumentAsync(documentUri, document, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> ReplaceDocumentAsync(Document document, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceDocumentAsync(document, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(StoredProcedure storedProcedure, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceStoredProcedureAsync(storedProcedure, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> ReplaceStoredProcedureAsync(Uri storedProcedureUri, StoredProcedure storedProcedure, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceStoredProcedureAsync(storedProcedureUri, storedProcedure, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(Trigger trigger, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceTriggerAsync(trigger, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> ReplaceTriggerAsync(Uri triggerUri, Trigger trigger, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceTriggerAsync(triggerUri, trigger, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(UserDefinedFunction function, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceUserDefinedFunctionAsync(function, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> ReplaceUserDefinedFunctionAsync(Uri userDefinedFunctionUri, UserDefinedFunction function, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceUserDefinedFunctionAsync(userDefinedFunctionUri, function, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> ReplacePermissionAsync(Permission permission, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplacePermissionAsync(permission, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> ReplacePermissionAsync(Uri permissionUri, Permission permission, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplacePermissionAsync(permissionUri, permission, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> ReplaceUserAsync(User user, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceUserAsync(user, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> ReplaceUserAsync(Uri userUri, User user, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReplaceUserAsync(userUri, user, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Offer>> ReplaceOfferAsync(Offer offer)
        {
            var returnValue = await _documentClient.ReplaceOfferAsync(offer);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<MediaResponse> UpdateMediaAsync(string mediaLink, Stream mediaStream, MediaOptions options, CancellationToken cancellationToken)
        {
            var returnValue = await _documentClient.UpdateMediaAsync(mediaLink, mediaStream, options, cancellationToken);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> ReadAttachmentAsync(string attachmentLink, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadAttachmentAsync(attachmentLink, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Attachment>> ReadAttachmentAsync(Uri attachmentUri, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadAttachmentAsync(attachmentUri, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Database>> ReadDatabaseAsync(string databaseLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDatabaseAsync(databaseLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Database>> ReadDatabaseAsync(Uri databaseUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDatabaseAsync(databaseUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(string documentCollectionLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentCollectionAsync(documentCollectionLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<DocumentCollection>> ReadDocumentCollectionAsync(Uri documentCollectionUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentCollectionAsync(documentCollectionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> ReadDocumentAsync(string documentLink, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentAsync(documentLink, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Document>> ReadDocumentAsync(Uri documentUri, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentAsync(documentUri, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<DocumentResponse<T>> ReadDocumentAsync<T>(string documentLink, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentAsync<T>(documentLink, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<DocumentResponse<T>> ReadDocumentAsync<T>(Uri documentUri, RequestOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentAsync<T>(documentUri, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(string storedProcedureLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadStoredProcedureAsync(storedProcedureLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<StoredProcedure>> ReadStoredProcedureAsync(Uri storedProcedureUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadStoredProcedureAsync(storedProcedureUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> ReadTriggerAsync(string triggerLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadTriggerAsync(triggerLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Trigger>> ReadTriggerAsync(Uri triggerUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadTriggerAsync(triggerUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(string functionLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserDefinedFunctionAsync(functionLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<UserDefinedFunction>> ReadUserDefinedFunctionAsync(Uri functionUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserDefinedFunctionAsync(functionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> ReadPermissionAsync(string permissionLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadPermissionAsync(permissionLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Permission>> ReadPermissionAsync(Uri permissionUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadPermissionAsync(permissionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> ReadUserAsync(string userLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserAsync(userLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<User>> ReadUserAsync(Uri userUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserAsync(userUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Conflict>> ReadConflictAsync(string conflictLink, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadConflictAsync(conflictLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Conflict>> ReadConflictAsync(Uri conflictUri, RequestOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadConflictAsync(conflictUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<ResourceResponse<Offer>> ReadOfferAsync(string offerLink)
        {
            var returnValue = await _documentClient.ReadOfferAsync(offerLink);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<MediaResponse> ReadMediaMetadataAsync(string mediaLink)
        {
            var returnValue = await _documentClient.ReadMediaMetadataAsync(mediaLink);
            return returnValue;
        }

        public async Task<MediaResponse> ReadMediaAsync(string mediaLink, CancellationToken cancellationToken)
        {
            var returnValue = await _documentClient.ReadMediaAsync(mediaLink, cancellationToken);
            return returnValue;
        }

        public async Task<FeedResponse<Attachment>> ReadAttachmentFeedAsync(string attachmentsLink, FeedOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadAttachmentFeedAsync(attachmentsLink, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Attachment>> ReadAttachmentFeedAsync(Uri documentUri, FeedOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadAttachmentFeedAsync(documentUri, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Database>> ReadDatabaseFeedAsync(FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDatabaseFeedAsync(options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(string partitionKeyRangesOrCollectionLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadPartitionKeyRangeFeedAsync(partitionKeyRangesOrCollectionLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<PartitionKeyRange>> ReadPartitionKeyRangeFeedAsync(Uri partitionKeyRangesOrCollectionUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadPartitionKeyRangeFeedAsync(partitionKeyRangesOrCollectionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(string collectionsLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentCollectionFeedAsync(collectionsLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<DocumentCollection>> ReadDocumentCollectionFeedAsync(Uri databaseUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentCollectionFeedAsync(databaseUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Object>> ReadDocumentFeedAsync(string documentsLink, FeedOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentFeedAsync(documentsLink, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Object>> ReadDocumentFeedAsync(Uri documentCollectionUri, FeedOptions options, CancellationToken cancellationToken)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadDocumentFeedAsync(documentCollectionUri, options, cancellationToken);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(string storedProceduresLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadStoredProcedureFeedAsync(storedProceduresLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<StoredProcedure>> ReadStoredProcedureFeedAsync(Uri documentCollectionUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadStoredProcedureFeedAsync(documentCollectionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Trigger>> ReadTriggerFeedAsync(string triggersLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadTriggerFeedAsync(triggersLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Trigger>> ReadTriggerFeedAsync(Uri documentCollectionUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadTriggerFeedAsync(documentCollectionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(string userDefinedFunctionsLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserDefinedFunctionFeedAsync(userDefinedFunctionsLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<UserDefinedFunction>> ReadUserDefinedFunctionFeedAsync(Uri documentCollectionUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserDefinedFunctionFeedAsync(documentCollectionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Permission>> ReadPermissionFeedAsync(string permissionsLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadPermissionFeedAsync(permissionsLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Permission>> ReadPermissionFeedAsync(Uri userUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadPermissionFeedAsync(userUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<User>> ReadUserFeedAsync(string usersLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserFeedAsync(usersLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<User>> ReadUserFeedAsync(Uri databaseUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadUserFeedAsync(databaseUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Conflict>> ReadConflictFeedAsync(string conflictsLink, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadConflictFeedAsync(conflictsLink, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Conflict>> ReadConflictFeedAsync(Uri documentCollectionUri, FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadConflictFeedAsync(documentCollectionUri, options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<FeedResponse<Offer>> ReadOffersFeedAsync(FeedOptions options)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ReadOffersFeedAsync(options);
            PrepareOutputSession(returnValue);
            return returnValue;
        }

        public async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(string storedProcedureLink, Object[] procedureParams)
        {
            var returnValue = await _documentClient.ExecuteStoredProcedureAsync<TValue>(storedProcedureLink, procedureParams);
            return returnValue;
        }

        public async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(Uri storedProcedureUri, Object[] procedureParams)
        {
            var returnValue = await _documentClient.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri, procedureParams);
            return returnValue;
        }

        public async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(string storedProcedureLink, RequestOptions options, Object[] procedureParams)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ExecuteStoredProcedureAsync<TValue>(storedProcedureLink, options, procedureParams);
            return returnValue;
        }

        public async Task<StoredProcedureResponse<TValue>> ExecuteStoredProcedureAsync<TValue>(Uri storedProcedureUri, RequestOptions options, Object[] procedureParams)
        {
            PrepareInputSession(ref options);
            var returnValue = await _documentClient.ExecuteStoredProcedureAsync<TValue>(storedProcedureUri, options, procedureParams);
            return returnValue;
        }

        #endregion
    }
}
