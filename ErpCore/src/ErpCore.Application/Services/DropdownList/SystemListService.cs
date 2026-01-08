using Dapper;
using ErpCore.Application.DTOs.DropdownList;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.DropdownList;

/// <summary>
/// 系統列表服務實作
/// </summary>
public class SystemListService : BaseService, ISystemListService
{
    private readonly IDbConnectionFactory _connectionFactory;

    public SystemListService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<SystemListDto>> GetSystemListAsync(SystemListQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢系統列表");
            using var connection = _connectionFactory.CreateConnection();

            // 只顯示有選單、作業、按鈕的系統
            var sql = @"
                SELECT DISTINCT s.SystemId, s.SystemName, s.Status
                FROM ConfigSystems s
                WHERE EXISTS (
                    SELECT 1 FROM ConfigPrograms p WHERE p.SystemId = s.SystemId
                )
                OR EXISTS (
                    SELECT 1 FROM ConfigButtons b WHERE b.SystemId = s.SystemId
                )";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(query.SystemId))
            {
                sql += " AND s.SystemId LIKE @SystemId";
                parameters.Add("SystemId", $"%{query.SystemId}%");
            }

            if (!string.IsNullOrEmpty(query.SystemName))
            {
                sql += " AND s.SystemName LIKE @SystemName";
                parameters.Add("SystemName", $"%{query.SystemName}%");
            }

            if (!string.IsNullOrEmpty(query.Status))
            {
                sql += " AND (s.Status = @Status OR s.Status IS NULL)";
                parameters.Add("Status", query.Status);
            }

            // 排除特定系統（預設排除 EIP0000, CFG0000, XCOM000，除非是 xcom 使用者）
            var excludeSystems = new List<string> { "EIP0000", "CFG0000" };
            if (!string.IsNullOrEmpty(query.ExcludeSystems))
            {
                excludeSystems.AddRange(query.ExcludeSystems.Split(',', StringSplitOptions.RemoveEmptyEntries));
            }

            // 檢查是否為 xcom 使用者（這裡簡化處理，實際應從認證上下文取得）
            var currentUser = _userContext.GetUserId();
            if (string.IsNullOrEmpty(currentUser) || !currentUser.ToLower().Contains("xcom"))
            {
                excludeSystems.Add("XCOM000");
            }

            if (excludeSystems.Any())
            {
                sql += " AND s.SystemId NOT IN @ExcludeSystems";
                parameters.Add("ExcludeSystems", excludeSystems);
            }

            var sortField = query.SortField ?? "SystemId";
            var sortOrder = query.SortOrder ?? "ASC";
            sql += $" ORDER BY {sortField} {sortOrder}";

            var result = await connection.QueryAsync<SystemListDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統列表失敗", ex);
            throw;
        }
    }

    public async Task<IEnumerable<OptionDto>> GetSystemOptionsAsync(string? status = null, string? excludeSystems = null)
    {
        try
        {
            _logger.LogInfo("查詢系統選項");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT DISTINCT s.SystemId AS Value, s.SystemName AS Label
                FROM ConfigSystems s
                WHERE EXISTS (
                    SELECT 1 FROM ConfigPrograms p WHERE p.SystemId = s.SystemId
                )
                OR EXISTS (
                    SELECT 1 FROM ConfigButtons b WHERE b.SystemId = s.SystemId
                )";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND (s.Status = @Status OR s.Status IS NULL)";
                parameters.Add("Status", status);
            }

            var excludeList = new List<string> { "EIP0000", "CFG0000" };
            if (!string.IsNullOrEmpty(excludeSystems))
            {
                excludeList.AddRange(excludeSystems.Split(',', StringSplitOptions.RemoveEmptyEntries));
            }

            var currentUser = _userContext.GetUserId();
            if (string.IsNullOrEmpty(currentUser) || !currentUser.ToLower().Contains("xcom"))
            {
                excludeList.Add("XCOM000");
            }

            if (excludeList.Any())
            {
                sql += " AND s.SystemId NOT IN @ExcludeSystems";
                parameters.Add("ExcludeSystems", excludeList);
            }

            sql += " ORDER BY s.SystemId";

            var result = await connection.QueryAsync<OptionDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢系統選項失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<UserListDto>> GetUserListAsync(UserListQueryDto query)
    {
        try
        {
            _logger.LogInfo("查詢使用者列表");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status,
                       d.DepartmentName AS OrgName
                FROM Users u
                LEFT JOIN Departments d ON u.OrgId = d.DepartmentId
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
            sql += $" ORDER BY {sortField} {sortOrder}";

            // 查詢總筆數
            var countSql = sql.Replace("SELECT u.UserId, u.UserName, u.OrgId, u.Title, u.Status, d.DepartmentName AS OrgName", "SELECT COUNT(*)");
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

    public async Task<IEnumerable<OptionDto>> GetUserListOptionsAsync(string? orgId = null, string? status = "A", string? keyword = null)
    {
        try
        {
            _logger.LogInfo("查詢使用者列表選項");
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                SELECT UserId AS Value, 
                       UserId + ' - ' + UserName AS Label
                FROM Users
                WHERE 1=1";

            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(orgId))
            {
                sql += " AND OrgId = @OrgId";
                parameters.Add("OrgId", orgId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                sql += " AND Status = @Status";
                parameters.Add("Status", status);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                sql += " AND (UserId LIKE @Keyword OR UserName LIKE @Keyword)";
                parameters.Add("Keyword", $"%{keyword}%");
            }

            sql += " ORDER BY UserId";

            var result = await connection.QueryAsync<OptionDto>(sql, parameters);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢使用者列表選項失敗", ex);
            throw;
        }
    }
}

