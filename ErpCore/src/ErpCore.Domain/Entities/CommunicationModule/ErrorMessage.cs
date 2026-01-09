namespace ErpCore.Domain.Entities.CommunicationModule;

/// <summary>
/// 錯誤訊息記錄實體 (XCOMMSG)
/// </summary>
public class ErrorMessage
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤類型 (HTTP, APPLICATION, SYSTEM, WARNING)
    /// </summary>
    public string ErrorType { get; set; } = string.Empty;

    /// <summary>
    /// HTTP狀態碼 (401, 404, 500等)
    /// </summary>
    public int? HttpStatusCode { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string ErrorMessageText { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤詳細資訊
    /// </summary>
    public string? ErrorDetail { get; set; }

    /// <summary>
    /// 請求URL
    /// </summary>
    public string? RequestUrl { get; set; }

    /// <summary>
    /// 請求方法 (GET, POST等)
    /// </summary>
    public string? RequestMethod { get; set; }

    /// <summary>
    /// 使用者ID
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// 使用者IP
    /// </summary>
    public string? UserIp { get; set; }

    /// <summary>
    /// 使用者代理
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// 堆疊追蹤
    /// </summary>
    public string? StackTrace { get; set; }

    /// <summary>
    /// 發生時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 錯誤訊息模板實體 (XCOMMSG)
/// </summary>
public class ErrorMessageTemplate
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 錯誤代碼
    /// </summary>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// 語言代碼
    /// </summary>
    public string Language { get; set; } = "zh-TW";

    /// <summary>
    /// 錯誤標題
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 解決方案
    /// </summary>
    public string? Solution { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

