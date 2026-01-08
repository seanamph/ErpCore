namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 保管人及額度設定 (SYSQ120)
/// </summary>
public class PcKeep
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 分店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 保管人代碼
    /// </summary>
    public string KeepEmpId { get; set; } = string.Empty;

    /// <summary>
    /// 零用金額度
    /// </summary>
    public decimal? PcQuota { get; set; } = 0;

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CGroup { get; set; }
}

