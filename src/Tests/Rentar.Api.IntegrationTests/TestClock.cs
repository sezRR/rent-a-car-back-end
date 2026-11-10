using System;

namespace Rentar.Api.IntegrationTests;

/// <summary>
/// Follows the real clock unless a test freezes it, e.g. to stay clear of the 22:00 maintenance hour.
/// </summary>
public sealed class TestClock : TimeProvider
{
    private DateTimeOffset? _frozenAt;

    public override DateTimeOffset GetUtcNow()
    {
        return _frozenAt ?? System.GetUtcNow();
    }

    public void FreezeAt(DateTimeOffset instant)
    {
        _frozenAt = instant.ToUniversalTime();
    }

    public void Reset()
    {
        _frozenAt = null;
    }
}
