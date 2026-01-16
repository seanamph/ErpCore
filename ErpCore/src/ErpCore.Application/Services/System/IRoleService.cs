using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 角色服務介面 (SYS0210)
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// 查詢角色列表
    /// </summary>
    Task<PagedResult<RoleDto>> GetRolesAsync(RoleQueryDto query);

    /// <summary>
    /// 查詢單筆角色
    /// </summary>
    Task<RoleDto> GetRoleByIdAsync(string roleId);

    /// <summary>
    /// 新增角色
    /// </summary>
    Task<string> CreateRoleAsync(CreateRoleDto dto);

    /// <summary>
    /// 修改角色
    /// </summary>
    Task UpdateRoleAsync(string roleId, UpdateRoleDto dto);

    /// <summary>
    /// 刪除角色
    /// </summary>
    Task DeleteRoleAsync(string roleId);

    /// <summary>
    /// 複製角色 (SYS0210 - 創建新角色)
    /// </summary>
    Task<string> CopyRoleAsync(string roleId, CopyRoleDto dto);

    /// <summary>
    /// 複製角色到目標角色 (SYS0240)
    /// </summary>
    Task<CopyRoleResultDto> CopyRoleToTargetAsync(CopyRoleToTargetDto dto);

    /// <summary>
    /// 驗證角色複製 (SYS0240)
    /// </summary>
    Task<ValidateCopyRoleResultDto> ValidateCopyRoleAsync(ValidateCopyRoleDto dto);
}
