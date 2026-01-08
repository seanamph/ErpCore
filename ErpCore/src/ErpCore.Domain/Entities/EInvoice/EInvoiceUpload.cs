namespace ErpCore.Domain.Entities.EInvoice;

/// <summary>
/// 電子發票上傳記錄實體 (ECA3010)
/// </summary>
public class EInvoiceUpload
{
    /// <summary>
    /// 上傳記錄ID
    /// </summary>
    public long UploadId { get; set; }

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// 檔案大小 (bytes)
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 檔案類型 (Excel, XML等)
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// 上傳時間
    /// </summary>
    public DateTime UploadDate { get; set; }

    /// <summary>
    /// 上傳者
    /// </summary>
    public string? UploadBy { get; set; }

    /// <summary>
    /// 狀態 (PENDING, PROCESSING, COMPLETED, FAILED)
    /// </summary>
    public string Status { get; set; } = "PENDING";

    /// <summary>
    /// 處理開始時間
    /// </summary>
    public DateTime? ProcessStartDate { get; set; }

    /// <summary>
    /// 處理結束時間
    /// </summary>
    public DateTime? ProcessEndDate { get; set; }

    /// <summary>
    /// 總筆數
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    /// 成功筆數
    /// </summary>
    public int SuccessRecords { get; set; }

    /// <summary>
    /// 失敗筆數
    /// </summary>
    public int FailedRecords { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 處理日誌
    /// </summary>
    public string? ProcessLog { get; set; }

    /// <summary>
    /// 店別ID
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 零售商ID
    /// </summary>
    public string? RetailerId { get; set; }

    /// <summary>
    /// 上傳類型 (ECA2050, ECA3010, ECA3030等)
    /// </summary>
    public string? UploadType { get; set; } = "ECA3010";

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

