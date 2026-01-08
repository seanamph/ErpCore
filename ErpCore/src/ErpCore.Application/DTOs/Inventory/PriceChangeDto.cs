using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.Inventory;

/// <summary>
/// 變價單 DTO
/// </summary>
public class PriceChangeDto
{
    public string PriceChangeId { get; set; } = string.Empty;
    public string PriceChangeType { get; set; } = string.Empty;
    public string PriceChangeTypeName { get; set; } = string.Empty;
    public string? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public string? LogoId { get; set; }
    public string? LogoName { get; set; }
    public string? ApplyEmpId { get; set; }
    public string? ApplyEmpName { get; set; }
    public string? ApplyOrgId { get; set; }
    public DateTime? ApplyDate { get; set; }
    public DateTime? StartDate { get; set; }
    public string? ApproveEmpId { get; set; }
    public string? ApproveEmpName { get; set; }
    public DateTime? ApproveDate { get; set; }
    public string? ConfirmEmpId { get; set; }
    public string? ConfirmEmpName { get; set; }
    public DateTime? ConfirmDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 變價單詳細 DTO（含明細）
/// </summary>
public class PriceChangeDetailDto : PriceChangeDto
{
    public List<PriceChangeDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// 變價單明細項目 DTO
/// </summary>
public class PriceChangeDetailItemDto
{
    public long Id { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? GoodsName { get; set; }
    public decimal BeforePrice { get; set; }
    public decimal AfterPrice { get; set; }
    public decimal ChangeQty { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 變價單查詢 DTO
/// </summary>
public class PriceChangeQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PriceChangeId { get; set; }
    public string? PriceChangeType { get; set; }
    public string? SupplierId { get; set; }
    public string? LogoId { get; set; }
    public string? Status { get; set; }
    public DateTime? ApplyDateFrom { get; set; }
    public DateTime? ApplyDateTo { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}

/// <summary>
/// 新增變價單 DTO
/// </summary>
public class CreatePriceChangeDto
{
    public string PriceChangeType { get; set; } = string.Empty;
    public string? SupplierId { get; set; }
    public string? LogoId { get; set; }
    public string? ApplyEmpId { get; set; }
    public string? ApplyOrgId { get; set; }
    public DateTime? ApplyDate { get; set; }
    public DateTime? StartDate { get; set; }
    public string? Notes { get; set; }
    public List<CreatePriceChangeDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// 新增變價單明細項目 DTO
/// </summary>
public class CreatePriceChangeDetailItemDto
{
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public decimal BeforePrice { get; set; }
    public decimal AfterPrice { get; set; }
    public decimal ChangeQty { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改變價單 DTO
/// </summary>
public class UpdatePriceChangeDto
{
    public string? SupplierId { get; set; }
    public string? LogoId { get; set; }
    public string? ApplyOrgId { get; set; }
    public DateTime? ApplyDate { get; set; }
    public DateTime? StartDate { get; set; }
    public string? Notes { get; set; }
    public List<CreatePriceChangeDetailItemDto> Details { get; set; } = new();
}

/// <summary>
/// 審核變價單 DTO
/// </summary>
public class ApprovePriceChangeDto
{
    public DateTime ApproveDate { get; set; }
}

/// <summary>
/// 確認變價單 DTO
/// </summary>
public class ConfirmPriceChangeDto
{
    public DateTime ConfirmDate { get; set; }
}

