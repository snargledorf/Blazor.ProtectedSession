namespace Blazor.ProtectedSession;

public sealed class ProtectedSessionOptions
{
    public TimeSpan IdleTimeout { get; set; } = TimeSpan.FromMinutes(20);
    
    public TimeSpan IOTimeout { get; set; } = TimeSpan.FromMinutes(1);

    public string StorageKey { get; set; } = "protected_storage_session_key";
    
    public ProtectedStorageLocation StorageLocation { get; set; } = ProtectedStorageLocation.LocalStorage;
}