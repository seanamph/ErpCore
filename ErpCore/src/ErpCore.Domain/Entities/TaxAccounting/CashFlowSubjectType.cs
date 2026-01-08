namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 現金流量科目設定實體 (SYST133)
/// 對應舊系統 CASH_STYPE
/// </summary>
public class CashFlowSubjectType
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 中分類代號
    /// </summary>
    public string CashMTypeId { get; set; } = string.Empty;

    /// <summary>
    /// 科目代號 (對應 AccountSubjects.StypeId)
    /// </summary>
    public string CashSTypeId { get; set; } = string.Empty;

    /// <summary>
    /// 借貸項目 (A:借方, B:貸方)
    /// </summary>
    public string? AbItem { get; set; }

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

