namespace ErpCore.Domain.Entities.Stocktaking;

/// <summary>
/// 盤點單明細
/// </summary>
public class StocktakingDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// 盤點計劃單號
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// 店舖代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 帳面數量
    /// </summary>
    public decimal BookQty { get; set; }

    /// <summary>
    /// 實盤數量
    /// </summary>
    public decimal PhysicalQty { get; set; }

    /// <summary>
    /// 差異數量
    /// </summary>
    public decimal DiffQty { get; set; }

    /// <summary>
    /// 單位成本
    /// </summary>
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// 差異金額
    /// </summary>
    public decimal? DiffAmount { get; set; }

    /// <summary>
    /// 盤點區域
    /// </summary>
    public string? Kind { get; set; }

    /// <summary>
    /// 盤點貨架
    /// </summary>
    public string? ShelfNo { get; set; }

    /// <summary>
    /// 盤點貨架序號
    /// </summary>
    public int? SerialNo { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
