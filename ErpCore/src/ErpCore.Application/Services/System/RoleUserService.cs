using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Infrastructure.Data;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色之使用者列表服務實作 (SYS0750)
/// </summary>
public class RoleUserService : BaseService, IRoleUserService
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ExportHelper _exportHelper;

    public RoleUserService(
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext,
        ExportHelper exportHelper) : base(logger, userContext)
    {
        _connectionFactory = connectionFactory;
        _exportHelper = exportHelper;
    }

    public async Task<RoleUserListResponseDto> GetRoleUserListAsync(RoleUserListRequestDto request)
    {
        try
        {
            if (string.IsNullOrEmpty(request.RoleId))
            {
                throw new ArgumentException("角色代碼為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 查詢角色資訊
            var roleSql = "SELECT RoleId, RoleName FROM Roles WHERE RoleId = @RoleId";
            var role = await connection.QueryFirstOrDefaultAsync<dynamic>(roleSql, new { RoleId = request.RoleId });
            if (role == null)
            {
                throw new KeyNotFoundException($"角色 {request.RoleId} 不存在");
            }

            var response = new RoleUserListResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName
            };

            // 查詢角色之使用者列表
            var sql = @"
                SELECT 
                    U.UserId,
                    U.UserName,
                    U.UserType,
                    U.Status,
                    U.Title,
                    U.OrgId
                FROM UserRoles UR
                INNER JOIN Users U ON UR.UserId = U.UserId
                WHERE UR.RoleId = @RoleId
                ORDER BY U.UserType, U.UserId";

            var results = await connection.QueryAsync<dynamic>(sql, new { RoleId = request.RoleId });

            // 組織回應資料
            foreach (var item in results)
            {
                var userId = (string)item.UserId;
                var userName = (string)item.UserName;
                var userType = (string?)item.UserType;
                var status = (string)item.Status;
                var title = (string?)item.Title;
                var orgId = (string?)item.OrgId;

                // 查詢組織名稱（如果有組織代碼）
                string? orgName = null;
                if (!string.IsNullOrEmpty(orgId))
                {
                    var orgSql = "SELECT OrgName FROM Organizations WHERE OrgId = @OrgId";
                    var org = await connection.QueryFirstOrDefaultAsync<dynamic>(orgSql, new { OrgId = orgId });
                    if (org != null)
                    {
                        orgName = org.OrgName;
                    }
                }

                var user = new RoleUserItemDto
                {
                    UserId = userId,
                    UserName = userName,
                    UserType = userType,
                    UserTypeName = GetUserTypeName(userType),
                    Status = status,
                    StatusName = GetStatusName(status),
                    Title = title,
                    OrgId = orgId,
                    OrgName = orgName
                };
                response.Users.Add(user);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢角色之使用者列表失敗", ex);
            throw;
        }
    }

    public async Task DeleteRoleUserAsync(string roleId, string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("角色代碼為必填");
            }

            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("使用者代碼為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 檢查使用者角色對應是否存在
            var checkSql = "SELECT COUNT(*) FROM UserRoles WHERE RoleId = @RoleId AND UserId = @UserId";
            var count = await connection.QuerySingleAsync<int>(checkSql, new { RoleId = roleId, UserId = userId });
            if (count == 0)
            {
                throw new KeyNotFoundException($"使用者 {userId} 與角色 {roleId} 的對應關係不存在");
            }

            // 刪除使用者角色對應
            var deleteSql = "DELETE FROM UserRoles WHERE RoleId = @RoleId AND UserId = @UserId";
            await connection.ExecuteAsync(deleteSql, new { RoleId = roleId, UserId = userId });

            _logger.LogInfo($"刪除使用者角色對應成功: RoleId={roleId}, UserId={userId}");
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除使用者角色對應失敗", ex);
            throw;
        }
    }

    public async Task BatchDeleteRoleUsersAsync(string roleId, List<string> userIds)
    {
        try
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentException("角色代碼為必填");
            }

            if (userIds == null || userIds.Count == 0)
            {
                throw new ArgumentException("使用者代碼列表為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 批次刪除使用者角色對應
            var deleteSql = "DELETE FROM UserRoles WHERE RoleId = @RoleId AND UserId = @UserId";
            var parameters = userIds.Select(userId => new { RoleId = roleId, UserId = userId });
            var affectedRows = await connection.ExecuteAsync(deleteSql, parameters);

            _logger.LogInfo($"批次刪除使用者角色對應成功: RoleId={roleId}, 刪除筆數={affectedRows}");
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除使用者角色對應失敗", ex);
            throw;
        }
    }

    public async Task<byte[]> ExportRoleUserReportAsync(RoleUserListRequestDto request, string exportFormat)
    {
        try
        {
            var data = await GetRoleUserListAsync(request);

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "UserId", DisplayName = "使用者代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserName", DisplayName = "使用者名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserType", DisplayName = "使用者型態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "UserTypeName", DisplayName = "使用者型態名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Status", DisplayName = "帳號狀態", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "StatusName", DisplayName = "帳號狀態名稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "Title", DisplayName = "職稱", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgId", DisplayName = "組織代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "OrgName", DisplayName = "組織名稱", DataType = ExportDataType.String }
            };

            var title = $"角色之使用者列表 - {data.RoleName} ({data.RoleId})";

            if (exportFormat == "Excel")
            {
                return _exportHelper.ExportToExcel(data.Users, columns, "角色之使用者列表", title);
            }
            else if (exportFormat == "PDF")
            {
                return _exportHelper.ExportToPdf(data.Users, columns, title);
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {exportFormat}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("匯出角色之使用者報表失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得使用者型態名稱
    /// </summary>
    private string? GetUserTypeName(string? userType)
    {
        return userType switch
        {
            "1" => "公司人員",
            "2" => "專櫃人員",
            "3" => "其他人員",
            _ => null
        };
    }

    /// <summary>
    /// 取得帳號狀態名稱
    /// </summary>
    private string? GetStatusName(string status)
    {
        return status switch
        {
            "A" => "啟用",
            "I" => "停用",
            "L" => "鎖定",
            _ => null
        };
    }
}

