using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 商店服務介面 (SYS3000 - 商店資料維護)
/// </summary>
public interface IStoreService
{
    /// <summary>
    /// 查詢商店列表
    /// </summary>
    Task<PagedResult<ShopDto>> GetShopsAsync(ShopQueryDto query);

    /// <summary>
    /// 查詢單筆商店
    /// </summary>
    Task<ShopDto> GetShopByIdAsync(string shopId);

    /// <summary>
    /// 新增商店
    /// </summary>
    Task<string> CreateShopAsync(CreateShopDto dto);

    /// <summary>
    /// 修改商店
    /// </summary>
    Task UpdateShopAsync(string shopId, UpdateShopDto dto);

    /// <summary>
    /// 刪除商店
    /// </summary>
    Task DeleteShopAsync(string shopId);

    /// <summary>
    /// 檢查商店編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string shopId);

    /// <summary>
    /// 更新商店狀態
    /// </summary>
    Task UpdateStatusAsync(string shopId, string status);
}

