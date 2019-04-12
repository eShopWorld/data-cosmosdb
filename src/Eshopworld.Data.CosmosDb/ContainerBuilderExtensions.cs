using System.Net.Http;
using Autofac;
using Eshopworld.Data.CosmosDb.SessionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents;

namespace Eshopworld.Data.CosmosDb
{
    public static class ContainerBuilderExtensions
    {
        public static ContainerBuilder AddCosmosSessionRelay(this ContainerBuilder containerBuilder, bool isFrontend)
        {
            containerBuilder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .IfNotRegistered(typeof(IHttpContextAccessor));
            if (isFrontend)
            {
                containerBuilder.RegisterType<FrontendCosmosDbSessionTokenProvider>()
                    .As<ICosmosDbSessionTokenProvider>();
            }
            else
            {
                containerBuilder.RegisterType<BackendCosmosDbSessionTokenProvider>()
                    .As<ICosmosDbSessionTokenProvider>();
            }

            containerBuilder.RegisterType<CosmosSessionHandler>().As<DelegatingHandler>();

            containerBuilder.RegisterDecorator<IDocumentClient>((c, p, client) =>
                {
                    var sessionProvider = c.Resolve<ICosmosDbSessionTokenProvider>();
                    return new SessionProxyDocumentClient(client, sessionProvider);
                });

            return containerBuilder;
        }
    }
}
