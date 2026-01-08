namespace ErpCore.Domain.Entities.Lease;

/// <summary>
/// 費用主檔 (SYSE310-SYSE430)
/// </summary>
public class LeaseFee
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 費用編號
    /// </summary>
    public string FeeId { get; set; } = string.Empty;

    /// <summary>
    /// 租賃編號
    /// </summary>
    public string LeaseId { get; set; } = string.Empty;

    /// <summary>
    /// 費用類型 (RENT:租金, MANAGEMENT:管理費, UTILITY:水電費, OTHER:其他費用)
    /// </summary>
    public string FeeType { get; set; } = string.Empty;

    /// <summary>
    /// 費用項目編號
    /// </summary>
    public string? FeeItemId { get; set; }

    /// <summary>
    /// 費用項目名稱
    /// </summary>
    public string? FeeItemName { get; set; }

    /// <summary>
    /// 費用金額
    /// </summary>
    public decimal FeeAmount { get; set; } = 0;

    /// <summary>
    /// 費用日期
    /// </summary>
    public DateTime FeeDate { get; set; }

    /// <summary>
    /// 到期日期
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// 繳費日期
    /// </summary>
    public DateTime? PaidDate { get; set; }

    /// <summary>
    /// 已繳金額
    /// </summary>
    public decimal PaidAmount { get; set; } = 0;

    /// <summary>
    /// 狀態 (P:待繳, P:部分繳, F:已繳, C:已取消)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 幣別
    /// </summary>
    public string? CurrencyId { get; set; } = "TWD";

    /// <summary>
    /// 匯率
    /// </summary>
    public decimal ExchangeRate { get; set; } = 1;

    /// <summary>
    /// 稅率
    /// </summary>
    public decimal TaxRate { get; set; } = 0;

    /// <summary>
    /// 稅額
    /// </summary>
    public decimal TaxAmount { get; set; } = 0;

    /// <summary>
    /// 總金額 (含稅)
    /// </summary>
    public decimal TotalAmount { get; set; } = 0;

    /// <summary>
    /// 備註
    /// </summary>
    public string? Memo { get; set; }

    /// <summary>
    /// 分公司代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 建立人員
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新人員
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

