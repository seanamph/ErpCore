namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 零用金盤點檔 (SYSQ241, SYSQ242)
/// </summary>
public class PcCashInventory
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 盤點單號
    /// </summary>
    public string InventoryId { get; set; } = string.Empty;

    /// <summary>
    /// 分店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 盤點日期
    /// </summary>
    public DateTime InventoryDate { get; set; }

    /// <summary>
    /// 保管人代碼
    /// </summary>
    public string KeepEmpId { get; set; } = string.Empty;

    /// <summary>
    /// 盤點金額
    /// </summary>
    public decimal InventoryAmount { get; set; }

    /// <summary>
    /// 實際金額
    /// </summary>
    public decimal? ActualAmount { get; set; }

    /// <summary>
    /// 差異金額
    /// </summary>
    public decimal? DifferenceAmount { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? InventoryStatus { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? BUser { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime BTime { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? CUser { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime? CTime { get; set; }
}

