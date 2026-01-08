namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// 商品明細檔 (對應舊系統 GOODS_D)
/// </summary>
public class ProductDetail
{
    /// <summary>
    /// 店別代碼 (SHOP_ID) - 主鍵
    /// </summary>
    public string ShopId { get; set; } = string.Empty;

    /// <summary>
    /// 進銷碼 (GOODS_ID) - 主鍵
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 供應商代碼 (SUPP_ID)
    /// </summary>
    public string? SuppId { get; set; }

    /// <summary>
    /// 商品條碼 (BARCODE_ID)
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 中價 (MPRC)
    /// </summary>
    public decimal Mprc { get; set; }

    /// <summary>
    /// 可進貨 (CAN_BUY, Y/N)
    /// </summary>
    public string CanBuy { get; set; } = "Y";

    /// <summary>
    /// 可進貨日期 (CB_DATE)
    /// </summary>
    public DateTime? CbDate { get; set; }

    /// <summary>
    /// 可退貨 (CAN_RETURNS, Y/N)
    /// </summary>
    public string CanReturns { get; set; } = "Y";

    /// <summary>
    /// 可退貨日期 (CR_DATE)
    /// </summary>
    public DateTime? CrDate { get; set; }

    /// <summary>
    /// 可入庫 (CAN_STORE_IN, Y/N)
    /// </summary>
    public string CanStoreIn { get; set; } = "Y";

    /// <summary>
    /// 可入庫日期 (CSI_DATE)
    /// </summary>
    public DateTime? CsiDate { get; set; }

    /// <summary>
    /// 可出庫 (CAN_STORE_OUT, Y/N)
    /// </summary>
    public string CanStoreOut { get; set; } = "Y";

    /// <summary>
    /// 可出庫日期 (CSO_DATE)
    /// </summary>
    public DateTime? CsoDate { get; set; }

    /// <summary>
    /// 可銷售 (CAN_SALES, Y/N)
    /// </summary>
    public string CanSales { get; set; } = "Y";

    /// <summary>
    /// 可銷售日期 (CS_DATE)
    /// </summary>
    public DateTime? CsDate { get; set; }

    /// <summary>
    /// 是否超量 (IS_OVER, Y/N)
    /// </summary>
    public string IsOver { get; set; } = "N";

    /// <summary>
    /// 超量日期 (IO_DATE)
    /// </summary>
    public DateTime? IoDate { get; set; }

    /// <summary>
    /// 建立者 (BUSER)
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間 (BTIME)
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者 (CUSER)
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間 (CTIME)
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

