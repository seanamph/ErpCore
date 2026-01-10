using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ErpCore.Shared.Common;

namespace ErpCore.Api.Filters;

/// <summary>
/// 模型驗證過濾器
/// 自動驗證ModelState，如果驗證失敗則返回錯誤回應
/// </summary>
public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .SelectMany(x => x.Value!.Errors.Select(e => new
                {
                    Field = x.Key,
                    Message = e.ErrorMessage
                }))
                .ToList();

            var errorMessage = string.Join("; ", errors.Select(e => $"{e.Field}: {e.Message}"));
            
            context.Result = new BadRequestObjectResult(
                ApiResponse<object>.Fail($"資料驗證失敗: {errorMessage}", "VALIDATION_ERROR"));
        }

        base.OnActionExecuting(context);
    }
}

