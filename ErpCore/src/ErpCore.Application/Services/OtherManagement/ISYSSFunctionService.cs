using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.OtherManagement;

/// <summary>
/// S系統功能服務介面 (SYSS000)
/// </summary>
public interface ISYSSFunctionService
{
    Task<PagedResult<SYSSFunctionDto>> GetSYSSFunctionsAsync(SYSSFunctionQueryDto query);
    Task<SYSSFunctionDto> GetSYSSFunctionByIdAsync(long tKey);
    Task<SYSSFunctionDto> GetSYSSFunctionByFunctionIdAsync(string functionId);
    Task<long> CreateSYSSFunctionAsync(CreateSYSSFunctionDto dto);
    Task UpdateSYSSFunctionAsync(long tKey, UpdateSYSSFunctionDto dto);
    Task DeleteSYSSFunctionAsync(long tKey);
    Task UpdateStatusAsync(long tKey, string status);
}

