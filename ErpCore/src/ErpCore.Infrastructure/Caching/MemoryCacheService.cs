using ErpCore.Shared.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace ErpCore.Infrastructure.Caching;

/// <summary>
/// 記憶體快取服務實作
/// 使用 .NET MemoryCache 實作快取功能
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILoggerService _logger;

    public MemoryCacheService(IMemoryCache memoryCache, ILoggerService logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("快取鍵值為空，無法取得快取資料");
                return Task.FromResult<T?>(null);
            }

            if (_memoryCache.TryGetValue(key, out T? value))
            {
                _logger.LogDebug($"取得快取資料成功: {key}");
                return Task.FromResult(value);
            }

            _logger.LogDebug($"快取資料不存在: {key}");
            return Task.FromResult<T?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得快取資料時發生錯誤: {key}", ex);
            return Task.FromResult<T?>(null);
        }
    }

    public Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("快取鍵值為空，無法設定快取資料");
                return Task.FromResult(false);
            }

            if (value == null)
            {
                _logger.LogWarning("快取資料為空，無法設定快取");
                return Task.FromResult(false);
            }

            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                // 預設過期時間為 1 小時
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            }

            _memoryCache.Set(key, value, options);
            _logger.LogDebug($"設定快取資料成功: {key}, 過期時間: {expiration?.TotalMinutes ?? 60} 分鐘");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定快取資料時發生錯誤: {key}", ex);
            return Task.FromResult(false);
        }
    }

    public Task<bool> RemoveAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("快取鍵值為空，無法刪除快取資料");
                return Task.FromResult(false);
            }

            _memoryCache.Remove(key);
            _logger.LogDebug($"刪除快取資料成功: {key}");
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除快取資料時發生錯誤: {key}", ex);
            return Task.FromResult(false);
        }
    }

    public Task<bool> ExistsAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Task.FromResult(false);
            }

            var exists = _memoryCache.TryGetValue(key, out _);
            return Task.FromResult(exists);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查快取資料是否存在時發生錯誤: {key}", ex);
            return Task.FromResult(false);
        }
    }

    public Task<bool> ClearAsync()
    {
        try
        {
            // MemoryCache 不支援直接清除所有資料
            // 需要實作自訂的清除邏輯或使用其他快取實作
            _logger.LogWarning("MemoryCache 不支援清除所有快取資料");
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError("清除所有快取資料時發生錯誤", ex);
            return Task.FromResult(false);
        }
    }
}

