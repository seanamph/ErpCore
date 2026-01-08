using Dapper;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 子系統項目 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConfigSubSystemRepository : BaseRepository, IConfigSubSystemRepository
{
    public ConfigSubSystemRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ConfigSubSystem?> GetByIdAsync(string subSystemId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ConfigSubSystems 
                WHERE SubSystemId = @SubSystemId";

            return await QueryFirstOrDefaultAsync<ConfigSubSystem>(sql, new { SubSystemId = subSystemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢子系統失敗: {subSystemId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ConfigSubSystem>> QueryAsync(ConfigSubSystemQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ConfigSubSystems
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SubSystemId))
            {
                sql += " AND SubSystemId LIKE @SubSystemId";
                parameters.Add("SubSystemId", $"%{query.SubSystemId}%");
            }

            if (!string.IsNullOrEmpty(query.SubSystemName))
            {
                sql += " AND SubSystemName LIKE @SubSystemName";
                parameters.Add("SubSystemName", $"%{query.SubSystemName}%");
            }

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.ParentSubSystemId))
            {
                sql += " AND ParentSubSystemId = @ParentSubSystemId";
                parameters.Add("ParentSubSystemId", query.ParentSubSystemId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "SeqNo, SubSystemId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ConfigSubSystem>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ConfigSubSystems
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.SubSystemId))
            {
                countSql += " AND SubSystemId LIKE @SubSystemId";
                countParameters.Add("SubSystemId", $"%{query.SubSystemId}%");
            }
            if (!string.IsNullOrEmpty(query.SubSystemName))
            {
                countSql += " AND SubSystemName LIKE @SubSystemName";
                countParameters.Add("SubSystemName", $"%{query.SubSystemName}%");
            }
            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND SystemId = @SystemId";
                countParameters.Add("SystemId", query.SystemId);
            }
            if (!string.IsNullOrEmpty(query.ParentSubSystemId))
            {
                countSql += " AND ParentSubSystemId = @ParentSubSystemId";
                countParameters.Add("ParentSubSystemId", query.ParentSubSystemId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ConfigSubSystem>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢子系統列表失敗", ex);
            throw;
        }
    }

    public async Task<ConfigSubSystem> CreateAsync(ConfigSubSystem configSubSystem)
    {
        try
        {
            const string sql = @"
                INSERT INTO ConfigSubSystems (
                    SubSystemId, SubSystemName, SeqNo, SystemId, ParentSubSystemId, Status, Notes,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @SubSystemId, @SubSystemName, @SeqNo, @SystemId, @ParentSubSystemId, @Status, @Notes,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<ConfigSubSystem>(sql, configSubSystem);
            if (result == null)
            {
                throw new InvalidOperationException("新增子系統失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增子系統失敗: {configSubSystem.SubSystemId}", ex);
            throw;
        }
    }

    public async Task<ConfigSubSystem> UpdateAsync(ConfigSubSystem configSubSystem)
    {
        try
        {
            const string sql = @"
                UPDATE ConfigSubSystems SET
                    SubSystemName = @SubSystemName,
                    SeqNo = @SeqNo,
                    SystemId = @SystemId,
                    ParentSubSystemId = @ParentSubSystemId,
                    Status = @Status,
                    Notes = @Notes,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE SubSystemId = @SubSystemId";

            var result = await QueryFirstOrDefaultAsync<ConfigSubSystem>(sql, configSubSystem);
            if (result == null)
            {
                throw new InvalidOperationException($"子系統不存在: {configSubSystem.SubSystemId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改子系統失敗: {configSubSystem.SubSystemId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string subSystemId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ConfigSubSystems 
                WHERE SubSystemId = @SubSystemId";

            await ExecuteAsync(sql, new { SubSystemId = subSystemId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除子系統失敗: {subSystemId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string subSystemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ConfigSubSystems 
                WHERE SubSystemId = @SubSystemId";

            var count = await QuerySingleAsync<int>(sql, new { SubSystemId = subSystemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查子系統是否存在失敗: {subSystemId}", ex);
            throw;
        }
    }

    public async Task<bool> HasChildrenAsync(string subSystemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ConfigSubSystems 
                WHERE ParentSubSystemId = @SubSystemId";

            var count = await QuerySingleAsync<int>(sql, new { SubSystemId = subSystemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查子系統是否有下層子系統失敗: {subSystemId}", ex);
            throw;
        }
    }

    public async Task<bool> HasProgramsAsync(string subSystemId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ConfigPrograms 
                WHERE SubSystemId = @SubSystemId";

            var count = await QuerySingleAsync<int>(sql, new { SubSystemId = subSystemId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查子系統是否有作業失敗: {subSystemId}", ex);
            throw;
        }
    }
}

