using ErpCore.Domain.Entities.Procurement;

namespace ErpCore.Infrastructure.Repositories.Procurement;

/// <summary>
/// 採購報表列印 Repository 介面
/// </summary>
public interface IPurchaseReportPrintRepository
{
    Task<PurchaseReportPrint?> GetByIdAsync(long tKey);
    Task<IEnumerable<PurchaseReportPrint>> QueryAsync(PurchaseReportPrintQuery query);
    Task<int> GetCountAsync(PurchaseReportPrintQuery query);
    Task<PurchaseReportPrint> CreateAsync(PurchaseReportPrint entity);
    Task<PurchaseReportPrint> UpdateAsync(PurchaseReportPrint entity);
    Task DeleteAsync(long tKey);
    Task<List<PurchaseReportTemplate>> GetTemplatesAsync(string? reportType, string? reportCode);
}

/// <summary>
/// 採購報表列印查詢條件
/// </summary>
public class PurchaseReportPrintQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ReportType { get; set; }
    public string? ReportCode { get; set; }
    public string? PrintUserId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
