using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.OtherManagement;

/// <summary>
/// U系統功能服務介面 (SYSU000)
/// </summary>
public interface ISYSUFunctionService
{
    Task<PagedResult<SYSUFunctionDto>> GetSYSUFunctionsAsync(SYSUFunctionQueryDto query);
    Task<SYSUFunctionDto> GetSYSUFunctionByIdAsync(long tKey);
    Task<SYSUFunctionDto> GetSYSUFunctionByFunctionIdAsync(string functionId);
    Task<long> CreateSYSUFunctionAsync(CreateSYSUFunctionDto dto);
    Task UpdateSYSUFunctionAsync(long tKey, UpdateSYSUFunctionDto dto);
    Task DeleteSYSUFunctionAsync(long tKey);
    Task UpdateStatusAsync(long tKey, string status);
}

