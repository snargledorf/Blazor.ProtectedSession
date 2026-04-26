namespace Blazor.ProtectedSession;

public interface IProtectedSession
{
    ValueTask SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    ValueTask<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    ValueTask DeleteAsync(string key, CancellationToken cancellationToken = default);
}