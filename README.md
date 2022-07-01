# Simple.Prism.Bridge.MicrosoftDependency
[![NugetShield]][NugetPackage]

[NugetPackage]: https://www.nuget.org/packages/Simple.Prism.Bridge.MicrosoftDependency
[NugetShield]: https://img.shields.io/nuget/v/Simple.Prism.Bridge.MicrosoftDependency

It's a simple one-way bridge that passes type registrations for Microsoft.Extensions.DependencyInjection's IServiceCollection to Prism's IContainerRegistry.  
Resolving types from IServiceProvider is not supported.

Usage.
```csharp
protected override void RegisterTypes(IContainerRegistry containerRegistry)
{
    containerRegistry.RegisterBridge(services =>
    {
        services.AddHttpClient();
    });
}
```