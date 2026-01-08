using ErpCore.Domain.Entities.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 文本文件處理明細 Repository 介面
/// </summary>
public interface ITextFileProcessDetailRepository
{
    /// <summary>
    /// 根據處理記錄ID查詢明細列表（分頁）
    /// </summary>
    Task<PagedResult<TextFileProcessDetail>> GetPagedByLogIdAsync(Guid logId, TextFileProcessDetailQuery query);

    /// <summary>
    /// 批次建立明細
    /// </summary>
    Task CreateBatchAsync(IEnumerable<TextFileProcessDetail> details);

    /// <summary>
    /// 批次更新明細
    /// </summary>
    Task UpdateBatchAsync(IEnumerable<TextFileProcessDetail> details);

    /// <summary>
    /// 根據處理記錄ID刪除所有明細
    /// </summary>
    Task DeleteByLogIdAsync(Guid logId);
}

/// <summary>
/// 文本文件處理明細查詢條件
/// </summary>
public class TextFileProcessDetailQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProcessStatus { get; set; }
}

