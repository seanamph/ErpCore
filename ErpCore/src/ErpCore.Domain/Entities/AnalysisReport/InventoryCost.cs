namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 庫存成本表 (對應舊系統 IMS_AM.NAM_COST)
/// </summary>
public class InventoryCost
{
    /// <summary>
    /// 成本ID
    /// </summary>
    public long CostId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 商品代碼
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 成本年月 (YYYYMM)
    /// </summary>
    public string CostYm { get; set; } = string.Empty;

    /// <summary>
    /// 上期數量
    /// </summary>
    public decimal? LastQty { get; set; }

    /// <summary>
    /// 上期單價
    /// </summary>
    public decimal? LastPrice { get; set; }

    /// <summary>
    /// 上期金額
    /// </summary>
    public decimal? LastAmt { get; set; }

    /// <summary>
    /// 下期數量
    /// </summary>
    public decimal? NextQty { get; set; }

    /// <summary>
    /// 下期單價
    /// </summary>
    public decimal? NextPrice { get; set; }

    /// <summary>
    /// 下期金額
    /// </summary>
    public decimal? NextAmt { get; set; }

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

