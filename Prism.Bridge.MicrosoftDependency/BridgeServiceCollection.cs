using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Prism.Ioc;

namespace Prism.Bridge.MicrosoftDependency;

/// <summary>
/// A service collection that bridges to Prism's service registry.
/// This class transforms a simple registration process into a Prism registration.
/// It does not support unregistration etc.
/// It is intended to be used implicitly in <see cref="BridgeServiceCollectionExtensions.RegisterBridge(IContainerRegistry, Action{IServiceCollection})"/> extension methods.
/// </summary>
public class BridgeServiceCollection : IServiceCollection, IDisposable
{
    /// <summary>A constructor that associates with the Prism service registry.</summary>
    /// <param name="containerRegistry">Prism service registry.</param>
    public BridgeServiceCollection(IContainerRegistry containerRegistry)
    {
        this.registry = containerRegistry;
        this.services = new ServiceCollection();
        this.services.BuildServiceProvider();
    }

    /// <inheritdoc />
    public ServiceDescriptor this[int index]
    {
        get => this.services[index];
        set => this.services[index] = value;
    }

    /// <inheritdoc />
    public int Count => this.services.Count;

    /// <inheritdoc />
    public bool IsReadOnly => this.services.IsReadOnly;

    /// <inheritdoc />
    public void Add(ServiceDescriptor item)
    {
        ((IServiceCollection)this.services).Add(item);

        bridgeRegistry(item);
    }

    /// <inheritdoc />
    public void Insert(int index, ServiceDescriptor item)
    {
        this.services.Insert(index, item);

        bridgeRegistry(item);
    }

    /// <inheritdoc />
    public bool Remove(ServiceDescriptor item)
    {
        return this.services.Remove(item);
    }

    /// <inheritdoc />
    public void RemoveAt(int index)
    {
        this.services.RemoveAt(index);
    }

    /// <inheritdoc />
    public void Clear()
    {
        this.services.Clear();
    }

    /// <inheritdoc />
    public bool Contains(ServiceDescriptor item)
    {
        return this.services.Contains(item);
    }

    /// <inheritdoc />
    public int IndexOf(ServiceDescriptor item)
    {
        return this.services.IndexOf(item);
    }

    /// <inheritdoc />
    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return this.services.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.services.GetEnumerator();
    }

    /// <inheritdoc />
    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
        this.services.CopyTo(array, arrayIndex);
    }

    /// <summary>Build a service provider.</summary>
    /// <returns>Service provider</returns>
    public ServiceProvider Build()
    {
        this.provider = this.services.BuildServiceProvider();
        return this.provider;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                this.provider?.Dispose();
                this.provider = null;
            }

            disposedValue = true;
        }
    }

    private IContainerRegistry registry;
    private ServiceCollection services;
    private ServiceProvider? provider;
    private bool disposedValue;

    private void bridgeRegistry(ServiceDescriptor item)
    {
        if (item == null) return;

        if (item.ImplementationInstance != null)
        {
            this.registry.RegisterInstance(item.ServiceType, item.ImplementationInstance);
        }
        else if (item.ImplementationFactory != null)
        {
            switch (item.Lifetime)
            {
                case ServiceLifetime.Singleton: this.registry.RegisterSingleton(item.ServiceType, _ => item.ImplementationFactory(this.provider ?? throw new InvalidOperationException())); break;
                case ServiceLifetime.Scoped: this.registry.RegisterScoped(item.ServiceType, _ => item.ImplementationFactory(this.provider!)); break;
                case ServiceLifetime.Transient: this.registry.Register(item.ServiceType, _ => item.ImplementationFactory(this.provider!)); break;
                default: break;
            }
        }
        else
        {
            switch (item.Lifetime)
            {
                case ServiceLifetime.Singleton: this.registry.RegisterSingleton(item.ServiceType, item.ImplementationType); break;
                case ServiceLifetime.Scoped: this.registry.RegisterScoped(item.ServiceType, item.ImplementationType); break;
                case ServiceLifetime.Transient: this.registry.Register(item.ServiceType, item.ImplementationType); break;
                default: break;
            }
        }
    }
}
