using ErpCore.Application.DTOs.SystemExtension;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SystemExtension;

/// <summary>
/// 系統擴展服務介面 (SYSX110, SYSX120, SYSX140)
/// </summary>
public interface ISystemExtensionService
{
    /// <summary>
    /// 查詢系統擴展列表
    /// </summary>
    Task<PagedResult<SystemExtensionDto>> GetSystemExtensionsAsync(SystemExtensionQueryDto query);

    /// <summary>
    /// 查詢單筆系統擴展（根據主鍵）
    /// </summary>
    Task<SystemExtensionDto> GetSystemExtensionByTKeyAsync(long tKey);

    /// <summary>
    /// 查詢單筆系統擴展（根據擴展功能代碼）
    /// </summary>
    Task<SystemExtensionDto> GetSystemExtensionByExtensionIdAsync(string extensionId);

    /// <summary>
    /// 新增系統擴展
    /// </summary>
    Task<long> CreateSystemExtensionAsync(CreateSystemExtensionDto dto);

    /// <summary>
    /// 修改系統擴展
    /// </summary>
    Task UpdateSystemExtensionAsync(long tKey, UpdateSystemExtensionDto dto);

    /// <summary>
    /// 刪除系統擴展
    /// </summary>
    Task DeleteSystemExtensionAsync(long tKey);

    /// <summary>
    /// 查詢統計資訊 (SYSX120)
    /// </summary>
    Task<SystemExtensionStatisticsDto> GetStatisticsAsync(SystemExtensionStatisticsQueryDto query);

    /// <summary>
    /// 匯出系統擴展資料到 Excel (SYSX120)
    /// </summary>
    Task<byte[]> ExportToExcelAsync(SystemExtensionQueryDto query);
}

