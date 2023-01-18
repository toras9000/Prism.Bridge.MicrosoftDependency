# Simple.Prism.Bridge.MicrosoftDependency
[![NugetShield]][NugetPackage]

[NugetPackage]: https://www.nuget.org/packages/Simple.Prism.Bridge.MicrosoftDependency
[NugetShield]: https://img.shields.io/nuget/v/Simple.Prism.Bridge.MicrosoftDependency

It's a simple one-way bridge that passes type registrations for Microsoft.Extensions.DependencyInjection's IServiceCollection to Prism's IContainerRegistry.  
Resolving types from IServiceProvider is not supported.

Usage:
```csharp
protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    containerRegistry.RegisterBridge(services =>
    {
        // If IServiceCollection is required, use bridge registration.
        // Registering to IServiceCollection implies registering to IContainerRegistry.
        
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddHttpClient();
        services.AddDbContextFactory<AppDbContext>(builder => builder.UseSqlite(configuration.GetConnectionString("AppDatabase")));
    });
    
    // Other services can be registered directly with Prism.
    containerRegistry.Register<IAppDataService, EfCoreAppDataService>();
}
```

Sample: [TestPrismDependency](https://github.com/toras9000/TestPrismDependency)

