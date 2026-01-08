using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.ReportExtension;

/// <summary>
/// 報表模組7服務介面 (SYS7000)
/// </summary>
public interface IReportModule7Service
{
    /// <summary>
    /// 查詢報表列表
    /// </summary>
    Task<PagedResult<ReportQueryDto>> GetReportsAsync(ReportQueryListDto query);

    /// <summary>
    /// 執行報表查詢
    /// </summary>
    Task<ReportQueryResultDto> QueryReportAsync(ReportQueryRequestDto request);

    /// <summary>
    /// 匯出報表資料
    /// </summary>
    Task<byte[]> ExportReportAsync(string reportCode, ReportQueryRequestDto request, string format);
}

