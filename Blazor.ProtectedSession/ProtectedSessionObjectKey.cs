namespace Blazor.ProtectedSession;

public readonly record struct ProtectedSessionObjectKey(Guid SessionKey, string ObjectKey)
{
    public string CacheKey => $"protectedsession:{SessionKey}:{ObjectKey}";

    public override string ToString()
    {
        return CacheKey;
    }
}