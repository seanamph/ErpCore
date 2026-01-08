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
    public async Task<ActionResult> GetOtherFinancialFunctions()
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: 實作其他財務功能
            await Task.CompletedTask;
            return new { Message = "其他財務功能開發中" };
        }, "查詢其他財務功能失敗");
    }
}

