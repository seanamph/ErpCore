namespace ErpCore.Infrastructure.Caching;

/// <summary>
/// 快取服務介面
/// 提供快取資料的存取、設定、刪除等功能
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 取得快取資料
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    /// <param name="key">快取鍵值</param>
    /// <returns>快取資料，不存在則返回 null</returns>
    Task<T?> GetAsync<T>(string key) where T : class;

    /// <summary>
    /// 設定快取資料
    /// </summary>
    /// <typeparam name="T">資料類型</typeparam>
    /// <param name="key">快取鍵值</param>
    /// <param name="value">要快取的資料</param>
    /// <param name="expiration">過期時間（可選）</param>
    /// <returns>是否設定成功</returns>
    Task<bool> SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

    /// <summary>
    /// 刪除快取資料
    /// </summary>
    /// <param name="key">快取鍵值</param>
    /// <returns>是否刪除成功</returns>
    Task<bool> RemoveAsync(string key);

    /// <summary>
    /// 檢查快取資料是否存在
    /// </summary>
    /// <param name="key">快取鍵值</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string key);

    /// <summary>
    /// 清除所有快取資料
    /// </summary>
    /// <returns>是否清除成功</returns>
    Task<bool> ClearAsync();
}

