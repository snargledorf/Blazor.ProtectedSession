using Microsoft.Extensions.DependencyInjection;

namespace Blazor.ProtectedSession;

public sealed class ProtectedSessionBuilder(IServiceCollection serviceCollection)
{
    public ProtectedSessionBuilder UseLocalStorage() => Configure(ProtectedStorageLocation.LocalStorage);

    public ProtectedSessionBuilder UseSessionStorage() => Configure(ProtectedStorageLocation.Session);

    public ProtectedSessionBuilder WithTimeout(TimeSpan timeout)
    {
        serviceCollection.Configure<ProtectedSessionOptions>(opt => opt.IdleTimeout = timeout);
        return this;
    }

    public ProtectedSessionBuilder WithSessionKey(string sessionKey)
    {
        serviceCollection.Configure<ProtectedSessionOptions>(opt => opt.StorageKey = sessionKey);
        return this;
    }

    private ProtectedSessionBuilder Configure(ProtectedStorageLocation storageLocation)
    {
        serviceCollection.Configure<ProtectedSessionOptions>(opt =>
            opt.StorageLocation = storageLocation);

        return this;
    }
}