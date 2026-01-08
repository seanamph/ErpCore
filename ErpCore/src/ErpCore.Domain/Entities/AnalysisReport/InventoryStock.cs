namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 庫存資料表 (對應舊系統 IMS_AM.NAM_AM_STOCKS)
/// </summary>
public class InventoryStock
{
    /// <summary>
    /// 庫存ID
    /// </summary>
    public long StockId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 商品代碼
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 來源單號
    /// </summary>
    public string? SourceId { get; set; }

    /// <summary>
    /// 庫存日期
    /// </summary>
    public DateTime StocksDate { get; set; }

    /// <summary>
    /// 庫存狀態 (1:入庫, 2:出庫, 3:退貨, 4:退回, 5:報廢, 6:出售, 8:盤點)
    /// </summary>
    public string StocksStatus { get; set; } = string.Empty;

    /// <summary>
    /// 數量
    /// </summary>
    public decimal Qty { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? McAmt { get; set; }

    /// <summary>
    /// 未稅金額
    /// </summary>
    public decimal? StocksNtaxAmt { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

