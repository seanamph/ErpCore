using ErpCore.Application.DTOs.System;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色之使用者列表服務介面 (SYS0750)
/// </summary>
public interface IRoleUserService
{
    /// <summary>
    /// 查詢角色之使用者列表
    /// </summary>
    Task<RoleUserListResponseDto> GetRoleUserListAsync(RoleUserListRequestDto request);

    /// <summary>
    /// 刪除使用者角色對應
    /// </summary>
    Task DeleteRoleUserAsync(string roleId, string userId);

    /// <summary>
    /// 批次刪除使用者角色對應
    /// </summary>
    Task BatchDeleteRoleUsersAsync(string roleId, List<string> userIds);

    /// <summary>
    /// 匯出角色之使用者報表
    /// </summary>
    Task<byte[]> ExportRoleUserReportAsync(RoleUserListRequestDto request, string exportFormat);
}

