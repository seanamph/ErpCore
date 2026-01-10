using Microsoft.Extensions.Configuration;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Configuration;

/// <summary>
/// 日誌設定類別
/// </summary>
public class LoggingConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public LoggingConfig(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 取得日誌層級
    /// </summary>
    public string LogLevel
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Logging:LogLevel:Default") ?? "Information";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得日誌層級時發生錯誤");
                return "Information";
            }
        }
    }

    /// <summary>
    /// 取得日誌檔案路徑
    /// </summary>
    public string LogFilePath
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Logging:FilePath") ?? "logs";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得日誌檔案路徑時發生錯誤");
                return "logs";
            }
        }
    }

    /// <summary>
    /// 取得日誌檔案名稱格式（按小時分割）
    /// </summary>
    public string LogFileNameFormat
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Logging:FileNameFormat") ?? "yyyyMMdd-HH.log";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得日誌檔案名稱格式時發生錯誤");
                return "yyyyMMdd-HH.log";
            }
        }
    }

    /// <summary>
    /// 取得是否啟用檔案日誌
    /// </summary>
    public bool EnableFileLogging
    {
        get
        {
            try
            {
                return _configuration.GetValue<bool>("Logging:EnableFileLogging", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得檔案日誌啟用狀態時發生錯誤");
                return true;
            }
        }
    }
}

