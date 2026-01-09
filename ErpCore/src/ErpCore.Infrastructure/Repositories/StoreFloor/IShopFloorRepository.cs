using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// 商店樓層 Repository 介面 (SYS6000 - 商店資料維護)
/// </summary>
public interface IShopFloorRepository
{
    /// <summary>
    /// 根據商店編號查詢商店
    /// </summary>
    Task<ShopFloor?> GetByIdAsync(string shopId);

    /// <summary>
    /// 查詢商店列表（分頁）
    /// </summary>
    Task<PagedResult<ShopFloor>> QueryAsync(ShopFloorQuery query);

    /// <summary>
    /// 查詢商店總數
    /// </summary>
    Task<int> GetCountAsync(ShopFloorQuery query);

    /// <summary>
    /// 新增商店
    /// </summary>
    Task<ShopFloor> CreateAsync(ShopFloor shopFloor);

    /// <summary>
    /// 修改商店
    /// </summary>
    Task<ShopFloor> UpdateAsync(ShopFloor shopFloor);

    /// <summary>
    /// 刪除商店（軟刪除，將狀態設為停用）
    /// </summary>
    Task DeleteAsync(string shopId);

    /// <summary>
    /// 檢查商店編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string shopId);

    /// <summary>
    /// 更新商店狀態
    /// </summary>
    Task UpdateStatusAsync(string shopId, string status);
}

/// <summary>
/// 商店樓層查詢條件
/// </summary>
public class ShopFloorQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? ShopType { get; set; }
    public string? Status { get; set; }
    public string? City { get; set; }
    public string? FloorId { get; set; }
}

