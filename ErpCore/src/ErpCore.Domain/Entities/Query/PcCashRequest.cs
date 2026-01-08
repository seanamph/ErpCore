namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 零用金請款檔 (SYSQ220)
/// </summary>
public class PcCashRequest
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 請款單號
    /// </summary>
    public string RequestId { get; set; } = string.Empty;

    /// <summary>
    /// 分店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 請款日期
    /// </summary>
    public DateTime RequestDate { get; set; }

    /// <summary>
    /// 零用金單號列表 (JSON格式)
    /// </summary>
    public string? CashIds { get; set; }

    /// <summary>
    /// 請款金額
    /// </summary>
    public decimal RequestAmount { get; set; }

    /// <summary>
    /// 狀態
    /// </summary>
    public string? RequestStatus { get; set; }

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

