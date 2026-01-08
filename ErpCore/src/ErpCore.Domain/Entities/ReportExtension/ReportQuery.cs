namespace ErpCore.Domain.Entities.ReportExtension;

/// <summary>
/// 報表查詢設定實體
/// </summary>
public class ReportQuery
{
    /// <summary>
    /// 查詢ID
    /// </summary>
    public Guid QueryId { get; set; }

    /// <summary>
    /// 報表代碼
    /// </summary>
    public string ReportCode { get; set; } = string.Empty;

    /// <summary>
    /// 報表名稱
    /// </summary>
    public string ReportName { get; set; } = string.Empty;

    /// <summary>
    /// 查詢名稱
    /// </summary>
    public string? QueryName { get; set; }

    /// <summary>
    /// 查詢參數 (JSON格式)
    /// </summary>
    public string? QueryParams { get; set; }

    /// <summary>
    /// 查詢SQL
    /// </summary>
    public string? QuerySql { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

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

