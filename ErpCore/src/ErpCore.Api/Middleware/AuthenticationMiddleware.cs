using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Net;
using System.Text.Json;

namespace ErpCore.Api.Middleware;

/// <summary>
/// 身份驗證中介軟體
/// 驗證使用者身份（JWT Token等）
/// </summary>
public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 檢查是否需要身份驗證
        var path = context.Request.Path.Value?.ToLower() ?? "";
        
        // 排除不需要驗證的路徑
        if (path.StartsWith("/api/v1/auth/") || 
            path.StartsWith("/swagger") || 
            path.StartsWith("/health"))
        {
            await _next(context);
            return;
        }

        // 檢查是否有授權標頭
        if (!context.Request.Headers.ContainsKey("Authorization"))
        {
            // 某些公開API可能不需要驗證，這裡可以根據實際需求調整
            // 目前先通過，實際應該檢查JWT Token
            await _next(context);
            return;
        }

        // TODO: 實作JWT Token驗證邏輯
        // var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        // if (!ValidateToken(token))
        // {
        //     context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //     var errorResponse = ApiResponse<object>.Fail("未授權的存取", "UNAUTHORIZED");
        //     var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        //     await context.Response.WriteAsync(jsonResponse);
        //     return;
        // }

        await _next(context);
    }
}

