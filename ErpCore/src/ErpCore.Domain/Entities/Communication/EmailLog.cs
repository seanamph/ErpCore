namespace ErpCore.Domain.Entities.Communication;

/// <summary>
/// 郵件發送記錄實體
/// </summary>
public class EmailLog
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 寄件者地址
    /// </summary>
    public string FromAddress { get; set; } = string.Empty;

    /// <summary>
    /// 寄件者名稱
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    /// 收件者地址（可多個，逗號分隔）
    /// </summary>
    public string ToAddress { get; set; } = string.Empty;

    /// <summary>
    /// 副本地址
    /// </summary>
    public string? CcAddress { get; set; }

    /// <summary>
    /// 密件副本地址
    /// </summary>
    public string? BccAddress { get; set; }

    /// <summary>
    /// 郵件主旨
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// 郵件內容
    /// </summary>
    public string? Body { get; set; }

    /// <summary>
    /// 內容類型 (Text/HTML)
    /// </summary>
    public string? BodyType { get; set; } = "Text";

    /// <summary>
    /// 優先權 (1-5)
    /// </summary>
    public int? Priority { get; set; } = 3;

    /// <summary>
    /// 狀態 (Pending/Sent/Failed)
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 發送時間
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// SMTP伺服器
    /// </summary>
    public string? SmtpServer { get; set; }

    /// <summary>
    /// SMTP埠號
    /// </summary>
    public int? SmtpPort { get; set; }

    /// <summary>
    /// 是否有附件
    /// </summary>
    public bool HasAttachment { get; set; }

    /// <summary>
    /// 附件數量
    /// </summary>
    public int? AttachmentCount { get; set; } = 0;
}

