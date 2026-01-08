namespace ErpCore.Domain.Entities.Communication;

/// <summary>
/// 郵件附件實體
/// </summary>
public class EmailAttachment
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 郵件記錄ID
    /// </summary>
    public long EmailLogId { get; set; }

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 檔案路徑
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 檔案大小（位元組）
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 內容類型
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

