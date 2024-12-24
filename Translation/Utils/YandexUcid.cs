using System;

namespace LoreBridge.Translation.Utils;

public class YandexUcid
{
    private DateTimeOffset _expires = DateTimeOffset.UtcNow;
    private Guid _ucid = Guid.NewGuid();

    public Guid Get()
    {
        if (!IsExpired()) return _ucid;

        _ucid = Guid.NewGuid();
        _expires = _expires.Add(TimeSpan.FromSeconds(360));

        return _ucid;
    }

    private bool IsExpired()
    {
        return DateTimeOffset.UtcNow > _expires;
    }
}