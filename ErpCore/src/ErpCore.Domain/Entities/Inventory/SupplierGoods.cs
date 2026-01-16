namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// 供應商商品資料實體 (SYSW110)
/// </summary>
public class SupplierGoods
{
    /// <summary>
    /// 供應商編號 (主鍵)
    /// </summary>
    public string SupplierId { get; set; } = string.Empty;

    /// <summary>
    /// 商品條碼 (主鍵)
    /// </summary>
    public string BarcodeId { get; set; } = string.Empty;

    /// <summary>
    /// 店別代碼 (主鍵)
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 進價
    /// </summary>
    public decimal? Lprc { get; set; } = 0;

    /// <summary>
    /// 中價
    /// </summary>
    public decimal? Mprc { get; set; } = 0;

    /// <summary>
    /// 稅別 (1:應稅, 0:免稅)
    /// </summary>
    public string? Tax { get; set; } = "1";

    /// <summary>
    /// 最小訂購量
    /// </summary>
    public decimal? MinQty { get; set; } = 0;

    /// <summary>
    /// 最大訂購量
    /// </summary>
    public decimal? MaxQty { get; set; } = 0;

    /// <summary>
    /// 商品單位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 換算率
    /// </summary>
    public decimal? Rate { get; set; } = 1;

    /// <summary>
    /// 狀態 (0:正常, 1:停用)
    /// </summary>
    public string? Status { get; set; } = "0";

    /// <summary>
    /// 有效起始日
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 有效終止日
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 促銷價格
    /// </summary>
    public decimal? Slprc { get; set; } = 0;

    /// <summary>
    /// 到貨天數
    /// </summary>
    public int? ArrivalDays { get; set; } = 0;

    /// <summary>
    /// 週一可訂購 (Y/N)
    /// </summary>
    public string? OrdDay1 { get; set; } = "Y";

    /// <summary>
    /// 週二可訂購 (Y/N)
    /// </summary>
    public string? OrdDay2 { get; set; } = "Y";

    /// <summary>
    /// 週三可訂購 (Y/N)
    /// </summary>
    public string? OrdDay3 { get; set; } = "Y";

    /// <summary>
    /// 週四可訂購 (Y/N)
    /// </summary>
    public string? OrdDay4 { get; set; } = "Y";

    /// <summary>
    /// 週五可訂購 (Y/N)
    /// </summary>
    public string? OrdDay5 { get; set; } = "Y";

    /// <summary>
    /// 週六可訂購 (Y/N)
    /// </summary>
    public string? OrdDay6 { get; set; } = "Y";

    /// <summary>
    /// 週日可訂購 (Y/N)
    /// </summary>
    public string? OrdDay7 { get; set; } = "Y";

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}
