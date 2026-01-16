using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.TaxAccounting;

/// <summary>
/// 傳票轉入服務介面 (SYST002-SYST003)
/// </summary>
public interface IVoucherImportService
{
    Task<VoucherImportLogDto> UploadAhmFileAsync(IFormFile file);
    Task<PagedResult<VoucherImportLogDto>> GetImportLogsAsync(VoucherImportLogQueryDto query);
    Task<VoucherImportLogDetailDto> GetImportLogDetailsAsync(long tKey, VoucherImportDetailQueryDto query);
    Task<ImportResultDto> ImportHtvDailyAsync(ImportHtvDto dto);
    Task<ImportResultDto> ImportHtvMonthlyAsync(ImportHtvDto dto);
    Task<ImportResultDto> ImportHtvSupplierAsync(ImportHtvDto dto);
    Task<ImportProgressDto> GetImportProgressAsync(long tKey);
    Task<ImportResultDto> RetryImportAsync(long tKey);
}

