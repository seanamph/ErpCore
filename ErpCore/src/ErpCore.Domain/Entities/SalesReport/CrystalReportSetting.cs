namespace ErpCore.Domain.Entities.SalesReport;

/// <summary>
/// Crystal Reports 報表設定 (SYS1360 - Crystal Reports報表功能)
/// </summary>
public class CrystalReportSetting
{
    /// <summary>
    /// 設定編號
    /// </summary>
    public Guid SettingId { get; set; }

    /// <summary>
    /// 報表代碼 (SYS1360)
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 報表檔案路徑
    /// </summary>
    public string? ReportPath { get; set; }

    /// <summary>
    /// 報表參數 (JSON格式)
    /// </summary>
    public string? Parameters { get; set; }

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

