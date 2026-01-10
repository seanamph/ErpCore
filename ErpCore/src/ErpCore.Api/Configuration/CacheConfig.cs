using Microsoft.Extensions.Configuration;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Configuration;

/// <summary>
/// 快取設定類別
/// </summary>
public class CacheConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public CacheConfig(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 取得快取過期時間（分鐘）
    /// </summary>
    public int ExpirationMinutes
    {
        get
        {
            try
            {
                return _configuration.GetValue<int>("Cache:ExpirationMinutes", 30);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得快取過期時間時發生錯誤");
                return 30;
            }
        }
    }

    /// <summary>
    /// 取得是否啟用快取
    /// </summary>
    public bool EnableCache
    {
        get
        {
            try
            {
                return _configuration.GetValue<bool>("Cache:EnableCache", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得快取啟用狀態時發生錯誤");
                return true;
            }
        }
    }

    /// <summary>
    /// 取得快取提供者類型
    /// </summary>
    public string Provider
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Cache:Provider") ?? "Memory";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得快取提供者類型時發生錯誤");
                return "Memory";
            }
        }
    }
}

