namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 現金流量中分類實體 (SYST132)
/// 對應舊系統 CASH_MTYPE
/// </summary>
public class CashFlowMediumType
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 大分類代號
    /// </summary>
    public string CashLTypeId { get; set; } = string.Empty;

    /// <summary>
    /// 中分類代號
    /// </summary>
    public string CashMTypeId { get; set; } = string.Empty;

    /// <summary>
    /// 中分類名稱
    /// </summary>
    public string? CashMTypeName { get; set; }

    /// <summary>
    /// 借貸項目 (A:借方, B:貸方)
    /// </summary>
    public string? AbItem { get; set; }

    /// <summary>
    /// 排序
    /// </summary>
    public string? Sn { get; set; }

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

