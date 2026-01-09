namespace ErpCore.Domain.Entities.CustomerInvoice;

/// <summary>
/// 郵件傳真作業記錄實體 (SYS2000 - 郵件傳真作業)
/// </summary>
public class EmailFaxJob
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 作業編號
    /// </summary>
    public string JobId { get; set; } = string.Empty;

    /// <summary>
    /// 作業類型 (EMAIL:郵件, FAX:傳真)
    /// </summary>
    public string JobType { get; set; } = string.Empty;

    /// <summary>
    /// 主旨
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 收件人
    /// </summary>
    public string Recipient { get; set; } = string.Empty;

    /// <summary>
    /// 副本
    /// </summary>
    public string? Cc { get; set; }

    /// <summary>
    /// 密件副本
    /// </summary>
    public string? Bcc { get; set; }

    /// <summary>
    /// 內容
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 附件路徑
    /// </summary>
    public string? AttachmentPath { get; set; }

    /// <summary>
    /// 狀態 (PENDING:待發送, SENT:已發送, FAILED:失敗)
    /// </summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>
    /// 發送日期
    /// </summary>
    public DateTime? SendDate { get; set; }

    /// <summary>
    /// 發送人員
    /// </summary>
    public string? SendUser { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 重試次數
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 最大重試次數
    /// </summary>
    public int MaxRetry { get; set; }

    /// <summary>
    /// 排程日期
    /// </summary>
    public DateTime? ScheduleDate { get; set; }

    /// <summary>
    /// 範本編號
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 郵件傳真範本實體
/// </summary>
public class EmailFaxTemplate
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 範本編號
    /// </summary>
    public string TemplateId { get; set; } = string.Empty;

    /// <summary>
    /// 範本名稱
    /// </summary>
    public string TemplateName { get; set; } = string.Empty;

    /// <summary>
    /// 範本類型 (EMAIL, FAX)
    /// </summary>
    public string TemplateType { get; set; } = string.Empty;

    /// <summary>
    /// 主旨範本
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// 內容範本
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// 變數定義 (JSON格式)
    /// </summary>
    public string? Variables { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 郵件傳真發送記錄實體
/// </summary>
public class EmailFaxLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 作業編號
    /// </summary>
    public string JobId { get; set; } = string.Empty;

    /// <summary>
    /// 記錄日期
    /// </summary>
    public DateTime LogDate { get; set; }

    /// <summary>
    /// 記錄類型 (SEND, RETRY, ERROR, SUCCESS)
    /// </summary>
    public string LogType { get; set; } = string.Empty;

    /// <summary>
    /// 記錄訊息
    /// </summary>
    public string? LogMessage { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

