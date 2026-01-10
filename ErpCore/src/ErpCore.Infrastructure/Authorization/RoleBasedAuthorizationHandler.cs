using ErpCore.Shared.Logging;
using System.Security.Claims;

namespace ErpCore.Infrastructure.Authorization;

/// <summary>
/// 基於角色的授權處理器
/// 提供基於角色的授權檢查功能
/// </summary>
public class RoleBasedAuthorizationHandler
{
    private readonly ILoggerService _logger;
    private readonly PermissionChecker _permissionChecker;

    public RoleBasedAuthorizationHandler(ILoggerService logger, PermissionChecker permissionChecker)
    {
        _logger = logger;
        _permissionChecker = permissionChecker;
    }

    /// <summary>
    /// 檢查使用者是否可以存取資源
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="requiredRole">需要的角色</param>
    /// <returns>是否可以存取</returns>
    public bool CanAccess(ClaimsPrincipal? user, string requiredRole)
    {
        try
        {
            if (user == null)
            {
                _logger.LogWarning("使用者為空，無法檢查授權");
                return false;
            }

            if (string.IsNullOrWhiteSpace(requiredRole))
            {
                _logger.LogWarning("需要的角色為空，無法檢查授權");
                return false;
            }

            var hasRole = _permissionChecker.HasRole(user, requiredRole);
            
            if (hasRole)
            {
                _logger.LogDebug($"使用者有角色權限: {requiredRole}");
            }
            else
            {
                _logger.LogDebug($"使用者無角色權限: {requiredRole}");
            }

            return hasRole;
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查角色授權時發生錯誤: {requiredRole}", ex);
            return false;
        }
    }

    /// <summary>
    /// 檢查使用者是否可以存取資源（需要任一角色）
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="requiredRoles">需要的角色列表</param>
    /// <returns>是否可以存取</returns>
    public bool CanAccessAny(ClaimsPrincipal? user, IEnumerable<string> requiredRoles)
    {
        try
        {
            if (user == null)
            {
                return false;
            }

            if (requiredRoles == null || !requiredRoles.Any())
            {
                return false;
            }

            return _permissionChecker.HasAnyRole(user, requiredRoles);
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查角色授權（任一）時發生錯誤", ex);
            return false;
        }
    }

    /// <summary>
    /// 檢查使用者是否可以存取資源（需要所有角色）
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="requiredRoles">需要的角色列表</param>
    /// <returns>是否可以存取</returns>
    public bool CanAccessAll(ClaimsPrincipal? user, IEnumerable<string> requiredRoles)
    {
        try
        {
            if (user == null)
            {
                return false;
            }

            if (requiredRoles == null || !requiredRoles.Any())
            {
                return false;
            }

            return requiredRoles.All(role => _permissionChecker.HasRole(user, role));
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查角色授權（所有）時發生錯誤", ex);
            return false;
        }
    }

    /// <summary>
    /// 檢查使用者是否可以執行特定操作
    /// </summary>
    /// <param name="user">使用者 ClaimsPrincipal</param>
    /// <param name="resource">資源名稱</param>
    /// <param name="action">操作名稱</param>
    /// <returns>是否可以執行</returns>
    public bool CanPerformAction(ClaimsPrincipal? user, string resource, string action)
    {
        try
        {
            if (user == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(resource) || string.IsNullOrWhiteSpace(action))
            {
                return false;
            }

            // 組合權限名稱：資源.操作（例如：User.Create）
            var permission = $"{resource}.{action}";
            return _permissionChecker.HasPermission(user, permission);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查操作權限時發生錯誤: {resource}.{action}", ex);
            return false;
        }
    }
}

