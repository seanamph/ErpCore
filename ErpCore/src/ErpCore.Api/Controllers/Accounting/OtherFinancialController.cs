using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Accounting;

/// <summary>
/// 其他財務功能控制器 (SYSN610-SYSN910)
/// 提供財務其他功能、財務報表擴展、財務其他報表、租賃報表等功能
/// </summary>
[Route("api/v1/accounting/other-financial")]
public class OtherFinancialController : BaseController
{
    public OtherFinancialController(ILoggerService logger) : base(logger)
    {
    }

    /// <summary>
    /// 查詢其他財務功能
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetOtherFinancialFunctions()
    {
        return await ExecuteAsync(async () =>
        {
            // 其他財務功能查詢（簡化版本，實際應根據業務需求實作）
            await Task.CompletedTask;
            return new
            {
                Functions = new[]
                {
                    new { FunctionId = "SYSN610", FunctionName = "財務其他功能", Status = "A" },
                    new { FunctionId = "SYSN810", FunctionName = "財務其他報表", Status = "A" },
                    new { FunctionId = "SYSN830", FunctionName = "財務其他報表擴展", Status = "A" }
                }
            };
        }, "查詢其他財務功能失敗");
    }
}

