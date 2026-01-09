using ErpCore.Application.DTOs.SalesReport;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SalesReport;

/// <summary>
/// 銷售報表服務介面 (SYS1000 - 銷售報表模組系列)
/// </summary>
public interface ISalesReportService
{
    Task<PagedResult<SalesReportDto>> GetSalesReportsAsync(SalesReportQueryDto query);
    Task<SalesReportDto?> GetSalesReportByIdAsync(string reportId);
    Task<string> CreateSalesReportAsync(CreateSalesReportDto dto);
    Task UpdateSalesReportAsync(string reportId, UpdateSalesReportDto dto);
    Task DeleteSalesReportAsync(string reportId);
    Task<GenerateReportResponseDto> GenerateReportAsync(GenerateReportDto dto);
    Task<byte[]> DownloadReportAsync(string reportId, string format);
}

