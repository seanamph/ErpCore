using ErpCore.Domain.Entities.SalesReport;
using ErpCore.Shared.Common;
using SalesReportEntity = ErpCore.Domain.Entities.SalesReport.SalesReport;

namespace ErpCore.Infrastructure.Repositories.SalesReport;

/// <summary>
/// 銷售報表 Repository 接口 (SYS1000 - 銷售報表模組系列)
/// </summary>
public interface ISalesReportRepository
{
    /// <summary>
    /// 根據報表編號查詢
    /// </summary>
    Task<SalesReportEntity?> GetByIdAsync(string reportId);

    /// <summary>
    /// 查詢銷售報表列表（分頁）
    /// </summary>
    Task<PagedResult<SalesReportEntity>> QueryAsync(SalesReportQuery query);

    /// <summary>
    /// 新增銷售報表
    /// </summary>
    Task<SalesReportEntity> CreateAsync(SalesReportEntity report);

    /// <summary>
    /// 修改銷售報表
    /// </summary>
    Task<SalesReportEntity> UpdateAsync(SalesReportEntity report);

    /// <summary>
    /// 刪除銷售報表
    /// </summary>
    Task DeleteAsync(string reportId);

    /// <summary>
    /// 檢查報表編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string reportId);
}

/// <summary>
/// 銷售報表查詢條件
/// </summary>
public class SalesReportQuery : PagedQuery
{
    public string? ReportCode { get; set; }
    public string? ShopId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
}

