using Dapper;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 郵件附件儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmailAttachmentRepository : BaseRepository, IEmailAttachmentRepository
{
    public EmailAttachmentRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EmailAttachment> CreateAsync(EmailAttachment entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[EmailAttachments] 
                ([EmailLogId], [FileName], [FilePath], [FileSize], [ContentType], [CreatedAt])
                VALUES 
                (@EmailLogId, @FileName, @FilePath, @FileSize, @ContentType, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.Id = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立郵件附件失敗", ex);
            throw;
        }
    }

    public async Task<EmailAttachment?> GetByIdAsync(long id)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[EmailAttachments]
                WHERE [Id] = @Id";

            return await QueryFirstOrDefaultAsync<EmailAttachment>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件附件失敗: {id}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EmailAttachment>> GetByEmailLogIdAsync(long emailLogId)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[EmailAttachments]
                WHERE [EmailLogId] = @EmailLogId
                ORDER BY [CreatedAt]";

            return await QueryAsync<EmailAttachment>(sql, new { EmailLogId = emailLogId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件附件列表失敗: {emailLogId}", ex);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(long id)
    {
        try
        {
            var sql = @"
                DELETE FROM [dbo].[EmailAttachments]
                WHERE [Id] = @Id";

            var rowsAffected = await ExecuteAsync(sql, new { Id = id });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除郵件附件失敗: {id}", ex);
            throw;
        }
    }
}

