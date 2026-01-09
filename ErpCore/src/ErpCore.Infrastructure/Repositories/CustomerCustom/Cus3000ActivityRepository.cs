using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000 活動 Repository 實作 (SYS3510-SYS3580 - 活動管理)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Cus3000ActivityRepository : BaseRepository, ICus3000ActivityRepository
{
    public Cus3000ActivityRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Cus3000Activity?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000Activities 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Cus3000Activity>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000活動失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Cus3000Activity?> GetByActivityIdAsync(string activityId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000Activities 
                WHERE ActivityId = @ActivityId";

            return await QueryFirstOrDefaultAsync<Cus3000Activity>(sql, new { ActivityId = activityId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000活動失敗: {activityId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Cus3000Activity>> QueryAsync(Cus3000ActivityQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Cus3000Activities
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ActivityId))
            {
                sql += " AND ActivityId LIKE @ActivityId";
                parameters.Add("ActivityId", $"%{query.ActivityId}%");
            }

            if (!string.IsNullOrEmpty(query.ActivityName))
            {
                sql += " AND ActivityName LIKE @ActivityName";
                parameters.Add("ActivityName", $"%{query.ActivityName}%");
            }

            if (query.ActivityDateFrom.HasValue)
            {
                sql += " AND ActivityDate >= @ActivityDateFrom";
                parameters.Add("ActivityDateFrom", query.ActivityDateFrom.Value);
            }

            if (query.ActivityDateTo.HasValue)
            {
                sql += " AND ActivityDate <= @ActivityDateTo";
                parameters.Add("ActivityDateTo", query.ActivityDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (ActivityId LIKE @Keyword OR ActivityName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY CreatedAt DESC";
            }

            // 分頁
            if (query.PageSize > 0)
            {
                var offset = (query.PageIndex - 1) * query.PageSize;
                sql += $" OFFSET {offset} ROWS FETCH NEXT {query.PageSize} ROWS ONLY";
            }

            return await QueryAsync<Cus3000Activity>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000活動列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Cus3000ActivityQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Cus3000Activities
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ActivityId))
            {
                sql += " AND ActivityId LIKE @ActivityId";
                parameters.Add("ActivityId", $"%{query.ActivityId}%");
            }

            if (!string.IsNullOrEmpty(query.ActivityName))
            {
                sql += " AND ActivityName LIKE @ActivityName";
                parameters.Add("ActivityName", $"%{query.ActivityName}%");
            }

            if (query.ActivityDateFrom.HasValue)
            {
                sql += " AND ActivityDate >= @ActivityDateFrom";
                parameters.Add("ActivityDateFrom", query.ActivityDateFrom.Value);
            }

            if (query.ActivityDateTo.HasValue)
            {
                sql += " AND ActivityDate <= @ActivityDateTo";
                parameters.Add("ActivityDateTo", query.ActivityDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (ActivityId LIKE @Keyword OR ActivityName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000活動數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Cus3000Activity entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Cus3000Activities 
                (ActivityId, ActivityName, ActivityDate, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ActivityId, @ActivityName, @ActivityDate, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            return await ExecuteScalarAsync<long>(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增CUS3000活動失敗: {entity.ActivityId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Cus3000Activity entity)
    {
        try
        {
            const string sql = @"
                UPDATE Cus3000Activities 
                SET ActivityName = @ActivityName, ActivityDate = @ActivityDate, 
                    Status = @Status, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新CUS3000活動失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Cus3000Activities 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CUS3000活動失敗: {tKey}", ex);
            throw;
        }
    }
}

