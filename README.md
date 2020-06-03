# Eshopworld.Data.CosmosDb

A CosmosDb abstraction layer which follows company established practices.

## Usage

### Standalone application / Service Fabric service

In your `Program.cs` file
```c#
public static async Task Main()
{
    var configuration = EswDevOpsSdk.BuildConfiguration(env.ContentRootPath, env.EnvironmentName);

    var builder = new ContainerBuilder();

    builder.RegisterModule<CosmosDbModule>();
    builder.ConfigureCosmosDb(_configuration);

    using (builder.Build())
    {
        await Task.Delay(Timeout.Infinite);
    }
}
```

### WebAPI

In your `Startup.cs` file
```c#
private readonly IConfigurationRoot _configuration;

public Startup(IConfiguration configuration, IWebHostEnvironment env)
{
    _configuration = EswDevOpsSdk.BuildConfiguration(env.ContentRootPath, env.EnvironmentName);
}

public void ConfigureContainer(ContainerBuilder builder)
{
    builder.RegisterModule<CosmosDbModule>();
    builder.ConfigureCosmosDb(_configuration);
}
```
