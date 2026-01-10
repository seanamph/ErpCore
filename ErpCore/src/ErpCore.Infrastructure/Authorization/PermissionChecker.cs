using ErpCore.Shared.Logging;
using System.Security.Claims;

namespace ErpCore.Infrastructure.Authorization;

/// <summary>
/// 權限檢查服務
/// 提供權限檢查功能
/// </summary>
public class PermissionChecker
{
    private readonly ILoggerService _logger;

    public PermissionChecker(ILoggerService logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 檢查使用者是否有特定權限
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="permission">權限名稱</param>
    /// <returns>是否有權限</returns>
    public bool HasPermission(ClaimsPrincipal? user, string permission)
    {
        try
        {
            if (user == null)
            {
                _logger.LogWarning("使用者為空，無法檢查權限");
                return false;
            }

            if (string.IsNullOrWhiteSpace(permission))
            {
                _logger.LogWarning("權限名稱為空，無法檢查權限");
                return false;
            }

            // 檢查使用者是否有該權限的 Claim
            var hasPermission = user.HasClaim("Permission", permission);
            
            if (hasPermission)
            {
                _logger.LogDebug($"使用者有權限: {permission}");
            }
            else
            {
                _logger.LogDebug($"使用者無權限: {permission}");
            }

            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查權限時發生錯誤: {permission}", ex);
            return false;
        }
    }

    /// <summary>
    /// 檢查使用者是否有任一權限
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="permissions">權限名稱列表</param>
    /// <returns>是否有任一權限</returns>
    public bool HasAnyPermission(ClaimsPrincipal? user, IEnumerable<string> permissions)
    {
        try
        {
            if (user == null)
            {
                return false;
            }

            if (permissions == null || !permissions.Any())
            {
                return false;
            }

            return permissions.Any(permission => HasPermission(user, permission));
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查任一權限時發生錯誤", ex);
            return false;
        }
    }

    /// <summary>
    /// 檢查使用者是否有所有權限
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="permissions">權限名稱列表</param>
    /// <returns>是否有所有權限</returns>
    public bool HasAllPermissions(ClaimsPrincipal? user, IEnumerable<string> permissions)
    {
        try
        {
            if (user == null)
            {
                return false;
            }

            if (permissions == null || !permissions.Any())
            {
                return false;
            }

            return permissions.All(permission => HasPermission(user, permission));
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查所有權限時發生錯誤", ex);
            return false;
        }
    }

    /// <summary>
    /// 檢查使用者是否有特定角色
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="role">角色名稱</param>
    /// <returns>是否有該角色</returns>
    public bool HasRole(ClaimsPrincipal? user, string role)
    {
        try
        {
            if (user == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                return false;
            }

            return user.IsInRole(role);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查角色時發生錯誤: {role}", ex);
            return false;
        }
    }

    /// <summary>
    /// 檢查使用者是否有任一角色
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="roles">角色名稱列表</param>
    /// <returns>是否有任一角色</returns>
    public bool HasAnyRole(ClaimsPrincipal? user, IEnumerable<string> roles)
    {
        try
        {
            if (user == null)
            {
                return false;
            }

            if (roles == null || !roles.Any())
            {
                return false;
            }

            return roles.Any(role => HasRole(user, role));
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查任一角色時發生錯誤", ex);
            return false;
        }
    }
}

