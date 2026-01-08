using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表服務介面 (SYSL135)
/// </summary>
public interface IBusinessReportService
{
    /// <summary>
    /// 查詢業務報表列表
    /// </summary>
    Task<PagedResult<BusinessReportDto>> GetBusinessReportsAsync(BusinessReportQueryDto query);
}

