using Microsoft.Extensions.DependencyInjection;

namespace Equibles.Core.AutoWiring;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class ServiceAttribute : Attribute {
    /// <summary>
    /// The lifetime of the service.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// The interface type of the service.
    /// </summary>
    public Type InterfaceType { get; }

    /// <summary>
    /// Whether to replace the existing service.
    /// If false, the service will be added and multiple services of the same type will be registered.
    /// If true, the existing service will be replaced.
    /// </summary>
    public bool ReplaceExisting { get; }

    public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Scoped, Type interfaceType = null, bool replaceExisting = true) {
        Lifetime = lifetime;
        InterfaceType = interfaceType;
        ReplaceExisting = replaceExisting;
    }
}
