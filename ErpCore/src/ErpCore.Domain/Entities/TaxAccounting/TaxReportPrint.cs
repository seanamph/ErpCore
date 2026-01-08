namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 稅務報表列印記錄實體 (SYST510-SYST530)
/// </summary>
public class TaxReportPrint
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 報表類型 (SYST510/SYST520/SYST530)
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 報表日期
    /// </summary>
    public DateTime ReportDate { get; set; }

    /// <summary>
    /// 查詢起始日期
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// 查詢結束日期
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// 公司代號
    /// </summary>
    public string? CompId { get; set; }

    /// <summary>
    /// 檔案名稱
    /// </summary>
    public string? FileName { get; set; }

    /// <summary>
    /// 檔案格式 (CSV/PDF/EXCEL)
    /// </summary>
    public string? FileFormat { get; set; }

    /// <summary>
    /// 列印狀態 (1:成功, 2:失敗)
    /// </summary>
    public string PrintStatus { get; set; } = "1";

    /// <summary>
    /// 列印次數
    /// </summary>
    public int PrintCount { get; set; }

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
}

