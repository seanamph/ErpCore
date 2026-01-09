using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.SystemExit;

/// <summary>
/// 系統退出控制器 (SYS9999 - 系統退出功能)
/// </summary>
[Route("api/v1/system-exit")]
public class SystemExitController : BaseController
{
    public SystemExitController(ILoggerService logger) : base(logger)
    {
    }

    /// <summary>
    /// 系統退出
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<object>>> Exit()
    {
        return await ExecuteAsync(async () =>
        {
            var userId = User?.Identity?.Name ?? "SYSTEM";
            _logger.LogInfo($"系統退出請求 - 使用者: {userId}");
            
            // 記錄退出日誌
            // 這裡可以加入清理資源、保存資料等邏輯
            
            return new { Message = "系統退出成功" };
        }, "系統退出失敗");
    }
}

