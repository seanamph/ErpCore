using ErpCore.Application.DTOs.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreFloor;

/// <summary>
/// 商店查詢服務介面 (SYS6210-SYS6270 - 商店查詢作業)
/// </summary>
public interface IShopFloorQueryService
{
    /// <summary>
    /// 查詢商店列表
    /// </summary>
    Task<PagedResult<ShopFloorDto>> QueryShopFloorsAsync(StoreQueryDto query);

    /// <summary>
    /// 匯出商店查詢結果
    /// </summary>
    Task<byte[]> ExportShopFloorsAsync(StoreExportDto dto);
}

