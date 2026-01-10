using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Net;
using System.Text.Json;

namespace ErpCore.Api.Middleware;

/// <summary>
/// 授權中介軟體
/// 檢查使用者是否有權限存取特定資源
/// </summary>
public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    public AuthorizationMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // TODO: 實作權限檢查邏輯
        // 可以根據使用者的角色和權限設定來檢查是否有權限存取特定API
        
        // 範例：
        // var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // var path = context.Request.Path.Value;
        // var method = context.Request.Method;
        // 
        // if (!HasPermission(userId, path, method))
        // {
        //     context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
        //     var errorResponse = ApiResponse<object>.Fail("沒有權限存取此資源", "FORBIDDEN");
        //     var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        //     await context.Response.WriteAsync(jsonResponse);
        //     return;
        // }

        await _next(context);
    }
}

