using ErpCore.Application.DTOs.CustomerInvoice;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerInvoice;

/// <summary>
/// 郵件傳真服務介面 (SYS2000 - 郵件傳真作業)
/// </summary>
public interface IMailFaxService
{
    /// <summary>
    /// 查詢郵件傳真作業列表
    /// </summary>
    Task<PagedResult<EmailFaxJobDto>> GetEmailFaxJobsAsync(EmailFaxJobQueryDto query);

    /// <summary>
    /// 查詢單筆郵件傳真作業
    /// </summary>
    Task<EmailFaxJobDto> GetEmailFaxJobByIdAsync(string jobId);

    /// <summary>
    /// 發送郵件
    /// </summary>
    Task<string> SendEmailAsync(SendEmailRequestDto request);

    /// <summary>
    /// 發送傳真
    /// </summary>
    Task<string> SendFaxAsync(SendFaxRequestDto request);

    /// <summary>
    /// 查詢發送記錄
    /// </summary>
    Task<IEnumerable<EmailFaxLogDto>> GetLogsAsync(string jobId);
}

