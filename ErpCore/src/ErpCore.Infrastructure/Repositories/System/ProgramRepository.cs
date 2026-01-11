using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 系統作業 Repository 實作 (SYS0430)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ProgramRepository : BaseRepository, IProgramRepository
{
    public ProgramRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Program?> GetByIdAsync(string programId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Programs 
                WHERE ProgramId = @ProgramId";

            return await QueryFirstOrDefaultAsync<Program>(sql, new { ProgramId = programId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Program>> QueryAsync(ProgramQuery query)
    {
        try
        {
            var sql = @"
                SELECT p.*
                FROM Programs p
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                sql += " AND p.ProgramId LIKE @ProgramId";
                parameters.Add("ProgramId", $"%{query.ProgramId}%");
            }

            if (!string.IsNullOrEmpty(query.ProgramName))
            {
                sql += " AND p.ProgramName LIKE @ProgramName";
                parameters.Add("ProgramName", $"%{query.ProgramName}%");
            }

            if (!string.IsNullOrEmpty(query.MenuId))
            {
                sql += " AND p.MenuId = @MenuId";
                parameters.Add("MenuId", query.MenuId);
            }

            if (!string.IsNullOrEmpty(query.ProgramType))
            {
                sql += " AND p.ProgramType = @ProgramType";
                parameters.Add("ProgramType", query.ProgramType);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "p.SeqNo, p.ProgramId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Program>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Programs p
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                countSql += " AND p.ProgramId LIKE @ProgramId";
                countParameters.Add("ProgramId", $"%{query.ProgramId}%");
            }
            if (!string.IsNullOrEmpty(query.ProgramName))
            {
                countSql += " AND p.ProgramName LIKE @ProgramName";
                countParameters.Add("ProgramName", $"%{query.ProgramName}%");
            }
            if (!string.IsNullOrEmpty(query.MenuId))
            {
                countSql += " AND p.MenuId = @MenuId";
                countParameters.Add("MenuId", query.MenuId);
            }
            if (!string.IsNullOrEmpty(query.ProgramType))
            {
                countSql += " AND p.ProgramType = @ProgramType";
                countParameters.Add("ProgramType", query.ProgramType);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Program>
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

    public async Task<Program> CreateAsync(Program program)
    {
        try
        {
            const string sql = @"
                INSERT INTO Programs (
                    ProgramId, ProgramName, SeqNo, MenuId, ProgramUrl, ProgramType,
                    MaintainUserId, Status, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt,
                    CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ProgramId, @ProgramName, @SeqNo, @MenuId, @ProgramUrl, @ProgramType,
                    @MaintainUserId, @Status, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt,
                    @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Program>(sql, program);
            if (result == null)
            {
                throw new InvalidOperationException("新增作業失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增作業失敗: {program.ProgramId}", ex);
            throw;
        }
    }

    public async Task<Program> UpdateAsync(Program program)
    {
        try
        {
            const string sql = @"
                UPDATE Programs SET
                    ProgramName = @ProgramName,
                    SeqNo = @SeqNo,
                    MenuId = @MenuId,
                    ProgramUrl = @ProgramUrl,
                    ProgramType = @ProgramType,
                    MaintainUserId = @MaintainUserId,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE ProgramId = @ProgramId";

            var result = await QueryFirstOrDefaultAsync<Program>(sql, program);
            if (result == null)
            {
                throw new InvalidOperationException($"作業不存在: {program.ProgramId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改作業失敗: {program.ProgramId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string programId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Programs 
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
                SELECT COUNT(*) FROM Programs 
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
                SELECT COUNT(*) FROM Buttons 
                WHERE ProgramId = @ProgramId";

            var count = await QuerySingleAsync<int>(sql, new { ProgramId = programId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查是否有按鈕關聯失敗: {programId}", ex);
            throw;
        }
    }
}
