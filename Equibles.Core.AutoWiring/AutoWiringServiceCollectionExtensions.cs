using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Equibles.Core.AutoWiring;

public static class AutoWiringServiceCollectionExtensions {
    /// <summary>
    /// Scans the assembly containing <typeparamref name="TAssembly"/> for classes decorated
    /// with <see cref="ServiceAttribute"/> and registers them in the service collection.
    /// </summary>
    public static void AutoWireServicesFrom<TAssembly>(this IServiceCollection services) {
        var typesWithServiceAttribute = typeof(TAssembly).Assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<ServiceAttribute>() != null);

        foreach (var type in typesWithServiceAttribute) {
            var attribute = type.GetCustomAttribute<ServiceAttribute>();
            Debug.Assert(attribute != null, nameof(attribute) + " != null");
            var serviceType = attribute.InterfaceType ?? type;

            if (attribute.ReplaceExisting) {
                services.RemoveAll(serviceType);
            }

            var descriptor = new ServiceDescriptor(serviceType, type, attribute.Lifetime);
            services.Add(descriptor);
        }
    }
}
