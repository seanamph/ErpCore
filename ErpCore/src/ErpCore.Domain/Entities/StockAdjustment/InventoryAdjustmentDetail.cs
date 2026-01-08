namespace ErpCore.Domain.Entities.StockAdjustment;

/// <summary>
/// 庫存調整單明細
/// </summary>
public class InventoryAdjustmentDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public Guid DetailId { get; set; }

    /// <summary>
    /// 調整單號
    /// </summary>
    public string AdjustmentId { get; set; } = string.Empty;

    /// <summary>
    /// 行號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 條碼編號
    /// </summary>
    public string? BarcodeId { get; set; }

    /// <summary>
    /// 調整數量
    /// </summary>
    public decimal AdjustmentQty { get; set; }

    /// <summary>
    /// 調整前數量
    /// </summary>
    public decimal? BeforeQty { get; set; }

    /// <summary>
    /// 調整後數量
    /// </summary>
    public decimal? AfterQty { get; set; }

    /// <summary>
    /// 單位成本
    /// </summary>
    public decimal? UnitCost { get; set; }

    /// <summary>
    /// 調整成本
    /// </summary>
    public decimal? AdjustmentCost { get; set; }

    /// <summary>
    /// 調整金額
    /// </summary>
    public decimal? AdjustmentAmount { get; set; }

    /// <summary>
    /// 調整原因
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

