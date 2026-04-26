using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Blazor.ProtectedSession;

public static class ProtectedSessionServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddProtectedSession(Action<ProtectedSessionBuilder>? configure = null)
        {
            serviceCollection.AddOptions<ProtectedSessionOptions>();

            configure?.Invoke(new ProtectedSessionBuilder(serviceCollection));
            
            serviceCollection.AddBrowserStorage();
            
            serviceCollection.AddTransient<IProtectedSession, ProtectedSession>();
            
            return  serviceCollection;
        }

        private void AddBrowserStorage()
        {
            serviceCollection.AddSingleton<ISessionKeyLookup>(sp =>
            {
                ProtectedSessionOptions protectedSessionOptions = sp.GetRequiredService<IOptions<ProtectedSessionOptions>>().Value;

                ProtectedBrowserStorage protectedBrowserStorage = protectedSessionOptions.StorageLocation switch
                {
                    ProtectedStorageLocation.LocalStorage => sp.GetRequiredService<ProtectedLocalStorage>(),
                    ProtectedStorageLocation.Session => sp.GetRequiredService<ProtectedSessionStorage>(),
                    _ => throw new ArgumentOutOfRangeException(nameof(protectedSessionOptions.StorageLocation),
                        protectedSessionOptions.StorageLocation, null)
                };

                return new SessionKeyLookup(protectedBrowserStorage, protectedSessionOptions.StorageKey);
            });
        }
    }
}