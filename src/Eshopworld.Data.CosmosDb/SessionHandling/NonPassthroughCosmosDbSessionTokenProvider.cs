using Microsoft.AspNetCore.Http;

namespace Eshopworld.Data.CosmosDb.SessionHandling
{
    public class NonPassthroughCosmosDbSessionTokenProvider : ICosmosDbSessionTokenProvider
    {
        private static readonly object SessionTokenItemKey = new object();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NonPassthroughCosmosDbSessionTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string SessionToken
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                return (string)context.Items[SessionTokenItemKey];
            }
            set
            {
                var context = _httpContextAccessor.HttpContext;
                context.Items[SessionTokenItemKey] = value;
            }
        }
    }
}
