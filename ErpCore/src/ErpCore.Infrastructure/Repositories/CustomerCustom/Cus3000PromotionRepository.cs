using System.Data;
using Dapper;
using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000 促銷活動 Repository 實作 (SYS3310-SYS3399 - 促銷活動管理)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class Cus3000PromotionRepository : BaseRepository, ICus3000PromotionRepository
{
    public Cus3000PromotionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Cus3000Promotion?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000Promotions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Cus3000Promotion>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000促銷活動失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Cus3000Promotion?> GetByPromotionIdAsync(string promotionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Cus3000Promotions 
                WHERE PromotionId = @PromotionId";

            return await QueryFirstOrDefaultAsync<Cus3000Promotion>(sql, new { PromotionId = promotionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000促銷活動失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<Cus3000Promotion>> QueryAsync(Cus3000PromotionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Cus3000Promotions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PromotionId))
            {
                sql += " AND PromotionId LIKE @PromotionId";
                parameters.Add("PromotionId", $"%{query.PromotionId}%");
            }

            if (!string.IsNullOrEmpty(query.PromotionName))
            {
                sql += " AND PromotionName LIKE @PromotionName";
                parameters.Add("PromotionName", $"%{query.PromotionName}%");
            }

            if (query.StartDateFrom.HasValue)
            {
                sql += " AND StartDate >= @StartDateFrom";
                parameters.Add("StartDateFrom", query.StartDateFrom.Value);
            }

            if (query.StartDateTo.HasValue)
            {
                sql += " AND StartDate <= @StartDateTo";
                parameters.Add("StartDateTo", query.StartDateTo.Value);
            }

            if (query.EndDateFrom.HasValue)
            {
                sql += " AND EndDate >= @EndDateFrom";
                parameters.Add("EndDateFrom", query.EndDateFrom.Value);
            }

            if (query.EndDateTo.HasValue)
            {
                sql += " AND EndDate <= @EndDateTo";
                parameters.Add("EndDateTo", query.EndDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (PromotionId LIKE @Keyword OR PromotionName LIKE @Keyword)";
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

            return await QueryAsync<Cus3000Promotion>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000促銷活動列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(Cus3000PromotionQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM Cus3000Promotions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.PromotionId))
            {
                sql += " AND PromotionId LIKE @PromotionId";
                parameters.Add("PromotionId", $"%{query.PromotionId}%");
            }

            if (!string.IsNullOrEmpty(query.PromotionName))
            {
                sql += " AND PromotionName LIKE @PromotionName";
                parameters.Add("PromotionName", $"%{query.PromotionName}%");
            }

            if (query.StartDateFrom.HasValue)
            {
                sql += " AND StartDate >= @StartDateFrom";
                parameters.Add("StartDateFrom", query.StartDateFrom.Value);
            }

            if (query.StartDateTo.HasValue)
            {
                sql += " AND StartDate <= @StartDateTo";
                parameters.Add("StartDateTo", query.StartDateTo.Value);
            }

            if (query.EndDateFrom.HasValue)
            {
                sql += " AND EndDate >= @EndDateFrom";
                parameters.Add("EndDateFrom", query.EndDateFrom.Value);
            }

            if (query.EndDateTo.HasValue)
            {
                sql += " AND EndDate <= @EndDateTo";
                parameters.Add("EndDateTo", query.EndDateTo.Value);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                sql += " AND (PromotionId LIKE @Keyword OR PromotionName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{query.Keyword}%");
            }

            return await ExecuteScalarAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000促銷活動數量失敗", ex);
            throw;
        }
    }

    public async Task<long> CreateAsync(Cus3000Promotion entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO Cus3000Promotions 
                (PromotionId, PromotionName, StartDate, EndDate, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@PromotionId, @PromotionName, @StartDate, @EndDate, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt);
                SELECT CAST(SCOPE_IDENTITY() as BIGINT);";

            return await ExecuteScalarAsync<long>(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增CUS3000促銷活動失敗: {entity.PromotionId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(Cus3000Promotion entity)
    {
        try
        {
            const string sql = @"
                UPDATE Cus3000Promotions 
                SET PromotionName = @PromotionName, StartDate = @StartDate, EndDate = @EndDate, 
                    Status = @Status, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新CUS3000促銷活動失敗: {entity.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Cus3000Promotions 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CUS3000促銷活動失敗: {tKey}", ex);
            throw;
        }
    }
}

