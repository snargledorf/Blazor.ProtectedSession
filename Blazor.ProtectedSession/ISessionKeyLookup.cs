namespace Blazor.ProtectedSession;

internal interface ISessionKeyLookup
{
    public ValueTask<Guid> GetSessionKeyAsync();
}