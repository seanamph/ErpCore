using System.Data;
using Dapper;
using ErpCore.Domain.Entities.Loyalty;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.Loyalty;

/// <summary>
/// 忠誠度系統設定 Repository 實作 (WEBLOYALTYINI - 忠誠度系統初始化)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class LoyaltySystemConfigRepository : BaseRepository, ILoyaltySystemConfigRepository
{
    public LoyaltySystemConfigRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<LoyaltySystemConfig?> GetByIdAsync(string configId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM LoyaltySystemConfigs 
                WHERE ConfigId = @ConfigId";

            return await QueryFirstOrDefaultAsync<LoyaltySystemConfig>(sql, new { ConfigId = configId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢忠誠度系統設定失敗: {configId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LoyaltySystemConfig>> QueryAsync(LoyaltySystemConfigQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM LoyaltySystemConfigs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ConfigId))
            {
                sql += " AND ConfigId LIKE @ConfigId";
                parameters.Add("ConfigId", $"%{query.ConfigId}%");
            }

            if (!string.IsNullOrEmpty(query.ConfigType))
            {
                sql += " AND ConfigType = @ConfigType";
                parameters.Add("ConfigType", query.ConfigType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            if (!string.IsNullOrEmpty(query.SortField))
            {
                var sortOrder = query.SortOrder == "DESC" ? "DESC" : "ASC";
                sql += $" ORDER BY {query.SortField} {sortOrder}";
            }
            else
            {
                sql += " ORDER BY ConfigId ASC";
            }

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            return await QueryAsync<LoyaltySystemConfig>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度系統設定列表失敗", ex);
            throw;
        }
    }

    public async Task<int> GetCountAsync(LoyaltySystemConfigQuery query)
    {
        try
        {
            var sql = @"
                SELECT COUNT(*) FROM LoyaltySystemConfigs
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ConfigId))
            {
                sql += " AND ConfigId LIKE @ConfigId";
                parameters.Add("ConfigId", $"%{query.ConfigId}%");
            }

            if (!string.IsNullOrEmpty(query.ConfigType))
            {
                sql += " AND ConfigType = @ConfigType";
                parameters.Add("ConfigType", query.ConfigType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            return await QuerySingleAsync<int>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度系統設定數量失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateAsync(LoyaltySystemConfig entity)
    {
        try
        {
            const string sql = @"
                INSERT INTO LoyaltySystemConfigs 
                (ConfigId, ConfigName, ConfigValue, ConfigType, Description, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
                VALUES 
                (@ConfigId, @ConfigName, @ConfigValue, @ConfigType, @Description, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt)";

            await ExecuteAsync(sql, entity);
            return entity.ConfigId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增忠誠度系統設定失敗: {entity.ConfigId}", ex);
            throw;
        }
    }

    public async Task UpdateAsync(LoyaltySystemConfig entity)
    {
        try
        {
            const string sql = @"
                UPDATE LoyaltySystemConfigs 
                SET ConfigName = @ConfigName, 
                    ConfigValue = @ConfigValue, 
                    ConfigType = @ConfigType, 
                    Description = @Description, 
                    Status = @Status, 
                    UpdatedBy = @UpdatedBy, 
                    UpdatedAt = @UpdatedAt
                WHERE ConfigId = @ConfigId";

            await ExecuteAsync(sql, entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改忠誠度系統設定失敗: {entity.ConfigId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string configId)
    {
        try
        {
            const string sql = @"
                DELETE FROM LoyaltySystemConfigs 
                WHERE ConfigId = @ConfigId";

            await ExecuteAsync(sql, new { ConfigId = configId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除忠誠度系統設定失敗: {configId}", ex);
            throw;
        }
    }
}

