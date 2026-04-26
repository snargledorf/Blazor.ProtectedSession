using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Blazor.ProtectedSession;

internal sealed class SessionKeyLookup(ProtectedBrowserStorage protectedBrowserStorage, string storageKey) : ISessionKeyLookup
{
    public async ValueTask<Guid> GetSessionKeyAsync()
    {
        ProtectedBrowserStorageResult<Guid> result =
            await protectedBrowserStorage.GetAsync<Guid>(storageKey).ConfigureAwait(false);

        if (result.Success)
            return result.Value;
        
        var sessionKey = Guid.NewGuid();
        
        await protectedBrowserStorage.SetAsync(storageKey, sessionKey);

        return sessionKey;
    }
}