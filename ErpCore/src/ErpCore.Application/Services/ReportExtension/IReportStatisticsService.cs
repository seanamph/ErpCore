using ErpCore.Application.DTOs.ReportExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.ReportExtension;

/// <summary>
/// 報表統計服務介面 (SYS7C10, SYS7C30)
/// </summary>
public interface IReportStatisticsService
{
    /// <summary>
    /// 查詢報表統計記錄
    /// </summary>
    Task<PagedResult<ReportStatisticDto>> GetStatisticsAsync(ReportStatisticQueryDto query);

    /// <summary>
    /// 建立報表統計記錄
    /// </summary>
    Task<ReportStatisticDto> CreateStatisticAsync(CreateReportStatisticDto dto);
}

/// <summary>
/// 新增報表統計 DTO
/// </summary>
public class CreateReportStatisticDto
{
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public string StatisticType { get; set; } = string.Empty;
    public DateTime StatisticDate { get; set; }
    public decimal? StatisticValue { get; set; }
    public string? StatisticData { get; set; }
}

