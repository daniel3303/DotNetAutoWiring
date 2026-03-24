using Equibles.Core.AutoWiring;
using Microsoft.Extensions.DependencyInjection;

namespace Equibles.Core.AutoWiring.Tests;

public interface ITestService { }

[Service(ServiceLifetime.Scoped, typeof(ITestService))]
public class TestService : ITestService { }

[Service(ServiceLifetime.Singleton)]
public class TestSingletonService { }

[Service(ServiceLifetime.Transient, typeof(ITestService), replaceExisting: false)]
public class AlternateTestService : ITestService { }

public class UnregisteredService : ITestService { }

public class AutoWiringTests {
    [Fact]
    public void AutoWireServicesFrom_RegistersServiceWithInterface() {
        var services = new ServiceCollection();

        services.AutoWireServicesFrom<AutoWiringTests>();

        var descriptors = services.Where(d => d.ServiceType == typeof(ITestService)).ToList();
        Assert.Contains(descriptors, d => d.ImplementationType == typeof(TestService));
    }

    [Fact]
    public void AutoWireServicesFrom_RegistersSingletonWithoutInterface() {
        var services = new ServiceCollection();

        services.AutoWireServicesFrom<AutoWiringTests>();

        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TestSingletonService));
        Assert.NotNull(descriptor);
        Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        Assert.Equal(typeof(TestSingletonService), descriptor.ImplementationType);
    }

    [Fact]
    public void AutoWireServicesFrom_DoesNotRegisterUnattributedClasses() {
        var services = new ServiceCollection();

        services.AutoWireServicesFrom<AutoWiringTests>();

        Assert.DoesNotContain(services, d => d.ServiceType == typeof(UnregisteredService));
    }

    [Fact]
    public void AutoWireServicesFrom_ReplaceExistingTrue_RemovesPreviousRegistration() {
        var services = new ServiceCollection();
        services.AddScoped<ITestService, UnregisteredService>();

        services.AutoWireServicesFrom<AutoWiringTests>();

        // The manually added UnregisteredService should have been replaced by TestService
        // (TestService has replaceExisting: true by default)
        Assert.DoesNotContain(services, d =>
            d.ServiceType == typeof(ITestService) && d.ImplementationType == typeof(UnregisteredService));
    }

    [Fact]
    public void AutoWireServicesFrom_ReplaceExistingFalse_KeepsMultipleRegistrations() {
        var services = new ServiceCollection();

        services.AutoWireServicesFrom<AutoWiringTests>();

        var descriptors = services.Where(d => d.ServiceType == typeof(ITestService)).ToList();
        // AlternateTestService has replaceExisting: false, so it adds alongside existing
        Assert.Contains(descriptors, d => d.ImplementationType == typeof(AlternateTestService));
    }

    [Fact]
    public void AutoWireServicesFrom_ResolvesServicesCorrectly() {
        var services = new ServiceCollection();

        services.AutoWireServicesFrom<AutoWiringTests>();

        var provider = services.BuildServiceProvider();
        var singleton = provider.GetService<TestSingletonService>();
        Assert.NotNull(singleton);

        var scopedServices = provider.GetServices<ITestService>().ToList();
        Assert.NotEmpty(scopedServices);
    }
}
