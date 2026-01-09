using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 商店樓層服務介面 (SYS6000 - 商店資料維護)
/// </summary>
public interface IShopFloorService
{
    /// <summary>
    /// 查詢商店列表
    /// </summary>
    Task<PagedResult<ShopFloorDto>> GetShopFloorsAsync(ShopFloorQueryDto query);

    /// <summary>
    /// 查詢單筆商店
    /// </summary>
    Task<ShopFloorDto> GetShopFloorByIdAsync(string shopId);

    /// <summary>
    /// 新增商店
    /// </summary>
    Task<string> CreateShopFloorAsync(CreateShopFloorDto dto);

    /// <summary>
    /// 修改商店
    /// </summary>
    Task UpdateShopFloorAsync(string shopId, UpdateShopFloorDto dto);

    /// <summary>
    /// 刪除商店
    /// </summary>
    Task DeleteShopFloorAsync(string shopId);

    /// <summary>
    /// 檢查商店編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string shopId);

    /// <summary>
    /// 更新商店狀態
    /// </summary>
    Task UpdateStatusAsync(string shopId, string status);
}

