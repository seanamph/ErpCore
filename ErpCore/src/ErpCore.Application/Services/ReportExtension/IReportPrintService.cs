using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.ReportExtension;

/// <summary>
/// 報表列印服務介面 (SYS7B10-SYS7B40)
/// </summary>
public interface IReportPrintService
{
    /// <summary>
    /// 列印報表
    /// </summary>
    Task<ReportPrintLogDto> PrintReportAsync(ReportPrintRequestDto request);

    /// <summary>
    /// 查詢報表列印記錄
    /// </summary>
    Task<PagedResult<ReportPrintLogDto>> GetPrintLogsAsync(ReportPrintLogQueryDto query);
}

/// <summary>
/// 報表列印記錄查詢 DTO
/// </summary>
public class ReportPrintLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportCode { get; set; }
    public string? PrintStatus { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

