namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// POP列印記錄 (SYSW170)
/// </summary>
public class PopPrintLog
{
    /// <summary>
    /// 記錄ID
    /// </summary>
    public Guid LogId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 商品編號
    /// </summary>
    public string GoodsId { get; set; } = string.Empty;

    /// <summary>
    /// 列印類型 (POP, PRODUCT_CARD)
    /// </summary>
    public string? PrintType { get; set; }

    /// <summary>
    /// 列印格式 (PR1, PR2, PR3, PR4, PR5, PR6, PR1_AP, PR2_AP等)
    /// </summary>
    public string? PrintFormat { get; set; }

    /// <summary>
    /// 版本標記 (AP, UA, STANDARD)
    /// </summary>
    public string? Version { get; set; }

    /// <summary>
    /// 列印數量
    /// </summary>
    public int PrintCount { get; set; } = 1;

    /// <summary>
    /// 列印日期
    /// </summary>
    public DateTime PrintDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 列印者
    /// </summary>
    public string? PrintedBy { get; set; }

    /// <summary>
    /// 店別編號
    /// </summary>
    public string? ShopId { get; set; }
}

