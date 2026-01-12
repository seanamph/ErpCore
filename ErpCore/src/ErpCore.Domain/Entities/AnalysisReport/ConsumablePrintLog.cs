namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 耗材列印記錄 (SYSA254)
/// </summary>
public class ConsumablePrintLog
{
    /// <summary>
    /// 記錄ID
    /// </summary>
    public Guid LogId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// 耗材編號
    /// </summary>
    public string ConsumableId { get; set; } = string.Empty;

    /// <summary>
    /// 列印類型 (1:耗材管理報表, 2:耗材標籤列印)
    /// </summary>
    public string? PrintType { get; set; }

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
    /// 店別代碼
    /// </summary>
    public string? SiteId { get; set; }
}
