using Dapper;
using ErpCore.Domain.Entities.Pos;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Pos;

/// <summary>
/// POS同步記錄 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class PosSyncLogRepository : BaseRepository, IPosSyncLogRepository
{
    public PosSyncLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<PosSyncLog?> GetByIdAsync(long id)
    {
        try
        {
            const string sql = @"
                SELECT * FROM PosSyncLogs 
                WHERE Id = @Id";

            return await QueryFirstOrDefaultAsync<PosSyncLog>(sql, new { Id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢POS同步記錄失敗: {id}", ex);
            throw;
        }
    }

    public async Task<PagedResult<PosSyncLog>> QueryAsync(PosSyncLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM PosSyncLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SyncType))
            {
                sql += " AND SyncType = @SyncType";
                parameters.Add("SyncType", query.SyncType);
            }

            if (!string.IsNullOrEmpty(query.SyncDirection))
            {
                sql += " AND SyncDirection = @SyncDirection";
                parameters.Add("SyncDirection", query.SyncDirection);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.StartDate.HasValue)
            {
                sql += " AND CreatedAt >= @StartDate";
                parameters.Add("StartDate", query.StartDate.Value);
            }

            if (query.EndDate.HasValue)
            {
                sql += " AND CreatedAt <= @EndDate";
                parameters.Add("EndDate", query.EndDate.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "CreatedAt" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<PosSyncLog>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM PosSyncLogs
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.SyncType))
            {
                countSql += " AND SyncType = @SyncType";
                countParameters.Add("SyncType", query.SyncType);
            }
            if (!string.IsNullOrEmpty(query.SyncDirection))
            {
                countSql += " AND SyncDirection = @SyncDirection";
                countParameters.Add("SyncDirection", query.SyncDirection);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.StartDate.HasValue)
            {
                countSql += " AND CreatedAt >= @StartDate";
                countParameters.Add("StartDate", query.StartDate.Value);
            }
            if (query.EndDate.HasValue)
            {
                countSql += " AND CreatedAt <= @EndDate";
                countParameters.Add("EndDate", query.EndDate.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<PosSyncLog>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢POS同步記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<PosSyncLog> CreateAsync(PosSyncLog log)
    {
        try
        {
            const string sql = @"
                INSERT INTO PosSyncLogs 
                    (SyncType, SyncDirection, RecordCount, SuccessCount, FailedCount, 
                     Status, StartTime, EndTime, ErrorMessage, CreatedBy, CreatedAt)
                VALUES 
                    (@SyncType, @SyncDirection, @RecordCount, @SuccessCount, @FailedCount, 
                     @Status, @StartTime, @EndTime, @ErrorMessage, @CreatedBy, @CreatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            var id = await ExecuteScalarAsync<long>(sql, new
            {
                log.SyncType,
                log.SyncDirection,
                log.RecordCount,
                log.SuccessCount,
                log.FailedCount,
                log.Status,
                log.StartTime,
                log.EndTime,
                log.ErrorMessage,
                log.CreatedBy,
                log.CreatedAt
            });

            log.Id = id;
            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增POS同步記錄失敗", ex);
            throw;
        }
    }

    public async Task<PosSyncLog> UpdateAsync(PosSyncLog log)
    {
        try
        {
            const string sql = @"
                UPDATE PosSyncLogs 
                SET RecordCount = @RecordCount, SuccessCount = @SuccessCount, 
                    FailedCount = @FailedCount, Status = @Status, 
                    EndTime = @EndTime, ErrorMessage = @ErrorMessage
                WHERE Id = @Id";

            await ExecuteAsync(sql, new
            {
                log.Id,
                log.RecordCount,
                log.SuccessCount,
                log.FailedCount,
                log.Status,
                log.EndTime,
                log.ErrorMessage
            });

            return log;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改POS同步記錄失敗: {log.Id}", ex);
            throw;
        }
    }
}

