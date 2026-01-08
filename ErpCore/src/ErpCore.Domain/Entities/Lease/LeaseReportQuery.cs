namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 租賃報表查詢記錄 (SYSM141-SYSM144)
/// </summary>
public class LeaseReportQuery
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 查詢編號
    /// </summary>
    public string QueryId { get; set; } = string.Empty;

    /// <summary>
    /// 報表類型 (SYSM141:租賃清單, SYSM142:租賃明細, SYSM143:租賃統計, SYSM144:租賃分析)
    /// </summary>
    public string ReportType { get; set; } = string.Empty;

    /// <summary>
    /// 查詢名稱
    /// </summary>
    public string? QueryName { get; set; }

    /// <summary>
    /// 查詢參數 (JSON格式)
    /// </summary>
    public string? QueryParams { get; set; }

    /// <summary>
    /// 查詢結果 (JSON格式)
    /// </summary>
    public string? QueryResult { get; set; }

    /// <summary>
    /// 查詢日期
    /// </summary>
    public DateTime QueryDate { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

