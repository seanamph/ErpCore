using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerInvoice;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerInvoice;

/// <summary>
/// 郵件傳真 Repository 實作 (SYS2000 - 郵件傳真作業)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MailFaxRepository : BaseRepository, IMailFaxRepository
{
    public MailFaxRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<EmailFaxJob?> GetByIdAsync(string jobId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EmailFaxJobs 
                WHERE JobId = @JobId";

            return await QueryFirstOrDefaultAsync<EmailFaxJob>(sql, new { JobId = jobId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件傳真作業失敗: {jobId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<EmailFaxJob>> QueryAsync(EmailFaxJobQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM EmailFaxJobs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.JobId))
            {
                sql += " AND JobId LIKE @JobId";
                parameters.Add("JobId", $"%{query.JobId}%");
            }

            if (!string.IsNullOrEmpty(query.JobType))
            {
                sql += " AND JobType = @JobType";
                parameters.Add("JobType", query.JobType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.SendDateFrom.HasValue)
            {
                sql += " AND SendDate >= @SendDateFrom";
                parameters.Add("SendDateFrom", query.SendDateFrom);
            }

            if (query.SendDateTo.HasValue)
            {
                sql += " AND SendDate <= @SendDateTo";
                parameters.Add("SendDateTo", query.SendDateTo);
            }

            // 排序
            var sortField = query.SortField ?? "CreatedAt";
            var sortOrder = query.SortOrder ?? "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<EmailFaxJob>(sql, parameters);
            var totalCount = await GetCountAsync(query);

            return new PagedResult<EmailFaxJob>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件傳真作業列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(EmailFaxJobQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM EmailFaxJobs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.JobId))
            {
                sql += " AND JobId LIKE @JobId";
                parameters.Add("JobId", $"%{query.JobId}%");
            }

            if (!string.IsNullOrEmpty(query.JobType))
            {
                sql += " AND JobType = @JobType";
                parameters.Add("JobType", query.JobType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.SendDateFrom.HasValue)
            {
                sql += " AND SendDate >= @SendDateFrom";
                parameters.Add("SendDateFrom", query.SendDateFrom);
            }

            if (query.SendDateTo.HasValue)
            {
                sql += " AND SendDate <= @SendDateTo";
                parameters.Add("SendDateTo", query.SendDateTo);
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件傳真作業總數失敗", ex);
            throw;
        }
    }

    public async Task<EmailFaxJob> CreateAsync(EmailFaxJob job)
    {
        try
        {
            job.CreatedAt = DateTime.Now;
            job.UpdatedAt = DateTime.Now;

            const string insertSql = @"
                INSERT INTO EmailFaxJobs 
                (JobId, JobType, Subject, Recipient, Cc, Bcc, Content, AttachmentPath, Status,
                 SendDate, SendUser, ErrorMessage, RetryCount, MaxRetry, ScheduleDate, TemplateId, Memo,
                 CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@JobId, @JobType, @Subject, @Recipient, @Cc, @Bcc, @Content, @AttachmentPath, @Status,
                 @SendDate, @SendUser, @ErrorMessage, @RetryCount, @MaxRetry, @ScheduleDate, @TemplateId, @Memo,
                 @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var tKey = await connection.QuerySingleAsync<long>(insertSql, job);
            job.TKey = tKey;

            _logger.LogInfo($"建立郵件傳真作業成功: {job.JobId}");
            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立郵件傳真作業失敗: {job.JobId}", ex);
            throw;
        }
    }

    public async Task<EmailFaxJob> UpdateAsync(EmailFaxJob job)
    {
        try
        {
            job.UpdatedAt = DateTime.Now;

            const string updateSql = @"
                UPDATE EmailFaxJobs SET
                    JobType = @JobType,
                    Subject = @Subject,
                    Recipient = @Recipient,
                    Cc = @Cc,
                    Bcc = @Bcc,
                    Content = @Content,
                    AttachmentPath = @AttachmentPath,
                    Status = @Status,
                    SendDate = @SendDate,
                    SendUser = @SendUser,
                    ErrorMessage = @ErrorMessage,
                    RetryCount = @RetryCount,
                    MaxRetry = @MaxRetry,
                    ScheduleDate = @ScheduleDate,
                    TemplateId = @TemplateId,
                    Memo = @Memo,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE JobId = @JobId";

            await ExecuteAsync(updateSql, job);
            _logger.LogInfo($"更新郵件傳真作業成功: {job.JobId}");
            return job;
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新郵件傳真作業失敗: {job.JobId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string jobId)
    {
        try
        {
            // 刪除記錄
            const string deleteLogsSql = "DELETE FROM EmailFaxLogs WHERE JobId = @JobId";
            await ExecuteAsync(deleteLogsSql, new { JobId = jobId });

            // 刪除主檔
            const string deleteSql = "DELETE FROM EmailFaxJobs WHERE JobId = @JobId";
            await ExecuteAsync(deleteSql, new { JobId = jobId });

            _logger.LogInfo($"刪除郵件傳真作業成功: {jobId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除郵件傳真作業失敗: {jobId}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string jobId, string status, DateTime? sendDate = null, string? errorMessage = null)
    {
        try
        {
            var sql = @"
                UPDATE EmailFaxJobs 
                SET Status = @Status,
                    SendDate = @SendDate,
                    ErrorMessage = @ErrorMessage,
                    UpdatedAt = GETDATE()";

            if (status == "SENT")
            {
                sql += ", RetryCount = RetryCount + 1";
            }

            sql += " WHERE JobId = @JobId";

            await ExecuteAsync(sql, new { JobId = jobId, Status = status, SendDate = sendDate, ErrorMessage = errorMessage });
            _logger.LogInfo($"更新郵件傳真作業狀態成功: JobId={jobId}, Status={status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新郵件傳真作業狀態失敗: JobId={jobId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EmailFaxLog>> GetLogsByJobIdAsync(string jobId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EmailFaxLogs 
                WHERE JobId = @JobId 
                ORDER BY LogDate DESC";

            return await QueryAsync<EmailFaxLog>(sql, new { JobId = jobId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件傳真發送記錄失敗: {jobId}", ex);
            throw;
        }
    }

    public async Task<EmailFaxLog> CreateLogAsync(EmailFaxLog log)
    {
        try
        {
            log.LogDate = DateTime.Now;
            log.CreatedAt = DateTime.Now;

            const string insertSql = @"
                INSERT INTO EmailFaxLogs 
                (JobId, LogDate, LogType, LogMessage, CreatedBy, CreatedAt)
                VALUES 
                (@JobId, @LogDate, @LogType, @LogMessage, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            using var connection = _connectionFactory.CreateConnection();
            var tKey = await connection.QuerySingleAsync<long>(insertSql, log);
            log.TKey = tKey;

            _logger.LogInfo($"建立郵件傳真發送記錄成功: JobId={log.JobId}");
            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError($"建立郵件傳真發送記錄失敗: JobId={log.JobId}", ex);
            throw;
        }
    }

    public async Task<EmailFaxTemplate?> GetTemplateByIdAsync(string templateId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM EmailFaxTemplates 
                WHERE TemplateId = @TemplateId AND Status = 'A'";

            return await QueryFirstOrDefaultAsync<EmailFaxTemplate>(sql, new { TemplateId = templateId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢郵件傳真範本失敗: {templateId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<EmailFaxTemplate>> GetAllTemplatesAsync(string? templateType = null)
    {
        try
        {
            var sql = @"
                SELECT * FROM EmailFaxTemplates 
                WHERE Status = 'A'";

            if (!string.IsNullOrEmpty(templateType))
            {
                sql += " AND TemplateType = @TemplateType";
                return await QueryAsync<EmailFaxTemplate>(sql, new { TemplateType = templateType });
            }

            return await QueryAsync<EmailFaxTemplate>(sql);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢郵件傳真範本列表失敗", ex);
            throw;
        }
    }
}

