using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 角色 Repository 實作 (SYS0210)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class RoleRepository : BaseRepository, IRoleRepository
{
    public RoleRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<Role?> GetByIdAsync(string roleId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM Roles 
                WHERE RoleId = @RoleId";

            return await QueryFirstOrDefaultAsync<Role>(sql, new { RoleId = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<Role>> QueryAsync(RoleQuery query)
    {
        try
        {
            var sql = @"
                SELECT * FROM Roles
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.RoleId))
            {
                sql += " AND RoleId LIKE @RoleId";
                parameters.Add("RoleId", $"%{query.RoleId}%");
            }

            if (!string.IsNullOrEmpty(query.RoleName))
            {
                sql += " AND RoleName LIKE @RoleName";
                parameters.Add("RoleName", $"%{query.RoleName}%");
            }

            // 排序
            var sortField = string.IsNullOrEmpty(query.SortField) ? "RoleId" : query.SortField;
            var sortOrder = string.IsNullOrEmpty(query.SortOrder) || query.SortOrder.ToUpper() == "ASC" ? "ASC" : "DESC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 分頁
            var offset = (query.PageIndex - 1) * query.PageSize;
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", offset);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<Role>(sql, parameters);

            // 查詢總數
            var countSql = @"
                SELECT COUNT(*) FROM Roles
                WHERE 1=1";

            var countParameters = new DynamicParameters();
            if (!string.IsNullOrEmpty(query.RoleId))
            {
                countSql += " AND RoleId LIKE @RoleId";
                countParameters.Add("RoleId", $"%{query.RoleId}%");
            }
            if (!string.IsNullOrEmpty(query.RoleName))
            {
                countSql += " AND RoleName LIKE @RoleName";
                countParameters.Add("RoleName", $"%{query.RoleName}%");
            }

            var totalCount = await QuerySingleAsync<int>(countSql, countParameters);

            return new PagedResult<Role>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢角色列表失敗", ex);
            throw;
        }
    }

    public async Task<Role> CreateAsync(Role role)
    {
        try
        {
            const string sql = @"
                INSERT INTO Roles (
                    RoleId, RoleName, RoleNote, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, CreatedPriority, CreatedGroup
                )
                OUTPUT INSERTED.*
                VALUES (
                    @RoleId, @RoleName, @RoleNote, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt, @CreatedPriority, @CreatedGroup
                )";

            var result = await QueryFirstOrDefaultAsync<Role>(sql, role);
            if (result == null)
            {
                throw new InvalidOperationException("新增角色失敗");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增角色失敗: {role.RoleId}", ex);
            throw;
        }
    }

    public async Task<Role> UpdateAsync(Role role)
    {
        try
        {
            const string sql = @"
                UPDATE Roles SET
                    RoleName = @RoleName,
                    RoleNote = @RoleNote,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                OUTPUT INSERTED.*
                WHERE RoleId = @RoleId";

            var result = await QueryFirstOrDefaultAsync<Role>(sql, role);
            if (result == null)
            {
                throw new InvalidOperationException($"角色不存在: {role.RoleId}");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改角色失敗: {role.RoleId}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string roleId)
    {
        try
        {
            const string sql = @"
                DELETE FROM Roles 
                WHERE RoleId = @RoleId";

            await ExecuteAsync(sql, new { RoleId = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除角色失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string roleId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM Roles 
                WHERE RoleId = @RoleId";

            var count = await QuerySingleAsync<int>(sql, new { RoleId = roleId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查角色是否存在失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<bool> HasUsersAsync(string roleId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserRoles 
                WHERE RoleId = @RoleId";

            var count = await QuerySingleAsync<int>(sql, new { RoleId = roleId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查角色是否有使用者失敗: {roleId}", ex);
            throw;
        }
    }
}
