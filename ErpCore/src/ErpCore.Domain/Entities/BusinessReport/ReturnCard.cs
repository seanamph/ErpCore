namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 銷退卡資料實體 (SYSL310)
/// </summary>
public class ReturnCard
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// UUID
    /// </summary>
    public Guid Uuid { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 店別名稱
    /// </summary>
    public string? SiteName { get; set; }

    /// <summary>
    /// 組織代碼
    /// </summary>
    public string? OrgId { get; set; }

    /// <summary>
    /// 組織名稱
    /// </summary>
    public string? OrgName { get; set; }

    /// <summary>
    /// 卡片年度
    /// </summary>
    public int CardYear { get; set; }

    /// <summary>
    /// 卡片月份
    /// </summary>
    public int CardMonth { get; set; }

    /// <summary>
    /// 卡片類型
    /// </summary>
    public string? CardType { get; set; }

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

/// <summary>
/// 銷退卡明細資料實體 (SYSL310)
/// </summary>
public class ReturnCardDetail
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 銷退卡主檔ID
    /// </summary>
    public long ReturnCardId { get; set; }

    /// <summary>
    /// 員工編號
    /// </summary>
    public string? EmployeeId { get; set; }

    /// <summary>
    /// 員工姓名
    /// </summary>
    public string? EmployeeName { get; set; }

    /// <summary>
    /// 銷退日期
    /// </summary>
    public DateTime? ReturnDate { get; set; }

    /// <summary>
    /// 銷退原因
    /// </summary>
    public string? ReturnReason { get; set; }

    /// <summary>
    /// 金額
    /// </summary>
    public decimal Amount { get; set; }

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

