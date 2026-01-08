namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 員工餐卡申請資料實體 (SYSL130)
/// </summary>
public class EmployeeMealCard
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 員工編號
    /// </summary>
    public string EmpId { get; set; } = string.Empty;

    /// <summary>
    /// 員工姓名
    /// </summary>
    public string? EmpName { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 卡片類型
    /// </summary>
    public string? CardType { get; set; }

    /// <summary>
    /// 動作類型
    /// </summary>
    public string? ActionType { get; set; }

    /// <summary>
    /// 動作類型明細
    /// </summary>
    public string? ActionTypeD { get; set; }

    /// <summary>
    /// 起始日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 結束日期
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// 狀態 (P:待審核, A:已審核, R:已拒絕)
    /// </summary>
    public string Status { get; set; } = "P";

    /// <summary>
    /// 審核者
    /// </summary>
    public string? Verifier { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? VerifyDate { get; set; }

    /// <summary>
    /// 備註
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// 交易單號
    /// </summary>
    public string? TxnNo { get; set; }

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

