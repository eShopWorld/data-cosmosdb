using System.Net.Http;
using Autofac;
using Eshopworld.Data.CosmosDb.SessionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;

namespace Eshopworld.Data.CosmosDb
{
    public static class ContainerBuilderExtensions
    {
        /// <summary>
        /// Adds support for passing Cosmos DB session tokens.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <param name="passthrough">If true session tokens are accepted from the incomming HTTP request and returned in its response. Otherwise
        /// session tokens are handled internally and the incomming HTTP request is not used.</param>
        /// <returns></returns>
        public static ContainerBuilder AddCosmosSessionRelay(this ContainerBuilder containerBuilder, bool passthrough)
        {
            if (passthrough)
            {
                containerBuilder.RegisterType<NonPassthroughCosmosDbSessionTokenProvider>()
                    .As<ICosmosDbSessionTokenProvider>();
            }
            else
            {
                containerBuilder.RegisterType<PassthroughCosmosDbSessionTokenProvider>()
                    .As<ICosmosDbSessionTokenProvider>();
            }

            containerBuilder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .IfNotRegistered(typeof(IHttpContextAccessor));

            containerBuilder.RegisterType<CosmosSessionHandler>().As<DelegatingHandler>();

            containerBuilder.RegisterType<SessionTokenCosmosRequestHandler>()
                .As<CosmosRequestHandler>();

            containerBuilder.RegisterDecorator<IDocumentClient>((c, p, client) =>
                {
                    var sessionProvider = c.Resolve<ICosmosDbSessionTokenProvider>();
                    return new SessionProxyDocumentClient(client, sessionProvider);
                });

            return containerBuilder;
        }
    }
}
