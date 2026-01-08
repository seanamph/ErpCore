namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 員餐卡交易明細資料實體 (SYSL210)
/// </summary>
public class EmployeeMealCardTransaction
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string TxnNo { get; set; } = string.Empty;

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 店別名稱
    /// </summary>
    public string? SiteName { get; set; }

    /// <summary>
    /// 卡片表面ID
    /// </summary>
    public string? CardSurfaceId { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 組織名稱
    /// </summary>
    public string? OrgName { get; set; }

    /// <summary>
    /// 動作類型
    /// </summary>
    public string? ActionType { get; set; }

    /// <summary>
    /// 動作類型名稱
    /// </summary>
    public string? ActionTypeName { get; set; }

    /// <summary>
    /// 年月 (YYYYMM)
    /// </summary>
    public string? YearMonth { get; set; }

    /// <summary>
    /// 金額1
    /// </summary>
    public decimal Amt1 { get; set; }

    /// <summary>
    /// 金額4
    /// </summary>
    public decimal Amt4 { get; set; }

    /// <summary>
    /// 金額5
    /// </summary>
    public decimal Amt5 { get; set; }

    /// <summary>
    /// 狀態 (1:啟用, 0:停用)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

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

