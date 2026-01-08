using ErpCore.Application.DTOs.Procurement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購報表服務介面 (SYSP410-SYSP4I0)
/// </summary>
public interface IProcurementReportService
{
    /// <summary>
    /// 查詢採購報表
    /// </summary>
    Task<PagedResult<ProcurementReportDto>> QueryReportAsync(ProcurementReportQueryDto query);

    /// <summary>
    /// 匯出採購報表
    /// </summary>
    Task<byte[]> ExportReportAsync(ExportProcurementReportDto dto);
}

