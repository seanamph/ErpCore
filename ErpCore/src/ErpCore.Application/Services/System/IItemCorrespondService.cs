using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 項目對應服務介面 (SYS0360)
/// </summary>
public interface IItemCorrespondService
{
    /// <summary>
    /// 查詢項目對應列表
    /// </summary>
    Task<PagedResult<ItemCorrespondDto>> GetItemCorrespondsAsync(ItemCorrespondQueryDto query);

    /// <summary>
    /// 查詢項目對應（單筆）
    /// </summary>
    Task<ItemCorrespondDto?> GetItemCorrespondByIdAsync(string itemId);

    /// <summary>
    /// 新增項目對應
    /// </summary>
    Task<ItemCorrespondDto> CreateItemCorrespondAsync(CreateItemCorrespondDto dto);

    /// <summary>
    /// 修改項目對應
    /// </summary>
    Task<ItemCorrespondDto> UpdateItemCorrespondAsync(string itemId, UpdateItemCorrespondDto dto);

    /// <summary>
    /// 刪除項目對應
    /// </summary>
    Task DeleteItemCorrespondAsync(string itemId);
}

