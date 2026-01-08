using Dapper;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 郵件記錄儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class EmailLogRepository : BaseRepository, IEmailLogRepository
{
    public EmailLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EmailLog> CreateAsync(EmailLog entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[EmailLogs] 
                ([FromAddress], [FromName], [ToAddress], [CcAddress], [BccAddress], [Subject], [Body], 
                 [BodyType], [Priority], [Status], [ErrorMessage], [SentAt], [CreatedBy], [CreatedAt], 
                 [SmtpServer], [SmtpPort], [HasAttachment], [AttachmentCount])
                VALUES 
                (@FromAddress, @FromName, @ToAddress, @CcAddress, @BccAddress, @Subject, @Body, 
                 @BodyType, @Priority, @Status, @ErrorMessage, @SentAt, @CreatedBy, @CreatedAt, 
                 @SmtpServer, @SmtpPort, @HasAttachment, @AttachmentCount);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.Id = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立郵件記錄失敗", ex);
            throw;
        }
    }

    public async Task<EmailLog?> GetByIdAsync(long id)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[EmailLogs]
                WHERE [Id] = @Id";

            return await QueryFirstOrDefaultAsync<EmailLog>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件記錄失敗: {id}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EmailLog>> QueryAsync(EmailLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[EmailLogs]
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.FromAddress))
            {
                sql += " AND [FromAddress] LIKE @FromAddress";
                parameters.Add("FromAddress", $"%{query.FromAddress}%");
            }

            if (!string.IsNullOrEmpty(query.ToAddress))
            {
                sql += " AND [ToAddress] LIKE @ToAddress";
                parameters.Add("ToAddress", $"%{query.ToAddress}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND [Status] = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND [CreatedAt] >= @StartDate";
                parameters.Add("StartDate", query.StartDate);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND [CreatedAt] <= @EndDate";
                parameters.Add("EndDate", query.EndDate);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY [{sortField}] {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EmailLog>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[EmailLogs]
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.FromAddress))
            {
                countSql += " AND [FromAddress] LIKE @FromAddress";
                countParameters.Add("FromAddress", $"%{query.FromAddress}%");
            }
            if (!string.IsNullOrEmpty(query.ToAddress))
            {
                countSql += " AND [ToAddress] LIKE @ToAddress";
                countParameters.Add("ToAddress", $"%{query.ToAddress}%");
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND [Status] = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND [CreatedAt] >= @StartDate";
                countParameters.Add("StartDate", query.StartDate);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND [CreatedAt] <= @EndDate";
                countParameters.Add("EndDate", query.EndDate);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<EmailLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(long id, string status, DateTime? sentAt = null, string? errorMessage = null)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[EmailLogs]
                SET [Status] = @Status,
                    [SentAt] = @SentAt,
                    [ErrorMessage] = @ErrorMessage
                WHERE [Id] = @Id";

            var rowsAffected = await ExecuteAsync(sql, new
            {
                Id = id,
                Status = status,
                SentAt = sentAt,
                ErrorMessage = errorMessage
            });
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新郵件記錄狀態失敗: {id}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EmailAttachment>> GetAttachmentsAsync(long emailLogId)
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
            _logger.LogError($"查詢郵件附件失敗: {emailLogId}", ex);
            throw;
        }
    }
}

