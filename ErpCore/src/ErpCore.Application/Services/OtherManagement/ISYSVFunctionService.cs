using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.OtherManagement;

/// <summary>
/// V系統功能服務介面 (SYSV000)
/// </summary>
public interface ISYSVFunctionService
{
    Task<PagedResult<SYSVFunctionDto>> GetSYSVFunctionsAsync(SYSVFunctionQueryDto query);
    Task<SYSVFunctionDto> GetSYSVFunctionByIdAsync(long tKey);
    Task<SYSVFunctionDto> GetSYSVFunctionByFunctionIdAsync(string functionId);
    Task<long> CreateSYSVFunctionAsync(CreateSYSVFunctionDto dto);
    Task UpdateSYSVFunctionAsync(long tKey, UpdateSYSVFunctionDto dto);
    Task DeleteSYSVFunctionAsync(long tKey);
    Task UpdateStatusAsync(long tKey, string status);
}

