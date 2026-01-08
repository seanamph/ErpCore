namespace ErpCore.Domain.Entities.Query;

/// <summary>
/// 零用金參數 (SYSQ110)
/// </summary>
public class CashParams
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 公司單位代號
    /// </summary>
    public string? UnitId { get; set; }

    /// <summary>
    /// 銀行存款會計科目代號
    /// </summary>
    public string ApexpLid { get; set; } = string.Empty;

    /// <summary>
    /// 進項稅額會計科目代號
    /// </summary>
    public string PtaxLid { get; set; } = string.Empty;

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

