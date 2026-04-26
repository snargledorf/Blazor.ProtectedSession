using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Blazor.ProtectedSession.Tests;

public class DependencyInjectionTests
{
    [Test]
    public async Task AddProtectedSession_ShouldReturnServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddProtectedSession();

        // Assert
        await Assert.That(result).IsSameReferenceAs(services);
    }

    [Test]
    public async Task AddProtectedSession_ShouldRegisterExpectedServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddProtectedSession();

        // Assert
        await Assert.That(services.Any(d => d.ServiceType == typeof(IProtectedSession) && d.ImplementationType == typeof(ProtectedSession))).IsTrue();
        await Assert.That(services.Any(d => d.ServiceType == typeof(ISessionKeyLookup))).IsTrue();
    }

    [Test]
    public async Task AddProtectedSession_WithConfiguration_ShouldApplyOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var timeout = TimeSpan.FromMinutes(30);
        var storageKey = "custom-key";

        // Act
        services.AddProtectedSession(builder =>
        {
            builder.WithTimeout(timeout)
                   .WithSessionKey(storageKey)
                   .UseLocalStorage();
        });

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ProtectedSessionOptions>>().Value;

        await Assert.That(options.IdleTimeout).IsEqualTo(timeout);
        await Assert.That(options.StorageKey).IsEqualTo(storageKey);
        await Assert.That(options.StorageLocation).IsEqualTo(ProtectedStorageLocation.LocalStorage);
    }

    [Test]
    public async Task AddProtectedSession_WithSessionStorage_ShouldApplyOptions()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddProtectedSession(builder => builder.UseSessionStorage());

        // Assert
        var serviceProvider = services.BuildServiceProvider();
        var options = serviceProvider.GetRequiredService<IOptions<ProtectedSessionOptions>>().Value;

        await Assert.That(options.StorageLocation).IsEqualTo(ProtectedStorageLocation.Session);
    }

    [Test]
    public async Task IProtectedSession_CanBeResolved_WhenDependenciesArePresent()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddProtectedSession();
        
        // Mock dependencies that are expected to be present
        services.AddSingleton(Substitute.For<HybridCache>());
        
        // We override ISessionKeyLookup because the default one depends on ProtectedLocalStorage/ProtectedSessionStorage
        // which are sealed and hard to mock without a full Blazor environment.
        services.AddSingleton(Substitute.For<ISessionKeyLookup>());

        var serviceProvider = services.BuildServiceProvider();

        // Act
        var protectedSession = serviceProvider.GetService<IProtectedSession>();

        // Assert
        await Assert.That(protectedSession).IsNotNull();
        await Assert.That(protectedSession).IsTypeOf<ProtectedSession>();
    }
}
