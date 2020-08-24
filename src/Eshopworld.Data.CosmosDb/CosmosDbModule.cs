using Autofac;

namespace Eshopworld.Data.CosmosDb
{
    /// <summary>
    /// Module for dependencies in the repository
    /// </summary>
    public class CosmosDbModule : Module
    {
        /// <summary>
        /// Load module dependencies
        /// </summary>
        /// <param name="builder"></param>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CosmosDbClientFactory>()
                .As<ICosmosDbClientFactory>()
                .SingleInstance();

            builder.RegisterType<CosmosDbRepository>()
                .As<ICosmosDbRepository>()
                .PropertiesAutowired();
        }
    }
}
