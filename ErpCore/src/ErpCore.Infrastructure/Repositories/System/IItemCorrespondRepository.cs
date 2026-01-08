using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 項目對應 Repository 介面 (SYS0360)
/// </summary>
public interface IItemCorrespondRepository
{
    /// <summary>
    /// 查詢項目對應列表
    /// </summary>
    Task<PagedResult<ItemCorrespond>> QueryAsync(ItemCorrespondQuery query);

    /// <summary>
    /// 根據ID查詢項目對應
    /// </summary>
    Task<ItemCorrespond?> GetByIdAsync(string itemId);

    /// <summary>
    /// 新增項目對應
    /// </summary>
    Task<ItemCorrespond> CreateAsync(ItemCorrespond itemCorrespond);

    /// <summary>
    /// 修改項目對應
    /// </summary>
    Task<ItemCorrespond> UpdateAsync(ItemCorrespond itemCorrespond);

    /// <summary>
    /// 刪除項目對應
    /// </summary>
    Task DeleteAsync(string itemId);

    /// <summary>
    /// 檢查項目對應是否存在
    /// </summary>
    Task<bool> ExistsAsync(string itemId);
}

/// <summary>
/// 項目對應查詢條件
/// </summary>
public class ItemCorrespondQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ItemId { get; set; }
    public string? ItemName { get; set; }
    public string? ItemType { get; set; }
    public string? Status { get; set; }
}

