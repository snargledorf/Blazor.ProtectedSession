using Blazor.ProtectedSession;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Blazor.ProtectedSession.Tests;

public class ProtectedSessionBuilderTests
{
    private readonly IServiceCollection _services = new ServiceCollection();

    [Test]
    public async Task UseLocalStorage_ShouldSetStorageLocation()
    {
        // Arrange
        var builder = new ProtectedSessionBuilder(_services);

        // Act
        builder.UseLocalStorage();
        var sp = _services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<ProtectedSessionOptions>>().Value;

        // Assert
        await Assert.That(options.StorageLocation).IsEqualTo(ProtectedStorageLocation.LocalStorage);
    }

    [Test]
    public async Task UseSessionStorage_ShouldSetStorageLocation()
    {
        // Arrange
        var builder = new ProtectedSessionBuilder(_services);

        // Act
        builder.UseSessionStorage();
        var sp = _services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<ProtectedSessionOptions>>().Value;

        // Assert
        await Assert.That(options.StorageLocation).IsEqualTo(ProtectedStorageLocation.Session);
    }

    [Test]
    public async Task WithTimeout_ShouldSetIdleTimeout()
    {
        // Arrange
        var builder = new ProtectedSessionBuilder(_services);
        var timeout = TimeSpan.FromMinutes(10);

        // Act
        builder.WithTimeout(timeout);
        var sp = _services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<ProtectedSessionOptions>>().Value;

        // Assert
        await Assert.That(options.IdleTimeout).IsEqualTo(timeout);
    }

    [Test]
    public async Task WithSessionKey_ShouldSetStorageKey()
    {
        // Arrange
        var builder = new ProtectedSessionBuilder(_services);
        var key = "custom-key";

        // Act
        builder.WithSessionKey(key);
        var sp = _services.BuildServiceProvider();
        var options = sp.GetRequiredService<IOptions<ProtectedSessionOptions>>().Value;

        // Assert
        await Assert.That(options.StorageKey).IsEqualTo(key);
    }

    [Test]
    public async Task AddProtectedSession_ShouldRegisterServices()
    {
        // Act
        _services.AddProtectedSession(opt => opt.UseLocalStorage());
        
        // We need to register HybridCache and ProtectedLocalStorage to satisfy dependencies if we were to build and get services
        // But for just checking registration, we can check the service collection
        
        // Assert
        await Assert.That(_services.Any(s => s.ServiceType == typeof(IProtectedSession))).IsTrue();
        await Assert.That(_services.Any(s => s.ServiceType == typeof(ISessionKeyLookup))).IsTrue();
    }
}
