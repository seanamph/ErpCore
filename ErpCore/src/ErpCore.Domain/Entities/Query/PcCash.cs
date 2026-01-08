namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 零用金主檔 (SYSQ210)
/// </summary>
public class PcCash
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 零用金單號
    /// </summary>
    public string CashId { get; set; } = string.Empty;

    /// <summary>
    /// 分店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 申請日期
    /// </summary>
    public DateTime AppleDate { get; set; }

    /// <summary>
    /// 申請人
    /// </summary>
    public string AppleName { get; set; } = string.Empty;

    /// <summary>
    /// 申請組織代號
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 保管人代碼
    /// </summary>
    public string? KeepEmpId { get; set; }

    /// <summary>
    /// 零用金金額
    /// </summary>
    public decimal CashAmount { get; set; }

    /// <summary>
    /// 狀態 (DRAFT:草稿, APPLIED:已申請, REQUESTED:已請款, TRANSFERRED:已拋轉, INVENTORIED:已盤點, APPROVED:已審核)
    /// </summary>
    public string? CashStatus { get; set; }

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

