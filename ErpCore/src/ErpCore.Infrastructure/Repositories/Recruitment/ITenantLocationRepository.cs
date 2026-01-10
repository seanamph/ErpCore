using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 租戶位置 Repository 介面 (SYSC999)
/// </summary>
public interface ITenantLocationRepository
{
    /// <summary>
    /// 根據主鍵查詢租戶位置
    /// </summary>
    Task<TenantLocation?> GetByKeyAsync(long tKey);

    /// <summary>
    /// 查詢租戶位置列表（分頁）
    /// </summary>
    Task<PagedResult<TenantLocation>> QueryAsync(TenantLocationQuery query);

    /// <summary>
    /// 根據租戶主檔主鍵查詢該租戶的所有位置
    /// </summary>
    Task<List<TenantLocation>> GetByTenantAsync(long agmTKey);

    /// <summary>
    /// 新增租戶位置
    /// </summary>
    Task<TenantLocation> CreateAsync(TenantLocation tenantLocation);

    /// <summary>
    /// 修改租戶位置
    /// </summary>
    Task<TenantLocation> UpdateAsync(TenantLocation tenantLocation);

    /// <summary>
    /// 刪除租戶位置
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 檢查主鍵是否存在
    /// </summary>
    Task<bool> ExistsAsync(long tKey);
}

/// <summary>
/// 租戶位置查詢條件
/// </summary>
public class TenantLocationQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public long? AgmTKey { get; set; }
    public string? LocationId { get; set; }
    public string? AreaId { get; set; }
    public string? FloorId { get; set; }
    public string? Status { get; set; }
}

