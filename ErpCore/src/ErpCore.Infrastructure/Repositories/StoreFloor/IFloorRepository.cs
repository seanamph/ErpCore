using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// 樓層 Repository 介面 (SYS6310-SYS6370 - 樓層資料維護)
/// </summary>
public interface IFloorRepository
{
    /// <summary>
    /// 根據樓層代碼查詢樓層
    /// </summary>
    Task<Floor?> GetByIdAsync(string floorId);

    /// <summary>
    /// 查詢樓層列表（分頁）
    /// </summary>
    Task<PagedResult<Floor>> QueryAsync(FloorQuery query);

    /// <summary>
    /// 查詢樓層總數
    /// </summary>
    Task<int> GetCountAsync(FloorQuery query);

    /// <summary>
    /// 新增樓層
    /// </summary>
    Task<Floor> CreateAsync(Floor floor);

    /// <summary>
    /// 修改樓層
    /// </summary>
    Task<Floor> UpdateAsync(Floor floor);

    /// <summary>
    /// 刪除樓層（軟刪除，將狀態設為停用）
    /// </summary>
    Task DeleteAsync(string floorId);

    /// <summary>
    /// 檢查樓層代碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string floorId);

    /// <summary>
    /// 更新樓層狀態
    /// </summary>
    Task UpdateStatusAsync(string floorId, string status);

    /// <summary>
    /// 查詢樓層的商店數量
    /// </summary>
    Task<int> GetShopCountAsync(string floorId);
}

/// <summary>
/// 樓層查詢條件
/// </summary>
public class FloorQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FloorId { get; set; }
    public string? FloorName { get; set; }
    public string? Status { get; set; }
}

