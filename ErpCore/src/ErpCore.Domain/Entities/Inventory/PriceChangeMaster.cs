namespace ErpCore.Domain.Entities.Inventory;

/// <summary>
/// 變價單主檔 (SYSW150)
/// </summary>
public class PriceChangeMaster
{
    /// <summary>
    /// 變價單號
    /// </summary>
    public string PriceChangeId { get; set; } = string.Empty;

    /// <summary>
    /// 變價類型 (1:進價, 2:售價)
    /// </summary>
    public string PriceChangeType { get; set; } = string.Empty;

    /// <summary>
    /// 廠商編號
    /// </summary>
    public string? SupplierId { get; set; }

    /// <summary>
    /// 品牌編號
    /// </summary>
    public string? LogoId { get; set; }

    /// <summary>
    /// 申請人員編號
    /// </summary>
    public string? ApplyEmpId { get; set; }

    /// <summary>
    /// 申請單位
    /// </summary>
    public string? ApplyOrgId { get; set; }

    /// <summary>
    /// 申請日期
    /// </summary>
    public DateTime? ApplyDate { get; set; }

    /// <summary>
    /// 啟用日期
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// 審核人員編號
    /// </summary>
    public string? ApproveEmpId { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? ApproveDate { get; set; }

    /// <summary>
    /// 確認人員編號
    /// </summary>
    public string? ConfirmEmpId { get; set; }

    /// <summary>
    /// 確認日期
    /// </summary>
    public DateTime? ConfirmDate { get; set; }

    /// <summary>
    /// 狀態 (1:已申請, 2:已審核, 9:已作廢, 10:已確認)
    /// </summary>
    public string Status { get; set; } = "1";

    /// <summary>
    /// 總金額
    /// </summary>
    public decimal TotalAmount { get; set; }

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

    /// <summary>
    /// 建立者等級
    /// </summary>
    public int? CreatedPriority { get; set; }

    /// <summary>
    /// 建立者群組
    /// </summary>
    public string? CreatedGroup { get; set; }
}

