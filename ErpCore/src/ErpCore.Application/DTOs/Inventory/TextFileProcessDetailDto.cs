namespace ErpCore.Application.DTOs.Inventory;

/// <summary>
/// 文本文件處理明細DTO
/// </summary>
public class TextFileProcessDetailDto
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

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
    /// 處理狀態
    /// </summary>
    public string ProcessStatus { get; set; } = string.Empty;

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 處理後的資料
    /// </summary>
    public object? ProcessedData { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 文本文件處理明細查詢DTO
/// </summary>
public class TextFileProcessDetailQueryDto
{
    /// <summary>
    /// 頁碼
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每頁筆數
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 處理狀態
    /// </summary>
    public string? ProcessStatus { get; set; }
}

