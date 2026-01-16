namespace ErpCore.Domain.Entities.Stocktaking;

/// <summary>
/// 店舖盤點記錄品暫存檔
/// </summary>
public class StocktakingTemp
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
    /// 子盤點單號
    /// </summary>
    public string? SPlanId { get; set; }

    /// <summary>
    /// 店舖代碼
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

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
    /// HT上傳量
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 人工量
    /// </summary>
    public decimal IQty { get; set; }

    /// <summary>
    /// 是否新增 (Y/N)
    /// </summary>
    public string? IsAdd { get; set; }

    /// <summary>
    /// HT狀態 (-1:申請中, 0:未審核, 1:已審核, 4:作廢)
    /// </summary>
    public string HtStatus { get; set; } = "0";

    /// <summary>
    /// 狀態
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? BUser { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime? BTime { get; set; }

    /// <summary>
    /// 審核者
    /// </summary>
    public string? ApprvId { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? ApprvDate { get; set; }

    /// <summary>
    /// 盤點日期
    /// </summary>
    public DateTime? InvDate { get; set; }

    /// <summary>
    /// 是否更新 (Y/N)
    /// </summary>
    public string? IsUpdate { get; set; }

    /// <summary>
    /// 序號
    /// </summary>
    public string? NumNo { get; set; }

    /// <summary>
    /// HT自動 (Y/N)
    /// </summary>
    public string? HtAuto { get; set; }

    /// <summary>
    /// 是否成功 (Y/N)
    /// </summary>
    public string? IsSuccess { get; set; }

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrMsg { get; set; }

    /// <summary>
    /// 是否HT (Y/N)
    /// </summary>
    public string? IsHt { get; set; }

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
}
