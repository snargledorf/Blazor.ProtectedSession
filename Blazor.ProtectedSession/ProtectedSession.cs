using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace Blazor.ProtectedSession;

internal sealed class ProtectedSession(
    HybridCache cache,
    ISessionKeyLookup sessionKeyLookup,
    IOptions<ProtectedSessionOptions> options) : IProtectedSession
{
    private readonly ProtectedSessionOptions _options = options.Value;

    public async ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var ioTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        ioTimeoutCts.CancelAfter(_options.IOTimeout);

        Guid sessionKey = await sessionKeyLookup.GetSessionKeyAsync();

        var objectKey = new ProtectedSessionObjectKey(sessionKey, key);

        await cache.SetAsync(objectKey.CacheKey,
            value,
            new HybridCacheEntryOptions
            {
                Expiration = _options.IdleTimeout
            }, cancellationToken: ioTimeoutCts.Token);
    }

    public async ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var ioTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        ioTimeoutCts.CancelAfter(_options.IOTimeout);

        Guid sessionKey = await sessionKeyLookup.GetSessionKeyAsync();

        var objectKey = new ProtectedSessionObjectKey(sessionKey, key);

        return await cache.GetOrCreateAsync<T?>(objectKey.CacheKey,
            _ => ValueTask.FromResult<T?>(default),
            new HybridCacheEntryOptions
            {
                Expiration = _options.IdleTimeout
            }, cancellationToken: ioTimeoutCts.Token);
    }

    public async ValueTask DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var ioTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        ioTimeoutCts.CancelAfter(_options.IOTimeout);

        Guid sessionKey = await sessionKeyLookup.GetSessionKeyAsync();

        var objectKey = new ProtectedSessionObjectKey(sessionKey, key);

        await cache.RemoveAsync(objectKey.CacheKey, ioTimeoutCts.Token);
    }
}