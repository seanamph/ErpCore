using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 業務報表查詢作業控制器 (SYSL135)
/// </summary>
[Route("api/v1/business-reports/sysl135")]
public class BusinessReportController : BaseController
{
    private readonly IBusinessReportService _service;

    public BusinessReportController(
        IBusinessReportService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢業務報表列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportDto>>>> GetBusinessReports(
        [FromQuery] BusinessReportQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportsAsync(query);
            return result;
        }, "查詢業務報表列表失敗");
    }
}

