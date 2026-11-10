using Microsoft.Extensions.Caching.Memory;
using Rentar.Shared.Application.Caching;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Rentar.Shared.Infrastructure.Caching;

public class MemoryCacheManager(IMemoryCache memoryCache) : ICacheManager
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public void Add(string key, object value, int duration)
    {
        _memoryCache.Set(key, value, TimeSpan.FromMinutes(duration));
    }

    public T Get<T>(string key)
    {
        return _memoryCache.Get<T>(key);
    }

    public object Get(string key)
    {
        return _memoryCache.Get(key);
    }

    public bool IsAdd(string key)
    {
        return _memoryCache.TryGetValue(key, out _);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public void RemoveByPattern(string pattern)
    {
        var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
        var keysToRemove = ((MemoryCache)_memoryCache).Keys
            .Where(key => regex.IsMatch(key.ToString()))
            .ToList();

        foreach (var key in keysToRemove)
        {
            _memoryCache.Remove(key);
        }
    }
}
