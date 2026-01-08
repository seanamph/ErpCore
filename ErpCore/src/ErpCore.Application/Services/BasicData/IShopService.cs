using ErpCore.Application.DTOs.BasicData;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 店舖服務介面
/// </summary>
public interface IShopService
{
    /// <summary>
    /// 查詢店舖列表
    /// </summary>
    Task<List<ShopDto>> GetShopsAsync(ShopQueryDto? query = null);
}

