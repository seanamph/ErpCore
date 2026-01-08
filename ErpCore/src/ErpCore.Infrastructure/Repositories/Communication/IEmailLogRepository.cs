using ErpCore.Domain.Entities.Communication;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 郵件記錄儲存庫介面
/// </summary>
public interface IEmailLogRepository
{
    /// <summary>
    /// 建立郵件記錄
    /// </summary>
    Task<EmailLog> CreateAsync(EmailLog entity);

    /// <summary>
    /// 根據ID查詢郵件記錄
    /// </summary>
    Task<EmailLog?> GetByIdAsync(long id);

    /// <summary>
    /// 查詢郵件記錄列表（分頁）
    /// </summary>
    Task<PagedResult<EmailLog>> QueryAsync(EmailLogQuery query);

    /// <summary>
    /// 更新郵件記錄狀態
    /// </summary>
    Task<bool> UpdateStatusAsync(long id, string status, DateTime? sentAt = null, string? errorMessage = null);

    /// <summary>
    /// 取得郵件附件列表
    /// </summary>
    Task<IEnumerable<EmailAttachment>> GetAttachmentsAsync(long emailLogId);
}

/// <summary>
/// 郵件記錄查詢條件
/// </summary>
public class EmailLogQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FromAddress { get; set; }
    public string? ToAddress { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

