namespace ErpCore.Domain.Entities.Stocktaking;

/// <summary>
/// 盤點計劃主檔
/// </summary>
public class StocktakingPlan
{
    /// <summary>
    /// 盤點計劃單號
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// 盤點日期
    /// </summary>
    public DateTime PlanDate { get; set; }

    /// <summary>
    /// 開始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 開始時間
    /// </summary>
    public DateTime? StartTime { get; set; }

    /// <summary>
    /// 結束時間
    /// </summary>
    public DateTime? EndTime { get; set; }

    /// <summary>
    /// 盤點類型
    /// </summary>
    public string? SakeType { get; set; }

    /// <summary>
    /// 盤點部門
    /// </summary>
    public string? SakeDept { get; set; }

    /// <summary>
    /// 計劃狀態 (-1:申請中, 0:未審核, 1:已審核, 4:作廢, 5:結案)
    /// </summary>
    public string PlanStatus { get; set; } = "0";

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
