using Dapper;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Domain.Entities.DropdownList;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.DropdownList;

/// <summary>
/// 選單 Repository 實作
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
            _logger.LogError($"查詢選單失敗: {menuId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Menu>> QueryAsync(MenuQueryDto query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Menus
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.MenuName))
            {
                sql += " AND MenuName LIKE @MenuName";
                parameters.Add("MenuName", $"%{query.MenuName}%");
            }

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND SystemId = @SystemId";
                parameters.Add("SystemId", query.SystemId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", query.Status);
            }

            // 排序
            var sortField = query.SortField ?? "MenuName";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            using var connection = _connectionFactory.CreateConnection();

            // 查詢總筆數
            var countSql = sql.Replace("SELECT *", "SELECT COUNT(*)").Split("ORDER BY")[0];
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // 查詢資料
            var items = await connection.QueryAsync<Menu>(sql, parameters);

            return new PagedResult<Menu>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢選單列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<MenuOptionDto>> GetOptionsAsync(string? systemId = null, string? status = "1")
    {
        try
        {
            var sql = @"
                SELECT MenuId AS Value, MenuName AS Label 
                FROM Menus
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(systemId))
            {
                sql += " AND SystemId = @SystemId";
                parameters.Add("SystemId", systemId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            sql += " ORDER BY SeqNo, MenuName";

            return await QueryAsync<MenuOptionDto>(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢選單選項失敗", ex);
            throw;
        }
    }
}

