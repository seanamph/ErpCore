using ErpCore.Application.DTOs.SystemExtensionH;
using ErpCore.Shared.Common;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.SystemExtensionH;

/// <summary>
/// 人事批量新增服務介面 (SYSH3D0_FMI - 人事批量新增)
/// </summary>
public interface IPersonnelBatchService
{
    Task<PersonnelImportLogDto> UploadFileAsync(IFormFile file);
    Task<PersonnelImportResultDto> ExecuteImportAsync(string importId);
    Task<PersonnelImportProgressDto> GetProgressAsync(string importId);
    Task<PersonnelImportLogDto> GetImportLogAsync(string importId);
    Task<PagedResult<PersonnelImportLogDto>> GetImportLogsAsync(PersonnelImportLogQueryDto query);
    Task<byte[]> ExportFailedDataAsync(string importId);
}

