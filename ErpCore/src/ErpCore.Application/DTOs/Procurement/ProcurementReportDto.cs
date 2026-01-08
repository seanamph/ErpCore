namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 採購報表查詢 DTO (SYSP410-SYSP4I0)
/// </summary>
public class ProcurementReportDto
{
    public string PurchaseOrderNo { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public string SupplierId { get; set; } = string.Empty;
    public string? SupplierName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// 採購報表查詢條件 DTO
/// </summary>
public class ProcurementReportQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? PurchaseOrderNo { get; set; }
    public DateTime? PurchaseDateFrom { get; set; }
    public DateTime? PurchaseDateTo { get; set; }
    public string? SupplierId { get; set; }
    public string? Status { get; set; }
    public string? ReportType { get; set; }
}

/// <summary>
/// 匯出採購報表 DTO
/// </summary>
public class ExportProcurementReportDto
{
    public ProcurementReportQueryDto Query { get; set; } = new();
    public string ExportType { get; set; } = "Excel"; // Excel, PDF
    public string? FileName { get; set; }
}

