using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Eshopworld.Data.CosmosDb.SessionHandling
{
    internal class BackendCosmosDbSessionTokenProvider : ICosmosDbSessionTokenProvider
    {
        private static readonly object SessionTokenItemKey = new object();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BackendCosmosDbSessionTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string SessionToken
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                var sessionTokenValues = context.Response.Headers[Constants.SessionTokenHeaderName];
                if (sessionTokenValues.Count != 1)
                    sessionTokenValues = context.Request.Headers[Constants.SessionTokenHeaderName];
                return sessionTokenValues.Count == 1 ? sessionTokenValues.First() : null;
            }
            set
            {
                var context = _httpContextAccessor.HttpContext;
                context.Response.Headers[Constants.SessionTokenHeaderName] = value;
            }
        }
    }

}
