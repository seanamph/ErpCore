using Dapper;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 使用者列表服務實作 (USER_LIST)
/// </summary>
public class UserListService : BaseService, IUserListService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UserListService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// 查詢使用者列表 (USER_LIST_USER_LIST)
    /// </summary>
    public async Task<PagedResult<UserListDto>> GetUserListAsync(UserListQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢使用者列表 (USER_LIST_USER_LIST)");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status,
                       d.DeptName AS OrgName
                FROM Users u
                LEFT JOIN Departments d ON u.OrgId = d.DeptId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND u.UserId LIKE @UserId";
                parameters.Add("UserId", $"%{query.UserId}%");
            }

            if (!string.IsNullOrEmpty(query.UserName))
            {
                sql += " AND u.UserName LIKE @UserName";
                parameters.Add("UserName", $"%{query.UserName}%");
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND u.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND u.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var sortField = query.SortField ?? "UserId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY u.{sortField} {sortOrder}";

            // 查詢總筆數
            var countSql = sql.Replace("SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status, d.DeptName AS OrgName", "SELECT COUNT(*)");
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await connection.QueryAsync<UserListDto>(sql, parameters);

            return new PagedResult<UserListDto>
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
            _logger.LogError("查詢使用者列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢部門使用者列表 (USER_LIST_DEPT_LIST)
    /// 根據部門查詢使用者（包含子部門）
    /// </summary>
    public async Task<PagedResult<UserListDto>> GetDeptUserListAsync(DeptUserListQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢部門使用者列表 (USER_LIST_DEPT_LIST)");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status,
                       d.DeptName AS OrgName
                FROM Users u
                LEFT JOIN Departments d ON u.OrgId = d.DeptId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            // 部門篩選（包含子部門）
            if (!string.IsNullOrEmpty(query.OrgId))
            {
                // 查詢該部門及其所有子部門
                sql += @" AND u.OrgId IN (
                    SELECT DeptId FROM Departments 
                    WHERE DeptId = @OrgId 
                    OR OrgId = @OrgId
                    OR DeptId IN (
                        SELECT DeptId FROM Departments 
                        WHERE OrgId IN (
                            SELECT DeptId FROM Departments WHERE OrgId = @OrgId
                        )
                    )
                )";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND u.UserId LIKE @UserId";
                parameters.Add("UserId", $"%{query.UserId}%");
            }

            if (!string.IsNullOrEmpty(query.UserName))
            {
                sql += " AND u.UserName LIKE @UserName";
                parameters.Add("UserName", $"%{query.UserName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND u.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var sortField = query.SortField ?? "UserId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY u.{sortField} {sortOrder}";

            // 查詢總筆數
            var countSql = sql.Replace("SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status, d.DeptName AS OrgName", "SELECT COUNT(*)");
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await connection.QueryAsync<UserListDto>(sql, parameters);

            return new PagedResult<UserListDto>
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
            _logger.LogError("查詢部門使用者列表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢其他使用者列表 (USER_LIST_OTHER_LIST)
    /// 查詢其他類型使用者（如外部使用者、廠商使用者等）
    /// </summary>
    public async Task<PagedResult<UserListDto>> GetOtherUserListAsync(UserListQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢其他使用者列表 (USER_LIST_OTHER_LIST)");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status,
                       d.DeptName AS OrgName
                FROM Users u
                LEFT JOIN Departments d ON u.OrgId = d.DeptId
                WHERE 1=1";

            var parameters = new DynamicParameters();

            // 其他類型使用者：UserType 不為空或為特定值
            sql += " AND (u.UserType IS NOT NULL AND u.UserType != '')";

            if (!string.IsNullOrEmpty(query.UserId))
            {
                sql += " AND u.UserId LIKE @UserId";
                parameters.Add("UserId", $"%{query.UserId}%");
            }

            if (!string.IsNullOrEmpty(query.UserName))
            {
                sql += " AND u.UserName LIKE @UserName";
                parameters.Add("UserName", $"%{query.UserName}%");
            }

            if (!string.IsNullOrEmpty(query.OrgId))
            {
                sql += " AND u.OrgId = @OrgId";
                parameters.Add("OrgId", query.OrgId);
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND u.Status = @Status";
                parameters.Add("Status", query.Status);
            }

            var sortField = query.SortField ?? "UserId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY u.{sortField} {sortOrder}";

            // 查詢總筆數
            var countSql = sql.Replace("SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status, d.DeptName AS OrgName", "SELECT COUNT(*)");
            var totalCount = await connection.QuerySingleAsync<int>(countSql, parameters);

            // 分頁
            sql += " OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            parameters.Add("Offset", (query.PageIndex - 1) * query.PageSize);
            parameters.Add("PageSize", query.PageSize);

            var items = await connection.QueryAsync<UserListDto>(sql, parameters);

            return new PagedResult<UserListDto>
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
            _logger.LogError("查詢其他使用者列表失敗", ex);
            throw;
        }
    }
}
