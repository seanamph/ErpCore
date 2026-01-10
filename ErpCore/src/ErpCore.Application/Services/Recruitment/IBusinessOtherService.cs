using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 招商其他功能服務介面 (SYSC999)
/// </summary>
public interface IBusinessOtherService
{
    /// <summary>
    /// 查詢租戶位置列表
    /// </summary>
    Task<PagedResult<TenantLocationDto>> GetTenantLocationsAsync(TenantLocationQueryDto query);

    /// <summary>
    /// 查詢單筆租戶位置
    /// </summary>
    Task<TenantLocationDto> GetTenantLocationAsync(long tKey);

    /// <summary>
    /// 根據租戶查詢位置列表
    /// </summary>
    Task<List<TenantLocationDto>> GetTenantLocationsByTenantAsync(long agmTKey);

    /// <summary>
    /// 新增租戶位置
    /// </summary>
    Task<TenantLocationKeyDto> CreateTenantLocationAsync(CreateTenantLocationDto dto);

    /// <summary>
    /// 修改租戶位置
    /// </summary>
    Task UpdateTenantLocationAsync(long tKey, UpdateTenantLocationDto dto);

    /// <summary>
    /// 刪除租戶位置
    /// </summary>
    Task DeleteTenantLocationAsync(long tKey);

    /// <summary>
    /// 批次刪除租戶位置
    /// </summary>
    Task DeleteTenantLocationsBatchAsync(BatchDeleteTenantLocationDto dto);
}

