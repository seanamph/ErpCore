using Dapper;
using ErpCore.Domain.Entities.SystemExtension;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemExtension;

/// <summary>
/// 系統擴展 Repository 實作 (SYSX110, SYSX120, SYSX140)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class SystemExtensionRepository : BaseRepository, ISystemExtensionRepository
{
    public SystemExtensionRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<SystemExtension?> GetByTKeyAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SystemExtensions 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<SystemExtension>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<SystemExtension?> GetByExtensionIdAsync(string extensionId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM SystemExtensions 
                WHERE ExtensionId = @ExtensionId";

            return await QueryFirstOrDefaultAsync<SystemExtension>(sql, new { ExtensionId = extensionId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢系統擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<SystemExtension>> QueryAsync(SystemExtensionQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM SystemExtensions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtensionId))
            {
                sql += " AND ExtensionId LIKE @ExtensionId";
                parameters.Add("ExtensionId", $"%{query.ExtensionId}%");
            }

            if (!string.IsNullOrEmpty(query.ExtensionName))
            {
                sql += " AND ExtensionName LIKE @ExtensionName";
                parameters.Add("ExtensionName", $"%{query.ExtensionName}%");
            }

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            if (query.CreatedDateFrom.HasValue)
            {
                sql += " AND CreatedAt >= @CreatedDateFrom";
                parameters.Add("CreatedDateFrom", query.CreatedDateFrom.Value);
            }

            if (query.CreatedDateTo.HasValue)
            {
                sql += " AND CreatedAt <= @CreatedDateTo";
                parameters.Add("CreatedDateTo", query.CreatedDateTo.Value);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "SeqNo" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<SystemExtension>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM SystemExtensions
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ExtensionId))
            {
                countSql += " AND ExtensionId LIKE @ExtensionId";
                countParameters.Add("ExtensionId", $"%{query.ExtensionId}%");
            }
            if (!string.IsNullOrEmpty(query.ExtensionName))
            {
                countSql += " AND ExtensionName LIKE @ExtensionName";
                countParameters.Add("ExtensionName", $"%{query.ExtensionName}%");
            }
            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                countSql += " AND ExtensionType = @ExtensionType";
                countParameters.Add("ExtensionType", query.ExtensionType);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }
            if (query.CreatedDateFrom.HasValue)
            {
                countSql += " AND CreatedAt >= @CreatedDateFrom";
                countParameters.Add("CreatedDateFrom", query.CreatedDateFrom.Value);
            }
            if (query.CreatedDateTo.HasValue)
            {
                countSql += " AND CreatedAt <= @CreatedDateTo";
                countParameters.Add("CreatedDateTo", query.CreatedDateTo.Value);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<SystemExtension>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<SystemExtension> CreateAsync(SystemExtension systemExtension)
    {
        try
        {
            const string sql = @"
                INSERT INTO SystemExtensions (
                    ExtensionId, ExtensionName, ExtensionType, ExtensionValue, ExtensionConfig,
                    SeqNo, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ExtensionId, @ExtensionName, @ExtensionType, @ExtensionValue, @ExtensionConfig,
                    @SeqNo, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<SystemExtension>(sql, systemExtension);
            if (result == null)
            {
                throw new InvalidOperationException("新增系統擴展失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增系統擴展失敗: {systemExtension.ExtensionId}", ex);
            throw;
        }
    }

    public async Task<SystemExtension> UpdateAsync(SystemExtension systemExtension)
    {
        try
        {
            const string sql = @"
                UPDATE SystemExtensions SET
                    ExtensionName = @ExtensionName,
                    ExtensionType = @ExtensionType,
                    ExtensionValue = @ExtensionValue,
                    ExtensionConfig = @ExtensionConfig,
                    SeqNo = @SeqNo,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            var result = await QueryFirstOrDefaultAsync<SystemExtension>(sql, systemExtension);
            if (result == null)
            {
                throw new InvalidOperationException($"系統擴展不存在: {systemExtension.TKey}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改系統擴展失敗: {systemExtension.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM SystemExtensions
                WHERE TKey = @TKey";

            var rowsAffected = await ExecuteAsync(sql, new { TKey = tKey });
            if (rowsAffected == 0)
            {
                throw new InvalidOperationException($"系統擴展不存在: {tKey}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除系統擴展失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string extensionId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM SystemExtensions
                WHERE ExtensionId = @ExtensionId";

            var count = await QuerySingleAsync<int>(sql, new { ExtensionId = extensionId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查系統擴展是否存在失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<SystemExtensionStatistics> GetStatisticsAsync(SystemExtensionStatisticsQuery query)
    {
        try
        {
            var sql = @"
                SELECT 
                    COUNT(*) AS TotalCount,
                    SUM(CASE WHEN Status = '1' THEN 1 ELSE 0 END) AS ActiveCount,
                    SUM(CASE WHEN Status = '0' THEN 1 ELSE 0 END) AS InactiveCount
                FROM SystemExtensions
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                sql += " AND ExtensionType = @ExtensionType";
                parameters.Add("ExtensionType", query.ExtensionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var stats = await QueryFirstOrDefaultAsync<SystemExtensionStatistics>(sql, parameters);
            if (stats == null)
            {
                stats = new SystemExtensionStatistics();
            }

            // 查詢依類型統計
            var typeSql = @"
                SELECT ExtensionType, COUNT(*) AS Count
                FROM SystemExtensions
                WHERE 1=1";

            var typeParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ExtensionType))
            {
                typeSql += " AND ExtensionType = @ExtensionType";
                typeParameters.Add("ExtensionType", query.ExtensionType);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                typeSql += " AND Status = @Status";
                typeParameters.Add("Status", query.Status);
            }

            typeSql += " GROUP BY ExtensionType";

            var typeStats = await QueryAsync<SystemExtensionTypeCount>(typeSql, typeParameters);
            stats.ByType = typeStats.ToList();

            return stats;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統擴展統計失敗", ex);
            throw;
        }
    }
}

