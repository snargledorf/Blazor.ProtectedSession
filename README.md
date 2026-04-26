# Blazor.ProtectedSession

A lightweight Blazor library for managed, protected session state.

## Features

- **Secure Session Management**: Automatically generates and stores a unique, encrypted session key in the browser.
- **Hybrid Performance**: Uses `HybridCache` to provide high-performance L1/L2 caching for session data.
- **Automatic Expiration**: Configurable idle timeouts for session data.
- **Protected Storage**: Supports both `LocalStorage` and `SessionStorage` for session key persistence.
- **Type Safety**: Fully typed API for setting and getting session objects.

## Installation

```bash
dotnet add package Blazor.ProtectedSession
```

## Quick Start (Basic Usage)

By default, the library uses `LocalStorage` to persist the session ID and has a default idle timeout of 20 minutes.

### 1. Register Services
```csharp
builder.Services.AddProtectedSession(); 
```

### 2. Use in Component
```razor
@inject IProtectedSession Session

@code {
    protected override async Task OnInitializedAsync()
    {
        // Works immediately with default settings
        await Session.SetAsync("last_visit", DateTime.Now);
    }
}
```

## Configuration Options

In your `Program.cs`, add the service and configure the desired storage location:

```csharp
builder.Services.AddProtectedSession(options =>
{
    options.UseLocalStorage() // Or UseSessionStorage()
           .WithTimeout(TimeSpan.FromMinutes(20))
           .WithSessionKey("my_app_session");
});

// HybridCache must also be registered
builder.Services.AddHybridCache();
```

## Usage

Inject `IProtectedSession` into your components or services:

```c#
@inject IProtectedSession Session

@code {
    public record UserInfo(string Name, bool IsAdmin);

    private async Task SaveData()
    {
        var userData = new UserInfo("John Doe", true);
        await Session.SetAsync("user_info", userData);
    }

    private async Task LoadData()
    {
        var userData = await Session.GetAsync<UserInfo>("user_info");
        if (userData != null)
        {
            Console.WriteLine($"Hello, {userData.Name}!");
        }
    }

    private async Task ClearData()
    {
        await Session.DeleteAsync("user_info");
    }
}
```

## License

This project is licensed under the [MIT License](LICENSE).
