using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Accounting;

/// <summary>
/// 財務報表控制器 (SYSN510-SYSN540)
/// 提供財務報表查詢、列印、統計、分析等功能
/// </summary>
[Route("api/v1/accounting/financial-reports")]
public class FinancialReportController : BaseController
{
    public FinancialReportController(ILoggerService logger) : base(logger)
    {
    }

    /// <summary>
    /// 查詢財務報表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetFinancialReports()
    {
        return await ExecuteAsync(async () =>
        {
            // TODO: 實作財務報表查詢功能
            await Task.CompletedTask;
            return new { Message = "財務報表查詢功能開發中" };
        }, "查詢財務報表失敗");
    }
}

