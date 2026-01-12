namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 耗材異動記錄 (SYSA255)
/// </summary>
public class ConsumableTransaction
{
    /// <summary>
    /// 異動記錄ID
    /// </summary>
    public long TransactionId { get; set; }

    /// <summary>
    /// 耗材編號
    /// </summary>
    public string ConsumableId { get; set; } = string.Empty;

    /// <summary>
    /// 異動類型 (1:入庫, 2:出庫, 3:退貨, 4:報廢, 5:出售, 6:領用)
    /// </summary>
    public string TransactionType { get; set; } = string.Empty;

    /// <summary>
    /// 異動日期
    /// </summary>
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// 數量
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// 單價
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal? Amount { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 庫別代碼
    /// </summary>
    public string? WarehouseId { get; set; }

    /// <summary>
    /// 來源單號
    /// </summary>
    public string? SourceId { get; set; }

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
}
