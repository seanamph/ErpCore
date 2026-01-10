using Microsoft.Extensions.Configuration;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Configuration;

/// <summary>
/// 應用程式設定類別
/// </summary>
public class AppConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public AppConfig(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 取得應用程式名稱
    /// </summary>
    public string AppName
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("App:Name") ?? "ErpCore";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得應用程式名稱時發生錯誤");
                return "ErpCore";
            }
        }
    }

    /// <summary>
    /// 取得應用程式版本
    /// </summary>
    public string AppVersion
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("App:Version") ?? "1.0.0";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得應用程式版本時發生錯誤");
                return "1.0.0";
            }
        }
    }

    /// <summary>
    /// 取得環境名稱
    /// </summary>
    public string Environment
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("App:Environment") ?? "Development";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得環境名稱時發生錯誤");
                return "Development";
            }
        }
    }

    /// <summary>
    /// 取得是否啟用 Swagger
    /// </summary>
    public bool EnableSwagger
    {
        get
        {
            try
            {
                return _configuration.GetValue<bool>("App:EnableSwagger", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 Swagger 啟用狀態時發生錯誤");
                return true;
            }
        }
    }
}

