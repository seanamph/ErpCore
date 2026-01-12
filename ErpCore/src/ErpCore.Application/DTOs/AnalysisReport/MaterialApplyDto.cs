using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.AnalysisReport;

/// <summary>
/// 單位領用申請單 DTO (SYSA210)
/// </summary>
public class MaterialApplyDto
{
    public long TKey { get; set; }
    public string ApplyId { get; set; } = string.Empty;
    public string EmpId { get; set; } = string.Empty;
    public string? EmpName { get; set; }
    public string OrgId { get; set; } = string.Empty;
    public string? OrgName { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public DateTime ApplyDate { get; set; }
    public string ApplyStatus { get; set; } = "0";
    public string? ApplyStatusName { get; set; }
    public decimal Amount { get; set; }
    public string? AprvEmpId { get; set; }
    public string? AprvEmpName { get; set; }
    public DateTime? AprvDate { get; set; }
    public DateTime? CheckDate { get; set; }
    public string? WhId { get; set; }
    public string? StoreId { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 單位領用申請單詳細 DTO（含明細）
/// </summary>
public class MaterialApplyDetailDto : MaterialApplyDto
{
    public List<MaterialApplyDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// 單位領用申請單明細項目 DTO
/// </summary>
public class MaterialApplyDetailItemDto
{
    public long TKey { get; set; }
    public long GoodsTKey { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public decimal ApplyQty { get; set; }
    public decimal? IssueQty { get; set; }
    public string? Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    public int SeqNo { get; set; }
}

/// <summary>
/// 新增單位領用申請單 DTO
/// </summary>
public class CreateMaterialApplyDto
{
    public string? ApplyId { get; set; }
    public string EmpId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public DateTime ApplyDate { get; set; }
    public string? WhId { get; set; }
    public string? StoreId { get; set; }
    public string? Notes { get; set; }
    public List<CreateMaterialApplyDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 新增單位領用申請單明細 DTO
/// </summary>
public class CreateMaterialApplyDetailDto
{
    public long GoodsTKey { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public decimal ApplyQty { get; set; }
    public decimal UnitPrice { get; set; }
    public string? Notes { get; set; }
    public int SeqNo { get; set; }
}

/// <summary>
/// 更新單位領用申請單 DTO
/// </summary>
public class UpdateMaterialApplyDto
{
    public string EmpId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public DateTime ApplyDate { get; set; }
    public string? WhId { get; set; }
    public string? StoreId { get; set; }
    public string? Notes { get; set; }
    public List<CreateMaterialApplyDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 審核單位領用申請單 DTO
/// </summary>
public class ApproveMaterialApplyDto
{
    public string AprvEmpId { get; set; } = string.Empty;
    public DateTime AprvDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 發料作業 DTO
/// </summary>
public class IssueMaterialApplyDto
{
    public DateTime CheckDate { get; set; }
    public List<IssueMaterialApplyDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 發料作業明細 DTO
/// </summary>
public class IssueMaterialApplyDetailDto
{
    public long TKey { get; set; }
    public decimal IssueQty { get; set; }
}

/// <summary>
/// 批次新增單位領用申請單 DTO
/// </summary>
public class BatchCreateMaterialApplyDto
{
    public string EmpId { get; set; } = string.Empty;
    public string OrgId { get; set; } = string.Empty;
    public string? SiteId { get; set; }
    public DateTime ApplyDate { get; set; }
    public List<BatchCreateMaterialApplyItemDto> Items { get; set; } = new();
}

/// <summary>
/// 批次新增項目 DTO
/// </summary>
public class BatchCreateMaterialApplyItemDto
{
    public string GoodsId { get; set; } = string.Empty;
    public decimal ApplyQty { get; set; }
}

/// <summary>
/// 單位領用申請單查詢 DTO
/// </summary>
public class MaterialApplyQueryDto : PagedQuery
{
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public MaterialApplyQueryFilters? Filters { get; set; }
}

/// <summary>
/// 單位領用申請單查詢篩選條件
/// </summary>
public class MaterialApplyQueryFilters
{
    public string? ApplyId { get; set; }
    public string? EmpId { get; set; }
    public string? OrgId { get; set; }
    public string? SiteId { get; set; }
    public DateTime? ApplyDateFrom { get; set; }
    public DateTime? ApplyDateTo { get; set; }
    public DateTime? AprvDateFrom { get; set; }
    public DateTime? AprvDateTo { get; set; }
    public DateTime? CheckDate { get; set; }
    public string? ApplyStatus { get; set; }
    public string? GoodsId { get; set; }
    public string? WhId { get; set; }
    public string? StoreId { get; set; }
}
