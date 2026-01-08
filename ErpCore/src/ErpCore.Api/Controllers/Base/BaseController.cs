using Microsoft.AspNetCore.Mvc;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Base;

/// <summary>
/// 基礎控制器
/// 提供統一的錯誤處理和回應格式
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected readonly ILoggerService _logger;

    protected BaseController(ILoggerService logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 處理成功回應
    /// </summary>
    protected ActionResult<ApiResponse<T>> OkResponse<T>(T data, string message = "操作成功")
    {
        return Ok(ApiResponse<T>.Ok(data, message));
    }

    /// <summary>
    /// 處理失敗回應
    /// </summary>
    protected ActionResult<ApiResponse<T>> FailResponse<T>(string message, string? errorCode = null)
    {
        return BadRequest(ApiResponse<T>.Fail(message, errorCode));
    }

    /// <summary>
    /// 執行操作並處理異常
    /// </summary>
    protected async Task<ActionResult<ApiResponse<T>>> ExecuteAsync<T>(Func<Task<T>> action, string? errorMessage = null)
    {
        try
        {
            var result = await action();
            return OkResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(errorMessage ?? "操作失敗", ex);
            return FailResponse<T>(errorMessage ?? ex.Message, "SYSTEM_ERROR");
        }
    }

    /// <summary>
    /// 執行操作並處理異常（無返回值）
    /// </summary>
    protected async Task<ActionResult<ApiResponse<object>>> ExecuteAsync(Func<Task> action, string? errorMessage = null)
    {
        try
        {
            await action();
            return OkResponse<object>(null!, "操作成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(errorMessage ?? "操作失敗", ex);
            return FailResponse<object>(errorMessage ?? ex.Message, "SYSTEM_ERROR");
        }
    }
}

