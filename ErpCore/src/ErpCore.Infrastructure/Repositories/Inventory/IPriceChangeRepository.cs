using ErpCore.Domain.Entities.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 變價單 Repository 介面
/// </summary>
public interface IPriceChangeRepository
{
    /// <summary>
    /// 根據變價單號和類型查詢變價單
    /// </summary>
    Task<PriceChangeMaster?> GetByIdAsync(string priceChangeId, string priceChangeType);

    /// <summary>
    /// 查詢變價單列表（分頁）
    /// </summary>
    Task<PagedResult<PriceChangeMaster>> QueryAsync(PriceChangeQuery query);

    /// <summary>
    /// 查詢變價單明細列表
    /// </summary>
    Task<List<PriceChangeDetail>> GetDetailsAsync(string priceChangeId, string priceChangeType);

    /// <summary>
    /// 檢查變價單是否存在
    /// </summary>
    Task<bool> ExistsAsync(string priceChangeId, string priceChangeType);

    /// <summary>
    /// 新增變價單
    /// </summary>
    Task<PriceChangeMaster> CreateAsync(PriceChangeMaster priceChange, List<PriceChangeDetail> details);

    /// <summary>
    /// 修改變價單
    /// </summary>
    Task<PriceChangeMaster> UpdateAsync(PriceChangeMaster priceChange, List<PriceChangeDetail> details);

    /// <summary>
    /// 刪除變價單
    /// </summary>
    Task DeleteAsync(string priceChangeId, string priceChangeType);

    /// <summary>
    /// 更新變價單狀態
    /// </summary>
    Task UpdateStatusAsync(string priceChangeId, string priceChangeType, string status, string? empId, DateTime? date);

    /// <summary>
    /// 更新商品進價
    /// </summary>
    Task UpdateProductPurchasePriceAsync(string goodsId, decimal price, string updatedBy);

    /// <summary>
    /// 更新商品售價
    /// </summary>
    Task UpdateProductSalePriceAsync(string goodsId, decimal price, string updatedBy);
}

/// <summary>
/// 變價單查詢條件
/// </summary>
public class PriceChangeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PriceChangeId { get; set; }
    public string? PriceChangeType { get; set; }
    public string? SupplierId { get; set; }
    public string? LogoId { get; set; }
    public string? Status { get; set; }
    public DateTime? ApplyDateFrom { get; set; }
    public DateTime? ApplyDateTo { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}

