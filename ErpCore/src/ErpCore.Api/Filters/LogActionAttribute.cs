using Microsoft.AspNetCore.Mvc.Filters;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Filters;

/// <summary>
/// 動作日誌過濾器
/// 記錄控制器動作的執行資訊
/// </summary>
public class LogActionAttribute : ActionFilterAttribute
{
    private readonly ILoggerService? _logger;

    public LogActionAttribute()
    {
    }

    public LogActionAttribute(ILoggerService logger)
    {
        _logger = logger;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var logger = _logger ?? context.HttpContext.RequestServices.GetService<ILoggerService>();
        if (logger != null)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();
            logger.LogInfo($"執行動作: {controllerName}.{actionName}");
        }

        base.OnActionExecuting(context);
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        var logger = _logger ?? context.HttpContext.RequestServices.GetService<ILoggerService>();
        if (logger != null)
        {
            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();
            
            if (context.Exception != null)
            {
                logger.LogError($"動作執行失敗: {controllerName}.{actionName}", context.Exception);
            }
            else
            {
                logger.LogInfo($"動作執行完成: {controllerName}.{actionName}");
            }
        }

        base.OnActionExecuted(context);
    }
}

