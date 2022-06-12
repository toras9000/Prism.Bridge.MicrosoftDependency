using System;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;

namespace Prism.Bridge.MicrosoftDependency;

/// <summary>
/// Extension method for bridge registration
/// </summary>
public static class BridgeServiceCollectionExtensions
{
    /// <summary>Bridging service collection registration to Prism containers.</summary>
    /// <param name="registry">Prism service registry</param>
    /// <param name="register">Registration process to the service collection.</param>
    public static void RegisterBridge(this IContainerRegistry registry, Action<IServiceCollection> register)
    {
        var services = new BridgeServiceCollection(registry);
        registry.RegisterInstance<IServiceCollection>(services);

        register(services);

        var provider = services.Build();
        registry.RegisterInstance<IServiceProvider>(provider);

    }
}
