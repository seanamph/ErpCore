namespace ErpCore.Domain.Entities.AnalysisReport;

/// <summary>
/// 單位領用申請單主檔 (SYSA210)
/// </summary>
public class MaterialApplyMaster
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// 領用單號
    /// </summary>
    public string ApplyId { get; set; } = string.Empty;

    /// <summary>
    /// 申請人代號
    /// </summary>
    public string EmpId { get; set; } = string.Empty;

    /// <summary>
    /// 部門代號
    /// </summary>
    public string OrgId { get; set; } = string.Empty;

    /// <summary>
    /// 分店代號
    /// </summary>
    public string? SiteId { get; set; }

    /// <summary>
    /// 申請日期
    /// </summary>
    public DateTime ApplyDate { get; set; }

    /// <summary>
    /// 領用單狀態 (0:待審核, 1:已審核, 2:已發料, 3:已取消)
    /// </summary>
    public string ApplyStatus { get; set; } = "0";

    /// <summary>
    /// 領用品總價值
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// 審核者
    /// </summary>
    public string? AprvEmpId { get; set; }

    /// <summary>
    /// 審核日期
    /// </summary>
    public DateTime? AprvDate { get; set; }

    /// <summary>
    /// 發料日期
    /// </summary>
    public DateTime? CheckDate { get; set; }

    /// <summary>
    /// 倉別
    /// </summary>
    public string? WhId { get; set; }

    /// <summary>
    /// 儲位
    /// </summary>
    public string? StoreId { get; set; }

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

    /// <summary>
    /// 明細列表
    /// </summary>
    public List<MaterialApplyDetail> Details { get; set; } = new();
}
