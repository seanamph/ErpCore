using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Loyalty;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度系統初始化記錄 Repository 實作 (WEBLOYALTYINI - 忠誠度系統初始化)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LoyaltySystemInitLogRepository : BaseRepository, ILoyaltySystemInitLogRepository
{
    public LoyaltySystemInitLogRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LoyaltySystemInitLog?> GetByIdAsync(string initId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LoyaltySystemInitLogs 
                WHERE InitId = @InitId";

            return await QueryFirstOrDefaultAsync<LoyaltySystemInitLog>(sql, new { InitId = initId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢忠誠度系統初始化記錄失敗: {initId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LoyaltySystemInitLog>> QueryAsync(LoyaltySystemInitLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LoyaltySystemInitLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.InitId))
            {
                sql += " AND InitId LIKE @InitId";
                parameters.Add("InitId", $"%{query.InitId}%");
            }

            if (!string.IsNullOrEmpty(query.InitStatus))
            {
                sql += " AND InitStatus = @InitStatus";
                parameters.Add("InitStatus", query.InitStatus);
            }

            if (query.InitDateFrom.HasValue)
            {
                sql += " AND InitDate >= @InitDateFrom";
                parameters.Add("InitDateFrom", query.InitDateFrom.Value);
            }

            if (query.InitDateTo.HasValue)
            {
                sql += " AND InitDate <= @InitDateTo";
                parameters.Add("InitDateTo", query.InitDateTo.Value);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY InitDate DESC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LoyaltySystemInitLog>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度系統初始化記錄列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LoyaltySystemInitLogQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LoyaltySystemInitLogs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.InitId))
            {
                sql += " AND InitId LIKE @InitId";
                parameters.Add("InitId", $"%{query.InitId}%");
            }

            if (!string.IsNullOrEmpty(query.InitStatus))
            {
                sql += " AND InitStatus = @InitStatus";
                parameters.Add("InitStatus", query.InitStatus);
            }

            if (query.InitDateFrom.HasValue)
            {
                sql += " AND InitDate >= @InitDateFrom";
                parameters.Add("InitDateFrom", query.InitDateFrom.Value);
            }

            if (query.InitDateTo.HasValue)
            {
                sql += " AND InitDate <= @InitDateTo";
                parameters.Add("InitDateTo", query.InitDateTo.Value);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度系統初始化記錄數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(LoyaltySystemInitLog entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO LoyaltySystemInitLogs 
                (InitId, InitStatus, InitDate, InitMessage, CreatedBy, CreatedAt)
                VALUES 
                (@InitId, @InitStatus, @InitDate, @InitMessage, @CreatedBy, @CreatedAt)";

            await ExecuteAsync(sql, entity);
            return entity.InitId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增忠誠度系統初始化記錄失敗: {entity.InitId}", ex);
            throw;
        }
    }

    public async Task<string> GenerateInitIdAsync()
    {
        try
        {
            const string sql = @"
                SELECT TOP 1 InitId FROM LoyaltySystemInitLogs 
                WHERE InitId LIKE 'INIT' + FORMAT(GETDATE(), 'yyyyMMdd') + '%'
                ORDER BY InitId DESC";

            var lastInitId = await QueryFirstOrDefaultAsync<string>(sql);

            if (string.IsNullOrEmpty(lastInitId))
            {
                return $"INIT{DateTime.Now:yyyyMMdd}001";
            }

            var sequence = int.Parse(lastInitId.Substring(lastInitId.Length - 3)) + 1;
            return $"INIT{DateTime.Now:yyyyMMdd}{sequence:D3}";
        }
        catch (Exception ex)
        {
            _logger.LogError("產生初始化編號失敗", ex);
            throw;
        }
    }
}

