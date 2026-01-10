using Microsoft.Extensions.Configuration;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Configuration;

/// <summary>
/// 資料庫設定類別
/// </summary>
public class DatabaseConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public DatabaseConfig(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 取得預設連線字串
    /// </summary>
    public string DefaultConnection
    {
        get
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    _logger.LogWarning("DefaultConnection 連線字串未設定");
                    throw new InvalidOperationException("DefaultConnection 連線字串未設定");
                }
                return connectionString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得資料庫連線字串時發生錯誤");
                throw;
            }
        }
    }

    /// <summary>
    /// 取得連線逾時時間（秒）
    /// </summary>
    public int ConnectionTimeout
    {
        get
        {
            try
            {
                return _configuration.GetValue<int>("Database:ConnectionTimeout", 30);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得連線逾時時間時發生錯誤");
                return 30;
            }
        }
    }

    /// <summary>
    /// 取得命令逾時時間（秒）
    /// </summary>
    public int CommandTimeout
    {
        get
        {
            try
            {
                return _configuration.GetValue<int>("Database:CommandTimeout", 30);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得命令逾時時間時發生錯誤");
                return 30;
            }
        }
    }
}

