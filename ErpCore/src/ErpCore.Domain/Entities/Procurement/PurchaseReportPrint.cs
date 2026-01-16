namespace ErpCore.Domain.Entities.Procurement;

/// <summary>
/// 採購報表列印記錄 (採購報表列印)
/// </summary>
public class PurchaseReportPrint
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 報表類型 (PO:採購單, SU:供應商, PY:付款單)
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 報表代碼
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 列印日期
    /// </summary>
    public DateTime PrintDate { get; set; }

    /// <summary>
    /// 列印使用者
    /// </summary>
    public string PrintUserId { get; set; } = string.Empty;

    /// <summary>
    /// 列印使用者名稱
    /// </summary>
    public string? PrintUserName { get; set; }

    /// <summary>
    /// 篩選條件 (JSON格式)
    /// </summary>
    public string? FilterConditions { get; set; }

    /// <summary>
    /// 列印設定 (JSON格式)
    /// </summary>
    public string? PrintSettings { get; set; }

    /// <summary>
    /// 檔案格式 (PDF, Excel)
    /// </summary>
    public string? FileFormat { get; set; }

    /// <summary>
    /// 檔案路徑
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 檔案大小 (bytes)
    /// </summary>
    public long? FileSize { get; set; }

    /// <summary>
    /// 狀態 (S:成功, F:失敗, P:處理中)
    /// </summary>
    public string Status { get; set; } = "S";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 頁數
    /// </summary>
    public int? PageCount { get; set; }

    /// <summary>
    /// 記錄數
    /// </summary>
    public int? RecordCount { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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
