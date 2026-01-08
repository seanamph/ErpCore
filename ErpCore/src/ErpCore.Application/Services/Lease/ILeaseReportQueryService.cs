using ErpCore.Application.DTOs.Lease;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃報表查詢記錄服務介面 (SYSM141-SYSM144)
/// </summary>
public interface ILeaseReportQueryService
{
    Task<PagedResult<LeaseReportQueryDto>> GetLeaseReportQueriesAsync(LeaseReportQueryQueryDto query);
    Task<LeaseReportQueryDto> GetLeaseReportQueryByIdAsync(string queryId);
    Task<LeaseReportQueryDto> CreateLeaseReportQueryAsync(CreateLeaseReportQueryDto dto);
    Task DeleteLeaseReportQueryAsync(string queryId);
    Task<bool> ExistsAsync(string queryId);
}

