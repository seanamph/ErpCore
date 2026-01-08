namespace ErpCore.Domain.Entities.TaxAccounting;

/// <summary>
/// 現金流量小計設定實體 (SYST134)
/// 對應舊系統 CASH_SUB
/// </summary>
public class CashFlowSubTotal
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
    /// 小計代號
    /// </summary>
    public string CashSubId { get; set; } = string.Empty;

    /// <summary>
    /// 小計名稱
    /// </summary>
    public string CashSubName { get; set; } = string.Empty;

    /// <summary>
    /// 中分類代號起
    /// </summary>
    public string? CashMTypeIdB { get; set; }

    /// <summary>
    /// 中分類代號迄
    /// </summary>
    public string? CashMTypeIdE { get; set; }

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

