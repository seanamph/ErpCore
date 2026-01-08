using ErpCore.Domain.Entities.Communication;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 郵件附件儲存庫介面
/// </summary>
public interface IEmailAttachmentRepository
{
    /// <summary>
    /// 建立郵件附件
    /// </summary>
    Task<EmailAttachment> CreateAsync(EmailAttachment entity);

    /// <summary>
    /// 根據ID查詢郵件附件
    /// </summary>
    Task<EmailAttachment?> GetByIdAsync(long id);

    /// <summary>
    /// 根據郵件記錄ID查詢附件列表
    /// </summary>
    Task<IEnumerable<EmailAttachment>> GetByEmailLogIdAsync(long emailLogId);

    /// <summary>
    /// 刪除郵件附件
    /// </summary>
    Task<bool> DeleteAsync(long id);
}

