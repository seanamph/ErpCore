using Microsoft.Extensions.Configuration;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Configuration;

/// <summary>
/// JWT 設定類別
/// </summary>
public class JwtConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public JwtConfig(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 取得 JWT 簽發者
    /// </summary>
    public string Issuer
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Jwt:Issuer") ?? "ErpCore";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 JWT Issuer 時發生錯誤");
                return "ErpCore";
            }
        }
    }

    /// <summary>
    /// 取得 JWT 受眾
    /// </summary>
    public string Audience
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("Jwt:Audience") ?? "ErpCore";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 JWT Audience 時發生錯誤");
                return "ErpCore";
            }
        }
    }

    /// <summary>
    /// 取得 JWT 密鑰
    /// </summary>
    public string SecretKey
    {
        get
        {
            try
            {
                var secretKey = _configuration.GetValue<string>("Jwt:SecretKey");
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    _logger.LogWarning("JWT SecretKey 未設定");
                    throw new InvalidOperationException("JWT SecretKey 未設定");
                }
                return secretKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 JWT SecretKey 時發生錯誤");
                throw;
            }
        }
    }

    /// <summary>
    /// 取得 JWT 過期時間（分鐘）
    /// </summary>
    public int ExpirationMinutes
    {
        get
        {
            try
            {
                return _configuration.GetValue<int>("Jwt:ExpirationMinutes", 60);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得 JWT 過期時間時發生錯誤");
                return 60;
            }
        }
    }
}

