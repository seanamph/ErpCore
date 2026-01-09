using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 郵件傳真 Repository 介面 (SYS2000 - 郵件傳真作業)
/// </summary>
public interface IMailFaxRepository
{
    /// <summary>
    /// 根據作業編號查詢郵件傳真作業
    /// </summary>
    Task<EmailFaxJob?> GetByIdAsync(string jobId);

    /// <summary>
    /// 查詢郵件傳真作業列表（分頁）
    /// </summary>
    Task<PagedResult<EmailFaxJob>> QueryAsync(EmailFaxJobQuery query);

    /// <summary>
    /// 查詢郵件傳真作業總數
    /// </summary>
    Task<int> GetCountAsync(EmailFaxJobQuery query);

    /// <summary>
    /// 新增郵件傳真作業
    /// </summary>
    Task<EmailFaxJob> CreateAsync(EmailFaxJob job);

    /// <summary>
    /// 修改郵件傳真作業
    /// </summary>
    Task<EmailFaxJob> UpdateAsync(EmailFaxJob job);

    /// <summary>
    /// 刪除郵件傳真作業
    /// </summary>
    Task DeleteAsync(string jobId);

    /// <summary>
    /// 更新作業狀態
    /// </summary>
    Task UpdateStatusAsync(string jobId, string status, DateTime? sendDate = null, string? errorMessage = null);

    /// <summary>
    /// 查詢郵件傳真發送記錄
    /// </summary>
    Task<IEnumerable<EmailFaxLog>> GetLogsByJobIdAsync(string jobId);

    /// <summary>
    /// 新增郵件傳真發送記錄
    /// </summary>
    Task<EmailFaxLog> CreateLogAsync(EmailFaxLog log);

    /// <summary>
    /// 查詢郵件傳真範本
    /// </summary>
    Task<EmailFaxTemplate?> GetTemplateByIdAsync(string templateId);

    /// <summary>
    /// 查詢所有郵件傳真範本
    /// </summary>
    Task<IEnumerable<EmailFaxTemplate>> GetAllTemplatesAsync(string? templateType = null);
}

/// <summary>
/// 郵件傳真作業查詢條件
/// </summary>
public class EmailFaxJobQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? JobId { get; set; }
    public string? JobType { get; set; }
    public string? Status { get; set; }
    public DateTime? SendDateFrom { get; set; }
    public DateTime? SendDateTo { get; set; }
}

