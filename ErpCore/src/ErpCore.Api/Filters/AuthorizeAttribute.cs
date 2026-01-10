using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ErpCore.Shared.Common;

namespace ErpCore.Api.Filters;

/// <summary>
/// 授權過濾器
/// 檢查使用者是否有權限存取特定動作
/// </summary>
public class AuthorizeAttribute : ActionFilterAttribute
{
    private readonly string[]? _requiredRoles;
    private readonly string[]? _requiredPermissions;

    /// <summary>
    /// 建構子
    /// </summary>
    /// <param name="requiredRoles">需要的角色（可選）</param>
    /// <param name="requiredPermissions">需要的權限（可選）</param>
    public AuthorizeAttribute(string[]? requiredRoles = null, string[]? requiredPermissions = null)
    {
        _requiredRoles = requiredRoles;
        _requiredPermissions = requiredPermissions;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // TODO: 實作權限檢查邏輯
        // var user = context.HttpContext.User;
        // 
        // if (user == null || !user.Identity?.IsAuthenticated ?? true)
        // {
        //     context.Result = new UnauthorizedObjectResult(
        //         ApiResponse<object>.Fail("未授權的存取", "UNAUTHORIZED"));
        //     return;
        // }
        // 
        // if (_requiredRoles != null && _requiredRoles.Length > 0)
        // {
        //     var userRoles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        //     if (!_requiredRoles.Any(role => userRoles.Contains(role)))
        //     {
        //         context.Result = new ForbidResult();
        //         return;
        //     }
        // }
        // 
        // if (_requiredPermissions != null && _requiredPermissions.Length > 0)
        // {
        //     var userPermissions = user.Claims.Where(c => c.Type == "Permission").Select(c => c.Value);
        //     if (!_requiredPermissions.Any(permission => userPermissions.Contains(permission)))
        //     {
        //         context.Result = new ForbidResult();
        //         return;
        //     }
        // }

        base.OnActionExecuting(context);
    }
}

