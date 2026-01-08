using Dapper;
using ErpCore.Domain.Entities.Communication;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Communication;

/// <summary>
/// 簡訊記錄儲存庫實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SmsLogRepository : BaseRepository, ISmsLogRepository
{
    public SmsLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SmsLog> CreateAsync(SmsLog entity)
    {
        try
        {
            var sql = @"
                INSERT INTO [dbo].[SmsLogs] 
                ([PhoneNumber], [Message], [Status], [ErrorMessage], [SentAt], [CreatedBy], [CreatedAt], 
                 [Provider], [ProviderMessageId])
                VALUES 
                (@PhoneNumber, @Message, @Status, @ErrorMessage, @SentAt, @CreatedBy, @CreatedAt, 
                 @Provider, @ProviderMessageId);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, entity);
            entity.Id = id;
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立簡訊記錄失敗", ex);
            throw;
        }
    }

    public async Task<SmsLog?> GetByIdAsync(long id)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[SmsLogs]
                WHERE [Id] = @Id";

            return await QueryFirstOrDefaultAsync<SmsLog>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢簡訊記錄失敗: {id}", ex);
            throw;
        }
    }

    public async Task<PagedResult<SmsLog>> QueryAsync(SmsLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM [dbo].[SmsLogs]
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PhoneNumber))
            {
                sql += " AND [PhoneNumber] LIKE @PhoneNumber";
                parameters.Add("PhoneNumber", $"%{query.PhoneNumber}%");
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

            var items = await QueryAsync<SmsLog>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM [dbo].[SmsLogs]
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.PhoneNumber))
            {
                countSql += " AND [PhoneNumber] LIKE @PhoneNumber";
                countParameters.Add("PhoneNumber", $"%{query.PhoneNumber}%");
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

            return new PagedResult<SmsLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢簡訊記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(long id, string status, DateTime? sentAt = null, string? errorMessage = null)
    {
        try
        {
            var sql = @"
                UPDATE [dbo].[SmsLogs]
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
            _logger.LogError($"更新簡訊記錄狀態失敗: {id}", ex);
            throw;
        }
    }
}

