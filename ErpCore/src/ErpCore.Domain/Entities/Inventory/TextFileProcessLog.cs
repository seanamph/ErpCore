namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// 文本文件處理記錄 (HT680)
/// </summary>
public class TextFileProcessLog
{
    /// <summary>
    /// 處理記錄ID
    /// </summary>
    public Guid LogId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 文件名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 文件類型 (BACK, INV, ORDER, ORDER_6, POP, PRIC)
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
    /// 處理狀態 (PENDING, PROCESSING, COMPLETED, FAILED)
    /// </summary>
    public string ProcessStatus { get; set; } = "PENDING";

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
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

