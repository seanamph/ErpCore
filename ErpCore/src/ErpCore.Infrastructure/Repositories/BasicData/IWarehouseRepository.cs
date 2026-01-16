using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 庫別 Repository 介面
/// </summary>
public interface IWarehouseRepository
{
    /// <summary>
    /// 根據庫別代碼查詢庫別
    /// </summary>
    Task<Warehouse?> GetByIdAsync(string warehouseId);

    /// <summary>
    /// 查詢庫別列表（分頁）
    /// </summary>
    Task<PagedResult<Warehouse>> QueryAsync(WarehouseQuery query);

    /// <summary>
    /// 新增庫別
    /// </summary>
    Task<Warehouse> CreateAsync(Warehouse warehouse);

    /// <summary>
    /// 修改庫別
    /// </summary>
    Task<Warehouse> UpdateAsync(Warehouse warehouse);

    /// <summary>
    /// 刪除庫別
    /// </summary>
    Task DeleteAsync(string warehouseId);

    /// <summary>
    /// 檢查庫別是否存在
    /// </summary>
    Task<bool> ExistsAsync(string warehouseId);
}

/// <summary>
/// 庫別查詢條件
/// </summary>
public class WarehouseQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public string? WarehouseType { get; set; }
    public string? Status { get; set; }
}
