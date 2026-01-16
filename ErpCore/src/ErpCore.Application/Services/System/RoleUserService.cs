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

    /// <summary>
    /// 查詢角色之使用者列表
    /// </summary>
    public async Task<RoleUserListResponseDto> GetRoleUserListAsync(
        RoleUserListRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                throw new ArgumentException("角色代碼為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            // 驗證角色是否存在
            var roleSql = @"
                SELECT RoleId, RoleName
                FROM Roles
                WHERE RoleId = @RoleId";
            
            var role = await connection.QueryFirstOrDefaultAsync<dynamic>(
                roleSql, 
                new { RoleId = request.RoleId });

            if (role == null)
            {
                throw new KeyNotFoundException($"角色 {request.RoleId} 不存在");
            }

            // 查詢使用者列表（使用視圖 V_RoleUsers）
            var sql = @"
                SELECT 
                    ROLE_ID,
                    ROLE_NAME,
                    USER_ID,
                    USER_NAME,
                    USER_TYPE,
                    STATUS,
                    TITLE,
                    ORG_ID,
                    ORG_NAME
                FROM V_RoleUsers
                WHERE ROLE_ID = @RoleId
                ORDER BY USER_TYPE, USER_ID";

            var results = await connection.QueryAsync<dynamic>(
                sql, 
                new { RoleId = request.RoleId });

            // 組織回應資料
            var response = new RoleUserListResponseDto
            {
                RoleId = role.RoleId,
                RoleName = role.RoleName,
                Users = new List<RoleUserDto>()
            };

            foreach (var item in results)
            {
                var user = new RoleUserDto
                {
                    UserId = (string)item.USER_ID,
                    UserName = (string)item.USER_NAME,
                    UserType = (string?)item.USER_TYPE,
                    UserTypeName = GetUserTypeName((string?)item.USER_TYPE),
                    Status = (string)item.STATUS,
                    StatusName = GetStatusName((string)item.STATUS),
                    Title = (string?)item.TITLE,
                    OrgId = (string?)item.ORG_ID,
                    OrgName = (string?)item.ORG_NAME
                };
                response.Users.Add(user);
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢角色之使用者列表失敗: RoleId={request.RoleId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 刪除使用者角色對應
    /// </summary>
    public async Task DeleteRoleUserAsync(
        string roleId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("角色代碼為必填");
            }

            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("使用者代碼為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                DELETE FROM UserRoles
                WHERE RoleId = @RoleId AND UserId = @UserId";

            var affectedRows = await connection.ExecuteAsync(
                sql,
                new { RoleId = roleId, UserId = userId });

            if (affectedRows == 0)
            {
                throw new KeyNotFoundException($"使用者 {userId} 與角色 {roleId} 的對應關係不存在");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除使用者角色對應失敗: RoleId={roleId}, UserId={userId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 批次刪除使用者角色對應
    /// </summary>
    public async Task BatchDeleteRoleUsersAsync(
        string roleId,
        List<string> userIds,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(roleId))
            {
                throw new ArgumentException("角色代碼為必填");
            }

            if (userIds == null || userIds.Count == 0)
            {
                throw new ArgumentException("使用者代碼列表為必填");
            }

            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                DELETE FROM UserRoles
                WHERE RoleId = @RoleId AND UserId = @UserId";

            var parameters = userIds.Select(userId => new { RoleId = roleId, UserId = userId });
            await connection.ExecuteAsync(sql, parameters);
        }
        catch (Exception ex)
        {
            _logger.LogError($"批次刪除使用者角色對應失敗: RoleId={roleId}, UserIds={string.Join(",", userIds)}", ex);
            throw;
        }
    }

    /// <summary>
    /// 匯出角色之使用者報表
    /// </summary>
    public async Task<byte[]> ExportRoleUserReportAsync(
        RoleUserListRequestDto request,
        string exportFormat,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var data = await GetRoleUserListAsync(request, cancellationToken);

            // 扁平化結構為列表
            var exportData = new List<RoleUserExportItem>();
            foreach (var user in data.Users)
            {
                exportData.Add(new RoleUserExportItem
                {
                    RoleId = data.RoleId,
                    RoleName = data.RoleName,
                    UserId = user.UserId,
                    UserName = user.UserName,
                    UserType = user.UserType ?? string.Empty,
                    UserTypeName = user.UserTypeName ?? string.Empty,
                    Status = user.Status,
                    StatusName = user.StatusName ?? string.Empty,
                    Title = user.Title ?? string.Empty,
                    OrgId = user.OrgId ?? string.Empty,
                    OrgName = user.OrgName ?? string.Empty
                });
            }

            // 定義匯出欄位
            var columns = new List<ExportColumn>
            {
                new ExportColumn { PropertyName = "RoleId", DisplayName = "角色代碼", DataType = ExportDataType.String },
                new ExportColumn { PropertyName = "RoleName", DisplayName = "角色名稱", DataType = ExportDataType.String },
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
                return _exportHelper.ExportToExcel(exportData, columns, "角色之使用者列表", title);
            }
            else if (exportFormat == "PDF")
            {
                return _exportHelper.ExportToPdf(exportData, columns, title);
            }
            else
            {
                throw new ArgumentException($"不支援的匯出格式: {exportFormat}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"匯出角色之使用者報表失敗: RoleId={request.RoleId}, Format={exportFormat}", ex);
            throw;
        }
    }

    /// <summary>
    /// 取得使用者型態名稱
    /// </summary>
    private string GetUserTypeName(string? userType)
    {
        return userType switch
        {
            "1" => "公司人員",
            "2" => "專櫃人員",
            "3" => "其他人員",
            _ => ""
        };
    }

    /// <summary>
    /// 取得帳號狀態名稱
    /// </summary>
    private string GetStatusName(string status)
    {
        return status switch
        {
            "A" => "啟用",
            "I" => "停用",
            "L" => "鎖定",
            _ => ""
        };
    }

    /// <summary>
    /// 角色之使用者匯出項目（扁平化結構）
    /// </summary>
    private class RoleUserExportItem
    {
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserType { get; set; } = string.Empty;
        public string UserTypeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string OrgId { get; set; } = string.Empty;
        public string OrgName { get; set; } = string.Empty;
    }
}
