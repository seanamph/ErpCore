using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using System.Net;
using System.Text.Json;

namespace ErpCore.Api.Middleware;

/// <summary>
/// 例外處理中介軟體
/// 統一處理所有未處理的例外
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var errorResponse = new ApiResponse<object>
        {
            Success = false,
            Message = "系統發生錯誤",
            ErrorCode = "SYSTEM_ERROR"
        };

        switch (exception)
        {
            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.Message = argEx.Message;
                errorResponse.ErrorCode = "INVALID_ARGUMENT";
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.Message = "未授權的存取";
                errorResponse.ErrorCode = "UNAUTHORIZED";
                break;

            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.Message = "資源不存在";
                errorResponse.ErrorCode = "NOT_FOUND";
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = "系統發生錯誤，請稍後再試";
                errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                break;
        }

        // 記錄錯誤日誌
        _logger.LogError($"發生例外: {exception.Message}", exception);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(jsonResponse);
    }
}

