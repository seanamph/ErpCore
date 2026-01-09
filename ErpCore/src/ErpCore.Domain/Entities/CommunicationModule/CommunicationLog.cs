namespace ErpCore.Domain.Entities.CommunicationModule;

/// <summary>
/// 通訊記錄實體 (XCOM000)
/// </summary>
public class CommunicationLog
{
    /// <summary>
    /// 記錄ID
    /// </summary>
    public long LogId { get; set; }

    /// <summary>
    /// 通訊ID
    /// </summary>
    public long CommunicationId { get; set; }

    /// <summary>
    /// 系統代碼
    /// </summary>
    public string SystemCode { get; set; } = string.Empty;

    /// <summary>
    /// 操作類型 (SEND, RECEIVE, SYNC)
    /// </summary>
    public string OperationType { get; set; } = string.Empty;

    /// <summary>
    /// 請求資料 (JSON格式)
    /// </summary>
    public string? RequestData { get; set; }

    /// <summary>
    /// 回應資料 (JSON格式)
    /// </summary>
    public string? ResponseData { get; set; }

    /// <summary>
    /// 狀態 (SUCCESS, FAILED, PENDING)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 執行時間 (毫秒)
    /// </summary>
    public int? Duration { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

