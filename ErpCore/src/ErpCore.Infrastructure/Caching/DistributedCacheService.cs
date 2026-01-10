using ErpCore.Shared.Logging;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ErpCore.Infrastructure.Caching;

/// <summary>
/// 分散式快取服務實作
/// 使用 .NET DistributedCache 實作分散式快取功能
/// </summary>
public class DistributedCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly ILoggerService _logger;

    public DistributedCacheService(IDistributedCache distributedCache, ILoggerService logger)
    {
        _distributedCache = distributedCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("快取鍵值為空，無法取得快取資料");
                return null;
            }

            var value = await _distributedCache.GetStringAsync(key);
            
            if (string.IsNullOrWhiteSpace(value))
            {
                _logger.LogDebug($"快取資料不存在: {key}");
                return null;
            }

            var result = JsonSerializer.Deserialize<T>(value);
            _logger.LogDebug($"取得分散式快取資料成功: {key}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"取得分散式快取資料時發生錯誤: {key}", ex);
            return null;
        }
    }

    public async Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("快取鍵值為空，無法設定快取資料");
                return false;
            }

            if (value == null)
            {
                _logger.LogWarning("快取資料為空，無法設定快取");
                return false;
            }

            var json = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                // 預設過期時間為 1 小時
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            }

            await _distributedCache.SetStringAsync(key, json, options);
            _logger.LogDebug($"設定分散式快取資料成功: {key}, 過期時間: {expiration?.TotalMinutes ?? 60} 分鐘");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"設定分散式快取資料時發生錯誤: {key}", ex);
            return false;
        }
    }

    public async Task<bool> RemoveAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                _logger.LogWarning("快取鍵值為空，無法刪除快取資料");
                return false;
            }

            await _distributedCache.RemoveAsync(key);
            _logger.LogDebug($"刪除分散式快取資料成功: {key}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除分散式快取資料時發生錯誤: {key}", ex);
            return false;
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            var value = await _distributedCache.GetStringAsync(key);
            return !string.IsNullOrWhiteSpace(value);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查分散式快取資料是否存在時發生錯誤: {key}", ex);
            return false;
        }
    }

    public Task<bool> ClearAsync()
    {
        try
        {
            // DistributedCache 不支援直接清除所有資料
            // 需要實作自訂的清除邏輯或使用其他快取實作
            _logger.LogWarning("DistributedCache 不支援清除所有快取資料");
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError("清除所有分散式快取資料時發生錯誤", ex);
            return Task.FromResult(false);
        }
    }
}

