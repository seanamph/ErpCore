using ErpCore.Domain.Entities.Inventory;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Inventory;

/// <summary>
/// 文本文件處理記錄 Repository 介面
/// </summary>
public interface ITextFileProcessLogRepository
{
    /// <summary>
    /// 根據ID查詢處理記錄
    /// </summary>
    Task<TextFileProcessLog?> GetByIdAsync(Guid logId);

    /// <summary>
    /// 查詢處理記錄列表（分頁）
    /// </summary>
    Task<PagedResult<TextFileProcessLog>> GetPagedAsync(TextFileProcessLogQuery query);

    /// <summary>
    /// 建立處理記錄
    /// </summary>
    Task<TextFileProcessLog> CreateAsync(TextFileProcessLog log);

    /// <summary>
    /// 更新處理記錄
    /// </summary>
    Task<TextFileProcessLog> UpdateAsync(TextFileProcessLog log);

    /// <summary>
    /// 刪除處理記錄
    /// </summary>
    Task DeleteAsync(Guid logId);
}

/// <summary>
/// 文本文件處理記錄查詢條件
/// </summary>
public class TextFileProcessLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; } = "CreatedAt";
    public string? SortOrder { get; set; } = "DESC";
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public string? ShopId { get; set; }
    public string? ProcessStatus { get; set; }
}

