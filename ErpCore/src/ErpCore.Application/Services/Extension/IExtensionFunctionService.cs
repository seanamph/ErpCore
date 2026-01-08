using ErpCore.Application.DTOs.Extension;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Extension;

/// <summary>
/// 擴展功能服務介面
/// </summary>
public interface IExtensionFunctionService
{
    Task<PagedResult<ExtensionFunctionDto>> GetExtensionFunctionsAsync(ExtensionFunctionQueryDto query);
    Task<ExtensionFunctionDto> GetExtensionFunctionByIdAsync(long tKey);
    Task<ExtensionFunctionDto> GetExtensionFunctionByExtensionIdAsync(string extensionId);
    Task<long> CreateExtensionFunctionAsync(CreateExtensionFunctionDto dto);
    Task UpdateExtensionFunctionAsync(long tKey, UpdateExtensionFunctionDto dto);
    Task DeleteExtensionFunctionAsync(long tKey);
    Task BatchCreateExtensionFunctionsAsync(BatchCreateExtensionFunctionDto dto);
    Task UpdateStatusAsync(long tKey, string status);
}

