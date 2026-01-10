using Microsoft.Extensions.Configuration;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Configuration;

/// <summary>
/// 郵件設定類別
/// </summary>
public class EmailConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public EmailConfig(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 取得 SMTP 伺服器
    /// </summary>
    public string SmtpServer
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Email:SmtpServer") ?? "localhost";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 SMTP 伺服器時發生錯誤");
                return "localhost";
            }
        }
    }

    /// <summary>
    /// 取得 SMTP 埠號
    /// </summary>
    public int SmtpPort
    {
        get
        {
            try
            {
                return _configuration.GetValue<int>("Email:SmtpPort", 25);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 SMTP 埠號時發生錯誤");
                return 25;
            }
        }
    }

    /// <summary>
    /// 取得寄件者電子郵件
    /// </summary>
    public string FromEmail
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Email:FromEmail") ?? "noreply@erpcore.com";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得寄件者電子郵件時發生錯誤");
                return "noreply@erpcore.com";
            }
        }
    }

    /// <summary>
    /// 取得寄件者名稱
    /// </summary>
    public string FromName
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Email:FromName") ?? "ErpCore System";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得寄件者名稱時發生錯誤");
                return "ErpCore System";
            }
        }
    }

    /// <summary>
    /// 取得是否啟用 SSL
    /// </summary>
    public bool EnableSsl
    {
        get
        {
            try
            {
                return _configuration.GetValue<bool>("Email:EnableSsl", false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 SSL 啟用狀態時發生錯誤");
                return false;
            }
        }
    }
}

