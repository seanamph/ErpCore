using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Shared.Common;

/// <summary>
/// 使用者上下文實作
/// 從 HTTP 上下文取得使用者資訊
/// </summary>
public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// 取得當前使用者 ID
    /// </summary>
    public string? GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // 從 Claims 取得使用者 ID
            var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? httpContext.User.FindFirst("UserId")?.Value
                      ?? httpContext.User.FindFirst("sub")?.Value;
            return userId;
        }

        // 如果沒有認證，從 Header 取得使用者 ID（開發階段使用）
        var headerUserId = httpContext?.Request?.Headers["X-User-Id"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerUserId))
        {
            return headerUserId;
        }

        // 預設返回 SYSTEM（向後相容）
        return "SYSTEM";
    }

    /// <summary>
    /// 取得當前使用者名稱
    /// </summary>
    public string? GetUserName()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var userName = httpContext.User.FindFirst(ClaimTypes.Name)?.Value
                        ?? httpContext.User.FindFirst("UserName")?.Value
                        ?? httpContext.User.Identity.Name;
            return userName;
        }

        // 如果沒有認證，從 Header 取得使用者名稱（開發階段使用）
        var headerUserName = httpContext?.Request?.Headers["X-User-Name"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerUserName))
        {
            return headerUserName;
        }

        return "SYSTEM";
    }

    /// <summary>
    /// 取得當前使用者組織 ID
    /// </summary>
    public string? GetOrgId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var orgId = httpContext.User.FindFirst("OrgId")?.Value
                     ?? httpContext.User.FindFirst("OrganizationId")?.Value;
            return orgId;
        }

        // 如果沒有認證，從 Header 取得組織 ID（開發階段使用）
        var headerOrgId = httpContext?.Request?.Headers["X-Org-Id"].FirstOrDefault();
        return headerOrgId;
    }
}

