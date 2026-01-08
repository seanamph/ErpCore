namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// 變價單明細 (SYSW150)
/// </summary>
public class PriceChangeDetail
{
    /// <summary>
    /// 明細ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 變價單號
    /// </summary>
    public string PriceChangeId { get; set; } = string.Empty;

    /// <summary>
    /// 變價類型 (1:進價, 2:售價)
    /// </summary>
    public string PriceChangeType { get; set; } = string.Empty;

    /// <summary>
    /// 序號
    /// </summary>
    public int LineNum { get; set; }

    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 調整前單價
    /// </summary>
    public decimal BeforePrice { get; set; }

    /// <summary>
    /// 調整後單價
    /// </summary>
    public decimal AfterPrice { get; set; }

    /// <summary>
    /// 變價數量
    /// </summary>
    public decimal ChangeQty { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

