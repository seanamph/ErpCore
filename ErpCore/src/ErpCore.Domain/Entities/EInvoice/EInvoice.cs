namespace ErpCore.Domain.Entities.EInvoice;

/// <summary>
/// 電子發票主檔實體 (ECA3010)
/// </summary>
public class EInvoice
{
    /// <summary>
    /// 發票ID
    /// </summary>
    public long InvoiceId { get; set; }

    /// <summary>
    /// 上傳記錄ID
    /// </summary>
    public long? UploadId { get; set; }

    /// <summary>
    /// 訂單編號
    /// </summary>
    public string? OrderNo { get; set; }

    /// <summary>
    /// 零售商訂單編號
    /// </summary>
    public string? RetailerOrderNo { get; set; }

    /// <summary>
    /// 零售商訂單明細編號
    /// </summary>
    public string? RetailerOrderDetailNo { get; set; }

    /// <summary>
    /// 訂單日期
    /// </summary>
    public DateTime? OrderDate { get; set; }

    /// <summary>
    /// 店別ID
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 供應商ID
    /// </summary>
    public string? ProviderId { get; set; }

    /// <summary>
    /// 類型
    /// </summary>
    public string? NdType { get; set; }

    /// <summary>
    /// 商品ID
    /// </summary>
    public string? GoodsId { get; set; }

    /// <summary>
    /// 商品名稱
    /// </summary>
    public string? GoodsName { get; set; }

    /// <summary>
    /// 規格ID
    /// </summary>
    public string? SpecId { get; set; }

    /// <summary>
    /// 供應商商品ID
    /// </summary>
    public string? ProviderGoodsId { get; set; }

    /// <summary>
    /// 規格顏色
    /// </summary>
    public string? SpecColor { get; set; }

    /// <summary>
    /// 規格尺寸
    /// </summary>
    public string? SpecSize { get; set; }

    /// <summary>
    /// 建議價格
    /// </summary>
    public decimal? SuggestPrice { get; set; }

    /// <summary>
    /// 網路價格
    /// </summary>
    public decimal? InternetPrice { get; set; }

    /// <summary>
    /// 運送類型
    /// </summary>
    public string? ShippingType { get; set; }

    /// <summary>
    /// 運費
    /// </summary>
    public decimal? ShippingFee { get; set; }

    /// <summary>
    /// 訂單數量
    /// </summary>
    public int? OrderQty { get; set; }

    /// <summary>
    /// 訂單運費
    /// </summary>
    public decimal? OrderShippingFee { get; set; }

    /// <summary>
    /// 訂單小計
    /// </summary>
    public decimal? OrderSubtotal { get; set; }

    /// <summary>
    /// 訂單總計
    /// </summary>
    public decimal? OrderTotal { get; set; }

    /// <summary>
    /// 訂單狀態
    /// </summary>
    public string? OrderStatus { get; set; }

    /// <summary>
    /// 處理狀態
    /// </summary>
    public string ProcessStatus { get; set; } = "PENDING";

    /// <summary>
    /// 錯誤訊息
    /// </summary>
    public string? ErrorMessage { get; set; }

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

