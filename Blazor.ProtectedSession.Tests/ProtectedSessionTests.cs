using Blazor.ProtectedSession;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace Blazor.ProtectedSession.Tests;

public class ProtectedSessionTests
{
    private readonly HybridCache _mockCache = Substitute.For<HybridCache>();
    private readonly ISessionKeyLookup _mockSessionKeyLookup = Substitute.For<ISessionKeyLookup>();
    private readonly IOptions<ProtectedSessionOptions> _options = Options.Create(new ProtectedSessionOptions());
    private readonly Guid _sessionKey = Guid.NewGuid();

    public ProtectedSessionTests()
    {
        _mockSessionKeyLookup.GetSessionKeyAsync().Returns(new ValueTask<Guid>(_sessionKey));
    }

    [Test]
    public async Task SetAsync_ShouldCallCacheSetAsync()
    {
        // Arrange
        var sut = new ProtectedSession(_mockCache, _mockSessionKeyLookup, _options);
        var key = "test-key";
        var value = "test-value";
        var expectedCacheKey = new ProtectedSessionObjectKey(_sessionKey, key).CacheKey;

        // Act
        await sut.SetAsync(key, value);

        // Assert
        await _mockCache.Received(1).SetAsync(
            expectedCacheKey,
            value,
            Arg.Is<HybridCacheEntryOptions>(o => o.Expiration == _options.Value.IdleTimeout),
            Arg.Any<IEnumerable<string>?>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GetAsync_ShouldCallCacheGetOrCreateAsync()
    {
        // Arrange
        var sut = new ProtectedSession(_mockCache, _mockSessionKeyLookup, _options);
        var key = "test-key";
        var value = "test-value";
        var expectedCacheKey = new ProtectedSessionObjectKey(_sessionKey, key).CacheKey;

        _mockCache.GetOrCreateAsync<string?>(
            expectedCacheKey,
            Arg.Any<Func<CancellationToken, ValueTask<string?>>>(),
            Arg.Any<HybridCacheEntryOptions?>(),
            Arg.Any<IEnumerable<string>?>(),
            Arg.Any<CancellationToken>()
        ).Returns(new ValueTask<string?>(value));

        // Act
        var result = await sut.GetAsync<string>(key);

        // Assert
        await Assert.That(result).IsEqualTo(value);
    }

    [Test]
    public async Task DeleteAsync_ShouldCallCacheRemoveAsync()
    {
        // Arrange
        var sut = new ProtectedSession(_mockCache, _mockSessionKeyLookup, _options);
        var key = "test-key";
        var expectedCacheKey = new ProtectedSessionObjectKey(_sessionKey, key).CacheKey;

        // Act
        await sut.DeleteAsync(key);

        // Assert
        await _mockCache.Received(1).RemoveAsync(expectedCacheKey, Arg.Any<CancellationToken>());
    }
}
