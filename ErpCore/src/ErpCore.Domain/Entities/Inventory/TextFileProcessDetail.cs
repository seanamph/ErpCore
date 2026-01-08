namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// 文本文件處理明細 (HT680)
/// </summary>
public class TextFileProcessDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 處理記錄ID
    /// </summary>
    public Guid LogId { get; set; }

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNumber { get; set; }

    /// <summary>
    /// 原始資料
    /// </summary>
    public string? RawData { get; set; }

    /// <summary>
    /// 處理狀態 (PENDING, SUCCESS, FAILED)
    /// </summary>
    public string ProcessStatus { get; set; } = "PENDING";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 處理後的資料 (JSON格式)
    /// </summary>
    public string? ProcessedData { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

