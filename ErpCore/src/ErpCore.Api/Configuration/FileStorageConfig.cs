using Microsoft.Extensions.Configuration;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Configuration;

/// <summary>
/// 檔案儲存設定類別
/// </summary>
public class FileStorageConfig
{
    private readonly IConfiguration _configuration;
    private readonly ILoggerService _logger;

    public FileStorageConfig(IConfiguration configuration, ILoggerService logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// 取得檔案儲存路徑
    /// </summary>
    public string StoragePath
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("FileStorage:Path") ?? "uploads";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得檔案儲存路徑時發生錯誤");
                return "uploads";
            }
        }
    }

    /// <summary>
    /// 取得最大檔案大小（MB）
    /// </summary>
    public int MaxFileSizeMB
    {
        get
        {
            try
            {
                return _configuration.GetValue<int>("FileStorage:MaxFileSizeMB", 10);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得最大檔案大小時發生錯誤");
                return 10;
            }
        }
    }

    /// <summary>
    /// 取得允許的檔案副檔名
    /// </summary>
    public string[] AllowedExtensions
    {
        get
        {
            try
            {
                var extensions = _configuration.GetValue<string>("FileStorage:AllowedExtensions");
                if (string.IsNullOrWhiteSpace(extensions))
                {
                    return new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png" };
                }
                return extensions.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(e => e.Trim())
                    .ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得允許的檔案副檔名時發生錯誤");
                return new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png" };
            }
        }
    }

    /// <summary>
    /// 取得儲存提供者類型
    /// </summary>
    public string Provider
    {
        get
        {
            try
            {
                return _configuration.GetValue<string>("FileStorage:Provider") ?? "Local";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "取得儲存提供者類型時發生錯誤");
                return "Local";
            }
        }
    }
}

