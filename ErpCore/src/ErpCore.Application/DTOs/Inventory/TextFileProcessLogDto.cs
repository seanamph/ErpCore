namespace ErpCore.Application.DTOs.Inventory;

/// <summary>
/// 文本文件處理記錄DTO
/// </summary>
public class TextFileProcessLogDto
{
    /// <summary>
    /// 處理記錄ID
    /// </summary>
    public Guid LogId { get; set; }

    /// <summary>
    /// 文件名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件類型
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 總記錄數
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// 成功記錄數
    /// </summary>
    public int SuccessRecords { get; set; }

    /// <summary>
    /// 失敗記錄數
    /// </summary>
    public int FailedRecords { get; set; }

    /// <summary>
    /// 處理狀態
    /// </summary>
    public string ProcessStatus { get; set; } = string.Empty;

    /// <summary>
    /// 處理開始時間
    /// </summary>
    public DateTime? ProcessStartTime { get; set; }

    /// <summary>
    /// 處理結束時間
    /// </summary>
    public DateTime? ProcessEndTime { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 文本文件處理記錄查詢DTO
/// </summary>
public class TextFileProcessLogQueryDto
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
    /// 排序欄位
    /// </summary>
    public string? SortField { get; set; } = "CreatedAt";

    /// <summary>
    /// 排序方向
    /// </summary>
    public string? SortOrder { get; set; } = "DESC";

    /// <summary>
    /// 文件名稱
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 文件類型
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 處理狀態
    /// </summary>
    public string? ProcessStatus { get; set; }
}

