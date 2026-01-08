using ErpCore.Domain.Entities.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// POP列印 Repository 介面
/// </summary>
public interface IPopPrintRepository
{
    /// <summary>
    /// 查詢商品列表（用於列印）
    /// </summary>
    Task<PagedResult<PopPrintProduct>> GetProductsAsync(PopPrintProductQuery query);

    /// <summary>
    /// 根據商品編號查詢商品
    /// </summary>
    Task<PopPrintProduct?> GetProductByIdAsync(string goodsId);

    /// <summary>
    /// 取得列印設定
    /// </summary>
    Task<PopPrintSetting?> GetSettingAsync(string? shopId, string? version = null);

    /// <summary>
    /// 建立或更新列印設定
    /// </summary>
    Task<PopPrintSetting> CreateOrUpdateSettingAsync(PopPrintSetting setting);

    /// <summary>
    /// 建立列印記錄
    /// </summary>
    Task<PopPrintLog> CreateLogAsync(PopPrintLog log);

    /// <summary>
    /// 查詢列印記錄列表
    /// </summary>
    Task<PagedResult<PopPrintLog>> GetLogsAsync(PopPrintLogQuery query);
}

/// <summary>
/// POP列印商品查詢條件
/// </summary>
public class PopPrintProductQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? BarCode { get; set; }
    public string? VendorGoodsId { get; set; }
    public string? LogoId { get; set; }
    public string? BClassId { get; set; }
    public string? MClassId { get; set; }
    public string? SClassId { get; set; }
}

/// <summary>
/// POP列印記錄查詢條件
/// </summary>
public class PopPrintLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? GoodsId { get; set; }
    public string? PrintType { get; set; }
    public string? PrintFormat { get; set; }
    public string? Version { get; set; } // AP, UA, STANDARD
    public string? ShopId { get; set; }
    public DateTime? PrintDateFrom { get; set; }
    public DateTime? PrintDateTo { get; set; }
}

/// <summary>
/// POP列印商品（簡化版，用於列印）
/// </summary>
public class PopPrintProduct
{
    public string GoodsId { get; set; } = string.Empty;
    public string GoodsName { get; set; } = string.Empty;
    public string? BarCode { get; set; }
    public string? VendorGoodsId { get; set; }
    public string? LogoId { get; set; }
    public decimal? Price { get; set; }
    public decimal? Mprc { get; set; }
    public string? Unit { get; set; }
    public string? UnitName { get; set; }
}

