namespace ErpCore.Domain.Entities.Tools;

/// <summary>
/// PDF轉換記錄實體
/// </summary>
public class PdfConversionLog
{
    /// <summary>
    /// 記錄編號
    /// </summary>
    public Guid LogId { get; set; }

    /// <summary>
    /// 來源HTML內容
    /// </summary>
    public string? SourceHtml { get; set; }

    /// <summary>
    /// PDF檔案路徑
    /// </summary>
    public string? PdfFilePath { get; set; }

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 檔案大小（位元組）
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 轉換狀態（PENDING, SUCCESS, FAILED）
    /// </summary>
    public string ConversionStatus { get; set; } = "PENDING";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 完成時間
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

