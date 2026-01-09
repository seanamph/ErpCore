namespace ErpCore.Domain.Entities.SalesReport;

/// <summary>
/// 銷售報表資料 (SYS1000 - 銷售報表模組系列)
/// </summary>
public class SalesReport
{
    /// <summary>
    /// 報表編號
    /// </summary>
    public string ReportId { get; set; } = string.Empty;

    /// <summary>
    /// 報表代碼 (SYS1100-SYS1D10等)
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型
    /// </summary>
    public string? ReportType { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? ShopId { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 報表資料 (JSON格式)
    /// </summary>
    public string? ReportData { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

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

