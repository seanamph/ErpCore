using ErpCore.Shared.Logging;
using System.Diagnostics;

namespace ErpCore.Api.Middleware;

/// <summary>
/// 日誌記錄中介軟體
/// 記錄所有API請求的詳細資訊
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;

    public LoggingMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestPath = context.Request.Path.Value ?? "";
        var requestMethod = context.Request.Method;
        var requestQuery = context.Request.QueryString.Value ?? "";

        // 記錄請求開始
        _logger.LogInfo($"API請求開始: {requestMethod} {requestPath}{requestQuery}");

        try
        {
            await _next(context);
            
            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            
            // 記錄請求完成
            _logger.LogInfo($"API請求完成: {requestMethod} {requestPath} - 狀態碼: {statusCode} - 耗時: {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"API請求發生錯誤: {requestMethod} {requestPath} - 耗時: {stopwatch.ElapsedMilliseconds}ms", ex);
            throw;
        }
    }
}

