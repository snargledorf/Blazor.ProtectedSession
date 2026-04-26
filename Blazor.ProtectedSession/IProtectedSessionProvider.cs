namespace Blazor.ProtectedSession;

public interface IProtectedSessionProvider
{
    IProtectedSession GetSession();
}