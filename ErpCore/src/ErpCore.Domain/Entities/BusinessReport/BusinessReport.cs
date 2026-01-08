namespace ErpCore.Domain.Entities.BusinessReport;

/// <summary>
/// 業務報表資料實體 (SYSL135)
/// </summary>
public class BusinessReport
{
    /// <summary>
    /// 報表主鍵
    /// </summary>
    public long ReportId { get; set; }

    /// <summary>
    /// 店別代碼
    /// </summary>
    public string SiteId { get; set; } = string.Empty;

    /// <summary>
    /// 店別名稱
    /// </summary>
    public string? SiteName { get; set; }

    /// <summary>
    /// 卡片類型
    /// </summary>
    public string CardType { get; set; } = string.Empty;

    /// <summary>
    /// 卡片類型名稱
    /// </summary>
    public string? CardTypeName { get; set; }

    /// <summary>
    /// 廠商代碼
    /// </summary>
    public string? VendorId { get; set; }

    /// <summary>
    /// 廠商名稱
    /// </summary>
    public string? VendorName { get; set; }

    /// <summary>
    /// 專櫃代碼
    /// </summary>
    public string? StoreId { get; set; }

    /// <summary>
    /// 專櫃名稱
    /// </summary>
    public string? StoreName { get; set; }

    /// <summary>
    /// 合約代碼
    /// </summary>
    public string? AgreementId { get; set; }

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
    /// 報表日期
    /// </summary>
    public DateTime ReportDate { get; set; }

    /// <summary>
    /// 狀態
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

