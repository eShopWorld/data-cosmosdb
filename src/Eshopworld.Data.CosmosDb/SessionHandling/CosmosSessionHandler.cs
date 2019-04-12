using System.Net.Http;
using System.Threading;

namespace Eshopworld.Data.CosmosDb.SessionHandling
{
    internal class CosmosSessionHandler : MessageProcessingHandler
    {
        private const string SessionTokenHeaderName = "x-ms-session-token";
        private readonly ICosmosDbSessionTokenProvider _sessionTokenProvider;

        public CosmosSessionHandler(ICosmosDbSessionTokenProvider sessionTokenProvider)
        {
            _sessionTokenProvider = sessionTokenProvider;
        }

        protected override HttpRequestMessage ProcessRequest(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var sessionToken = _sessionTokenProvider.SessionToken;
            if (sessionToken != null && !request.Headers.Contains(SessionTokenHeaderName))
                request.Headers.Add(SessionTokenHeaderName, sessionToken);
            return request;
        }

        protected override HttpResponseMessage ProcessResponse(HttpResponseMessage response, CancellationToken cancellationToken)
        {
            if (response.Headers.TryGetValues(SessionTokenHeaderName, out var values))
            {
                var sessionToken = values.FirstOrDefault();
                if (sessionToken != null)
                    _sessionTokenProvider.SessionToken = sessionToken;
            }

            return response;
        }
    }
