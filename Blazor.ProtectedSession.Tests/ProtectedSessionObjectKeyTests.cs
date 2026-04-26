using Blazor.ProtectedSession;

namespace Blazor.ProtectedSession.Tests;

public class ProtectedSessionObjectKeyTests
{
    [Test]
    public async Task CacheKey_ShouldBeFormattedCorrectly()
    {
        // Arrange
        var sessionKey = Guid.NewGuid();
        var key = "test-key";
        var objectKey = new ProtectedSessionObjectKey(sessionKey, key);

        // Act
        var cacheKey = objectKey.CacheKey;

        // Assert
        await Assert.That(cacheKey).IsEqualTo($"protectedsession:{sessionKey}:{key}");
    }

    [Test]
    public async Task ToString_ShouldReturnCacheKey()
    {
        // Arrange
        var sessionKey = Guid.NewGuid();
        var key = "test-key";
        var objectKey = new ProtectedSessionObjectKey(sessionKey, key);

        // Act
        var result = objectKey.ToString();

        // Assert
        await Assert.That(result).IsEqualTo(objectKey.CacheKey);
    }
}
