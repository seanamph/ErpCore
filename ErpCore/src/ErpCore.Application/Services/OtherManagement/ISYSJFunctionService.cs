using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.OtherManagement;

/// <summary>
/// J系統功能服務介面 (SYSJ000)
/// </summary>
public interface ISYSJFunctionService
{
    Task<PagedResult<SYSJFunctionDto>> GetSYSJFunctionsAsync(SYSJFunctionQueryDto query);
    Task<SYSJFunctionDto> GetSYSJFunctionByIdAsync(long tKey);
    Task<SYSJFunctionDto> GetSYSJFunctionByFunctionIdAsync(string functionId);
    Task<long> CreateSYSJFunctionAsync(CreateSYSJFunctionDto dto);
    Task UpdateSYSJFunctionAsync(long tKey, UpdateSYSJFunctionDto dto);
    Task DeleteSYSJFunctionAsync(long tKey);
    Task UpdateStatusAsync(long tKey, string status);
}

