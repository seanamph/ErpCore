using System.Data;
using System.Text;
using Dapper;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 使用者角色 Repository 實作 (SYS0220)
/// 使用 Dapper 進行資料庫存取
/// </summary>
public class UserRoleRepository : BaseRepository, IUserRoleRepository
{
    public UserRoleRepository(IDbConnectionFactory connectionFactory, ILoggerService logger)
        : base(connectionFactory, logger)
    {
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserRoles 
                WHERE UserId = @UserId
                ORDER BY CreatedAt DESC";

            return await QueryAsync<UserRole>(sql, new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者角色失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserRole?> GetByUserIdAndRoleIdAsync(string userId, string roleId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";

            return await QueryFirstOrDefaultAsync<UserRole>(sql, new { UserId = userId, RoleId = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者角色失敗: {userId}, {roleId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<UserRole>> GetByUserIdAndRoleIdsAsync(string userId, List<string> roleIds)
    {
        try
        {
            if (roleIds == null || roleIds.Count == 0)
            {
                return Enumerable.Empty<UserRole>();
            }

            const string sql = @"
                SELECT * FROM UserRoles 
                WHERE UserId = @UserId AND RoleId IN @RoleIds";

            return await QueryAsync<UserRole>(sql, new { UserId = userId, RoleIds = roleIds });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者角色失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<List<string>> GetRoleIdsByUserIdAsync(string userId)
    {
        try
        {
            const string sql = @"
                SELECT RoleId FROM UserRoles 
                WHERE UserId = @UserId";

            var result = await QueryAsync<string>(sql, new { UserId = userId });
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者角色ID列表失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserRole>> GetUserRolesAsync(string userId, int pageIndex = 1, int pageSize = 20)
    {
        try
        {
            var sql = @"
                SELECT ur.*
                FROM UserRoles ur
                WHERE ur.UserId = @UserId
                ORDER BY ur.CreatedAt DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("Offset", (pageIndex - 1) * pageSize);
            parameters.Add("PageSize", pageSize);

            var items = await QueryAsync<UserRole>(sql, parameters);

            // 查詢總數
            const string countSql = @"
                SELECT COUNT(*) FROM UserRoles 
                WHERE UserId = @UserId";

            var totalCount = await QueryFirstOrDefaultAsync<int>(countSql, new { UserId = userId });

            return new PagedResult<UserRole>
            {
                Items = items.ToList(),
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢使用者角色列表失敗: {userId}", ex);
            throw;
        }
    }

    public async Task<UserRole> CreateAsync(UserRole userRole)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserRoles (UserId, RoleId, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup)
                VALUES (@UserId, @RoleId, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup)";

            await ExecuteAsync(sql, new
            {
                userRole.UserId,
                userRole.RoleId,
                userRole.CreatedBy,
                userRole.CreatedAt,
                userRole.CreatedPriority,
                userRole.CreatedGroup
            });

            return userRole;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增使用者角色失敗: {userRole.UserId}, {userRole.RoleId}", ex);
            throw;
        }
    }

    public async Task CreateRangeAsync(IEnumerable<UserRole> userRoles)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserRoles (UserId, RoleId, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup)
                VALUES (@UserId, @RoleId, @CreatedBy, @CreatedAt, @CreatedPriority, @CreatedGroup)";

            await ExecuteAsync(sql, userRoles.Select(ur => new
            {
                ur.UserId,
                ur.RoleId,
                ur.CreatedBy,
                ur.CreatedAt,
                ur.CreatedPriority,
                ur.CreatedGroup
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError("批量新增使用者角色失敗", ex);
            throw;
        }
    }

    public async Task DeleteAsync(string userId, string roleId)
    {
        try
        {
            const string sql = @"
                DELETE FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";

            await ExecuteAsync(sql, new { UserId = userId, RoleId = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者角色失敗: {userId}, {roleId}", ex);
            throw;
        }
    }

    public async Task DeleteRangeAsync(IEnumerable<UserRole> userRoles)
    {
        try
        {
            const string sql = @"
                DELETE FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";

            await ExecuteAsync(sql, userRoles.Select(ur => new
            {
                ur.UserId,
                ur.RoleId
            }));
        }
        catch (Exception ex)
        {
            _logger.LogError("批量刪除使用者角色失敗", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string userId, string roleId)
    {
        try
        {
            const string sql = @"
                SELECT COUNT(*) FROM UserRoles 
                WHERE UserId = @UserId AND RoleId = @RoleId";

            var count = await QueryFirstOrDefaultAsync<int>(sql, new { UserId = userId, RoleId = roleId });
            return count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查使用者角色是否存在失敗: {userId}, {roleId}", ex);
            throw;
        }
    }

    public async Task<PagedResult<RoleUserListItem>> GetRoleUsersAsync(RoleUserQuery query)
    {
        try
        {
            var sql = new StringBuilder(@"
                SELECT 
                    u.UserId,
                    u.UserName,
                    u.OrgId,
                    o.OrgName,
                    CASE WHEN ur.UserId IS NOT NULL THEN 1 ELSE 0 END AS IsAssigned
                FROM Users u
                LEFT JOIN UserRoles ur ON u.UserId = ur.UserId AND ur.RoleId = @RoleId
                LEFT JOIN Organizations o ON u.OrgId = o.OrgId
                WHERE 1=1");

            var parameters = new DynamicParameters();
            parameters.Add("RoleId", query.RoleId);

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql.Append(" AND u.OrgId = @OrgId");
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.UserType))
            {
                sql.Append(" AND u.UserType = @UserType");
                parameters.Add("UserType", query.UserType);
            }

            if (!string.IsNullOrEmpty(query.Filter))
            {
                sql.Append(" AND (u.UserId LIKE @Filter OR u.UserName LIKE @Filter)");
                parameters.Add("Filter", $"%{query.Filter}%");
            }

            sql.Append(" ORDER BY u.UserId");

            // 查詢總數
            var countSql = $"SELECT COUNT(*) FROM ({sql}) AS Total";
            var total = await QueryFirstOrDefaultAsync<int>(countSql, parameters);

            // 分頁處理
            sql.Append(" OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY");
            parameters.Add("Offset", (query.Page - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await QueryAsync<RoleUserListItem>(sql.ToString(), parameters);

            return new PagedResult<RoleUserListItem>
            {
                Items = items.ToList(),
                TotalCount = total,
                PageIndex = query.Page,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色使用者列表失敗: {query.RoleId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<UserRole>> GetByRoleIdAsync(string roleId)
    {
        try
        {
            const string sql = @"
                SELECT * FROM UserRoles 
                WHERE RoleId = @RoleId
                ORDER BY CreatedAt DESC";

            return await QueryAsync<UserRole>(sql, new { RoleId = roleId });
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色使用者失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task DeleteByRoleIdAsync(string roleId, IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = "DELETE FROM UserRoles WHERE RoleId = @RoleId";
            
            if (transaction != null)
            {
                await transaction.Connection!.ExecuteAsync(sql, new { RoleId = roleId }, transaction);
            }
            else
            {
                await ExecuteAsync(sql, new { RoleId = roleId });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除角色使用者分配失敗: {roleId}", ex);
            throw;
        }
    }

    public async Task<int> CopyFromRoleAsync(string sourceRoleId, string targetRoleId, string createdBy, global::System.Data.IDbTransaction? transaction = null)
    {
        try
        {
            const string sql = @"
                INSERT INTO UserRoles (UserId, RoleId, CreatedBy, CreatedAt, CreatedPriority, CreatedGroup)
                SELECT UserId, @TargetRoleId, @CreatedBy, GETDATE(),
                       (SELECT CreatedPriority FROM Users WHERE UserId = @CreatedBy),
                       (SELECT CreatedGroup FROM Users WHERE UserId = @CreatedBy)
                FROM UserRoles
                WHERE RoleId = @SourceRoleId";

            var parameters = new
            {
                SourceRoleId = sourceRoleId,
                TargetRoleId = targetRoleId,
                CreatedBy = createdBy
            };

            if (transaction != null)
            {
                return await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
            }
            else
            {
                return await ExecuteAsync(sql, parameters);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"複製角色使用者分配失敗: {sourceRoleId} -> {targetRoleId}", ex);
            throw;
        }
    }
}

