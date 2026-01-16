using Dapper;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.SystemConfig;

/// <summary>
/// 系統作業 Repository 實作
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ConfigProgramRepository : BaseRepository, IConfigProgramRepository
{
    public ConfigProgramRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<ConfigProgram?> GetByIdAsync(string programId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM ConfigPrograms 
                WHERE ProgramId = @ProgramId";

            return await QueryFirstOrDefaultAsync<ConfigProgram>(sql, new { ProgramId = programId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ConfigProgram>> QueryAsync(ConfigProgramQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM ConfigPrograms
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                sql += " AND ProgramId LIKE @ProgramId";
                parameters.Add("ProgramId", $"%{query.ProgramId}%");
            }

            if (!string.IsNullOrEmpty(query.ProgramName))
            {
                sql += " AND ProgramName LIKE @ProgramName";
                parameters.Add("ProgramName", $"%{query.ProgramName}%");
            }

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.SubSystemId))
            {
                sql += " AND SubSystemId = @SubSystemId";
                parameters.Add("SubSystemId", query.SubSystemId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "SeqNo, ProgramId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<ConfigProgram>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM ConfigPrograms
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                countSql += " AND ProgramId LIKE @ProgramId";
                countParameters.Add("ProgramId", $"%{query.ProgramId}%");
            }
            if (!string.IsNullOrEmpty(query.ProgramName))
            {
                countSql += " AND ProgramName LIKE @ProgramName";
                countParameters.Add("ProgramName", $"%{query.ProgramName}%");
            }
            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND SystemId = @SystemId";
                countParameters.Add("SystemId", query.SystemId);
            }
            if (!string.IsNullOrEmpty(query.SubSystemId))
            {
                countSql += " AND SubSystemId = @SubSystemId";
                countParameters.Add("SubSystemId", query.SubSystemId);
            }
            if (!string.IsNullOrEmpty(query.Status))
            {
                countSql += " AND Status = @Status";
                countParameters.Add("Status", query.Status);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<ConfigProgram>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢作業列表失敗", ex);
            throw;
        }
    }

    public async Task<ConfigProgram> CreateAsync(ConfigProgram configProgram)
    {
        try
        {
            const string sql = @"
                INSERT INTO ConfigPrograms (
                    ProgramId, ProgramName, SeqNo, SystemId, SubSystemId, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ProgramId, @ProgramName, @SeqNo, @SystemId, @SubSystemId, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<ConfigProgram>(sql, configProgram);
            if (result == null)
            {
                throw new InvalidOperationException("新增作業失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增作業失敗: {configProgram.ProgramId}", ex);
            throw;
        }
    }

    public async Task<ConfigProgram> UpdateAsync(ConfigProgram configProgram)
    {
        try
        {
            const string sql = @"
                UPDATE ConfigPrograms SET
                    ProgramName = @ProgramName,
                    SeqNo = @SeqNo,
                    SystemId = @SystemId,
                    SubSystemId = @SubSystemId,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE ProgramId = @ProgramId";

            var result = await QueryFirstOrDefaultAsync<ConfigProgram>(sql, configProgram);
            if (result == null)
            {
                throw new InvalidOperationException($"作業不存在: {configProgram.ProgramId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改作業失敗: {configProgram.ProgramId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string programId)
    {
        try
        {
            const string sql = @"
                DELETE FROM ConfigPrograms 
                WHERE ProgramId = @ProgramId";

            await ExecuteAsync(sql, new { ProgramId = programId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string programId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ConfigPrograms 
                WHERE ProgramId = @ProgramId";

            var count = await QuerySingleAsync<int>(sql, new { ProgramId = programId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查作業是否存在失敗: {programId}", ex);
            throw;
        }
    }

    public async Task<bool> HasButtonsAsync(string programId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM ConfigButtons 
                WHERE ProgramId = @ProgramId";

            var count = await QuerySingleAsync<int>(sql, new { ProgramId = programId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查作業是否有按鈕失敗: {programId}", ex);
            throw;
        }
    }
}
