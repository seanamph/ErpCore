namespace ErpCore.Domain.Entities.Stocktaking;

/// <summary>
/// 盤點計劃店舖檔
/// </summary>
public class StocktakingPlanShop
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 盤點計劃單號
    /// </summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>
    /// 店舖代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 狀態 (0:計劃, 1:確認, 2:盤點中, 3:計算, 4:帳面庫存, 5:作廢, 6:結案, 7:認列完成)
    /// </summary>
    public string Status { get; set; } = "0";

    /// <summary>
    /// 盤點狀態
    /// </summary>
    public string? InvStatus { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
