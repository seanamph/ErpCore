using Dapper;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 郵件佇列儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmailQueueRepository : BaseRepository, IEmailQueueRepository
{
    public EmailQueueRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EmailQueue> CreateAsync(EmailQueue entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[EmailQueue] 
                ([EmailLogId], [Priority], [RetryCount], [MaxRetryCount], [Status], [NextRetryAt], 
                 [ProcessedAt], [ErrorMessage], [CreatedAt])
                VALUES 
                (@EmailLogId, @Priority, @RetryCount, @MaxRetryCount, @Status, @NextRetryAt, 
                 @ProcessedAt, @ErrorMessage, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.Id = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立郵件佇列項目失敗", ex);
            throw;
        }
    }

    public async Task<EmailQueue?> GetByIdAsync(long id)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[EmailQueue]
                WHERE [Id] = @Id";

            return await QueryFirstOrDefaultAsync<EmailQueue>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件佇列項目失敗: {id}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EmailQueue>> GetPendingEmailsAsync(int batchSize, DateTime? beforeDate = null)
    {
        try
        {
            var sql = @"
                SELECT TOP (@BatchSize) *
                FROM [dbo].[EmailQueue]
                WHERE [Status] = 'Pending'
                    AND ([NextRetryAt] IS NULL OR [NextRetryAt] <= @BeforeDate)
                ORDER BY [Priority] ASC, [CreatedAt] ASC";

            var before = beforeDate ?? DateTime.Now;
            return await QueryAsync<EmailQueue>(sql, new { BatchSize = batchSize, BeforeDate = before });
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢待處理郵件佇列失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(long id, string status, DateTime? processedAt = null, string? errorMessage = null)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[EmailQueue]
                SET [Status] = @Status,
                    [ProcessedAt] = @ProcessedAt,
                    [ErrorMessage] = @ErrorMessage
                WHERE [Id] = @Id";

            var rowsAffected = await ExecuteAsync(sql, new
            {
                Id = id,
                Status = status,
                ProcessedAt = processedAt,
                ErrorMessage = errorMessage
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新郵件佇列狀態失敗: {id}", ex);
            throw;
        }
    }

    public async Task<bool> UpdateRetryAsync(long id, int retryCount, DateTime? nextRetryAt, string? errorMessage = null)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[EmailQueue]
                SET [Status] = 'Pending',
                    [RetryCount] = @RetryCount,
                    [NextRetryAt] = @NextRetryAt,
                    [ErrorMessage] = @ErrorMessage
                WHERE [Id] = @Id";

            var rowsAffected = await ExecuteAsync(sql, new
            {
                Id = id,
                RetryCount = retryCount,
                NextRetryAt = nextRetryAt,
                ErrorMessage = errorMessage
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新郵件佇列重試資訊失敗: {id}", ex);
            throw;
        }
    }

    public async Task<EmailQueueStatus> GetQueueStatusAsync()
    {
        try
        {
            var sql = @"
                SELECT 
                    SUM(CASE WHEN [Status] = 'Pending' THEN 1 ELSE 0 END) AS PendingCount,
                    SUM(CASE WHEN [Status] = 'Processing' THEN 1 ELSE 0 END) AS ProcessingCount,
                    SUM(CASE WHEN [Status] = 'Sent' THEN 1 ELSE 0 END) AS SentCount,
                    SUM(CASE WHEN [Status] = 'Failed' THEN 1 ELSE 0 END) AS FailedCount,
                    COUNT(*) AS TotalCount
                FROM [dbo].[EmailQueue]";

            return await QueryFirstOrDefaultAsync<EmailQueueStatus>(sql) ?? new EmailQueueStatus();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件佇列狀態失敗", ex);
            throw;
        }
    }
}

