using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace Eshopworld.Data.CosmosDb.SessionHandling
{
    class SessionTokenCosmosRequestHandler : CosmosRequestHandler
    {
        private readonly ICosmosDbSessionTokenProvider _sessionProvider;

        public SessionTokenCosmosRequestHandler(ICosmosDbSessionTokenProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public override async Task<CosmosResponseMessage> SendAsync(CosmosRequestMessage request, CancellationToken cancellationToken)
        {
            var sessionToken = _sessionProvider.SessionToken;
            if (sessionToken != null && string.IsNullOrEmpty(request.Headers[Constants.SessionTokenHeaderName]))
                request.Headers.Add(Constants.SessionTokenHeaderName, sessionToken);
            var response = await base.SendAsync(request, cancellationToken);

            var responseSessionToken = response.Headers[Constants.SessionTokenHeaderName];
            if (!string.IsNullOrEmpty(responseSessionToken))
            {
                _sessionProvider.SessionToken = responseSessionToken;
            }

            return response;
        }
    }
}
