using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 子系統項目 Repository 實作 (SYS0420)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class MenuRepository : BaseRepository, IMenuRepository
{
    public MenuRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Menu?> GetByIdAsync(string menuId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Menus 
                WHERE MenuId = @MenuId";

            return await QueryFirstOrDefaultAsync<Menu>(sql, new { MenuId = menuId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢子系統失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Menu>> QueryAsync(MenuQuery query)
    {
        try
        {
            var sql = @"
                SELECT m.*
                FROM Menus m
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MenuId))
            {
                sql += " AND m.MenuId LIKE @MenuId";
                parameters.Add("MenuId", $"%{query.MenuId}%");
            }

            if (!string.IsNullOrEmpty(query.MenuName))
            {
                sql += " AND m.MenuName LIKE @MenuName";
                parameters.Add("MenuName", $"%{query.MenuName}%");
            }

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND m.SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.ParentMenuId))
            {
                if (query.ParentMenuId == "0")
                {
                    sql += " AND (m.ParentMenuId IS NULL OR m.ParentMenuId = '0')";
                }
                else
                {
                    sql += " AND m.ParentMenuId = @ParentMenuId";
                    parameters.Add("ParentMenuId", query.ParentMenuId);
                }
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "m.SeqNo, m.MenuId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Menu>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Menus m
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.MenuId))
            {
                countSql += " AND m.MenuId LIKE @MenuId";
                countParameters.Add("MenuId", $"%{query.MenuId}%");
            }
            if (!string.IsNullOrEmpty(query.MenuName))
            {
                countSql += " AND m.MenuName LIKE @MenuName";
                countParameters.Add("MenuName", $"%{query.MenuName}%");
            }
            if (!string.IsNullOrEmpty(query.SystemId))
            {
                countSql += " AND m.SystemId = @SystemId";
                countParameters.Add("SystemId", query.SystemId);
            }
            if (!string.IsNullOrEmpty(query.ParentMenuId))
            {
                if (query.ParentMenuId == "0")
                {
                    countSql += " AND (m.ParentMenuId IS NULL OR m.ParentMenuId = '0')";
                }
                else
                {
                    countSql += " AND m.ParentMenuId = @ParentMenuId";
                    countParameters.Add("ParentMenuId", query.ParentMenuId);
                }
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Menu>
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

    public async Task<Menu> CreateAsync(Menu menu)
    {
        try
        {
            const string sql = @"
                INSERT INTO Menus (
                    MenuId, MenuName, SeqNo, SystemId, ParentMenuId, Status,
                    CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @MenuId, @MenuName, @SeqNo, @SystemId, @ParentMenuId, @Status,
                    @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Menu>(sql, menu);
            if (result == null)
            {
                throw new InvalidOperationException("新增子系統失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增子系統失敗: {menu.MenuId}", ex);
            throw;
        }
    }

    public async Task<Menu> UpdateAsync(Menu menu)
    {
        try
        {
            const string sql = @"
                UPDATE Menus SET
                    MenuName = @MenuName,
                    SeqNo = @SeqNo,
                    SystemId = @SystemId,
                    ParentMenuId = @ParentMenuId,
                    Status = @Status,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE MenuId = @MenuId";

            var result = await QueryFirstOrDefaultAsync<Menu>(sql, menu);
            if (result == null)
            {
                throw new InvalidOperationException($"子系統不存在: {menu.MenuId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改子系統失敗: {menu.MenuId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string menuId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Menus 
                WHERE MenuId = @MenuId";

            await ExecuteAsync(sql, new { MenuId = menuId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除子系統失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string menuId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Menus 
                WHERE MenuId = @MenuId";

            var count = await QuerySingleAsync<int>(sql, new { MenuId = menuId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查子系統是否存在失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task<bool> HasChildrenAsync(string menuId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Menus 
                WHERE ParentMenuId = @MenuId";

            var count = await QuerySingleAsync<int>(sql, new { MenuId = menuId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查是否有下層子系統失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task<bool> HasProgramsAsync(string menuId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Programs 
                WHERE MenuId = @MenuId";

            var count = await QuerySingleAsync<int>(sql, new { MenuId = menuId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查是否有作業關聯失敗: {menuId}", ex);
            throw;
        }
    }
}
