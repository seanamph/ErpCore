using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 系統功能按鈕 Repository 實作 (SYS0440)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class ButtonRepository : BaseRepository, IButtonRepository
{
    public ButtonRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Button?> GetByIdAsync(long tKey)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Buttons 
                WHERE TKey = @TKey";

            return await QueryFirstOrDefaultAsync<Button>(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢按鈕失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Button>> QueryAsync(ButtonQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Buttons
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                sql += " AND ProgramId = @ProgramId";
                parameters.Add("ProgramId", query.ProgramId);
            }

            if (!string.IsNullOrEmpty(query.ButtonId))
            {
                sql += " AND ButtonId LIKE @ButtonId";
                parameters.Add("ButtonId", $"%{query.ButtonId}%");
            }

            if (!string.IsNullOrEmpty(query.ButtonName))
            {
                sql += " AND ButtonName LIKE @ButtonName";
                parameters.Add("ButtonName", $"%{query.ButtonName}%");
            }

            if (!string.IsNullOrEmpty(query.PageId))
            {
                sql += " AND PageId = @PageId";
                parameters.Add("PageId", query.PageId);
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "ProgramId, ButtonId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Button>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Buttons
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.ProgramId))
            {
                countSql += " AND ProgramId = @ProgramId";
                countParameters.Add("ProgramId", query.ProgramId);
            }
            if (!string.IsNullOrEmpty(query.ButtonId))
            {
                countSql += " AND ButtonId LIKE @ButtonId";
                countParameters.Add("ButtonId", $"%{query.ButtonId}%");
            }
            if (!string.IsNullOrEmpty(query.ButtonName))
            {
                countSql += " AND ButtonName LIKE @ButtonName";
                countParameters.Add("ButtonName", $"%{query.ButtonName}%");
            }
            if (!string.IsNullOrEmpty(query.PageId))
            {
                countSql += " AND PageId = @PageId";
                countParameters.Add("PageId", query.PageId);
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Button>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢按鈕列表失敗", ex);
            throw;
        }
    }

    public async Task<Button> CreateAsync(Button button)
    {
        try
        {
            const string sql = @"
                INSERT INTO Buttons (
                    ProgramId, ButtonId, ButtonName, PageId, ButtonMsg, ButtonAttr, ButtonUrl, MsgType, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @ProgramId, @ButtonId, @ButtonName, @PageId, @ButtonMsg, @ButtonAttr, @ButtonUrl, @MsgType, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Button>(sql, button);
            if (result == null)
            {
                throw new InvalidOperationException("新增按鈕失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增按鈕失敗: {button.ButtonId}", ex);
            throw;
        }
    }

    public async Task<Button> UpdateAsync(Button button)
    {
        try
        {
            const string sql = @"
                UPDATE Buttons SET
                    ProgramId = @ProgramId,
                    ButtonId = @ButtonId,
                    ButtonName = @ButtonName,
                    PageId = @PageId,
                    ButtonMsg = @ButtonMsg,
                    ButtonAttr = @ButtonAttr,
                    ButtonUrl = @ButtonUrl,
                    MsgType = @MsgType,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE TKey = @TKey";

            var result = await QueryFirstOrDefaultAsync<Button>(sql, button);
            if (result == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {button.TKey}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改按鈕失敗: {button.TKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            const string sql = @"
                DELETE FROM Buttons 
                WHERE TKey = @TKey";

            await ExecuteAsync(sql, new { TKey = tKey });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除按鈕失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> HasPermissionsAsync(long tKey)
    {
        try
        {
            // 檢查是否有使用者按鈕權限或角色按鈕權限
            const string sql = @"
                SELECT COUNT(*) FROM (
                    SELECT 1 FROM UserButtons WHERE ButtonId IN (SELECT ButtonId FROM Buttons WHERE TKey = @TKey)
                    UNION
                    SELECT 1 FROM RoleButtons WHERE ButtonId IN (SELECT ButtonId FROM Buttons WHERE TKey = @TKey)
                ) AS Permissions";

            var count = await QuerySingleAsync<int>(sql, new { TKey = tKey });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查按鈕權限失敗: {tKey}", ex);
            throw;
        }
    }
}
