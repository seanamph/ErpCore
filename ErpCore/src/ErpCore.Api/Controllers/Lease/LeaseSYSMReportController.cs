using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Lease;

/// <summary>
/// 租賃報表查詢控制器 (SYSM141-SYSM144)
/// </summary>
[ApiController]
[Route("api/v1/lease-sysm/reports")]
public class LeaseSYSMReportController : BaseController
{
    private readonly ILeaseReportQueryService _leaseReportQueryService;

    public LeaseSYSMReportController(
        ILeaseReportQueryService leaseReportQueryService,
        ILoggerService logger) : base(logger)
    {
        _leaseReportQueryService = leaseReportQueryService;
    }

    /// <summary>
    /// 查詢租賃報表查詢記錄列表
    /// </summary>
    [HttpGet("queries")]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseReportQueryDto>>>> GetLeaseReportQueries(
        [FromQuery] LeaseReportQueryQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseReportQueryService.GetLeaseReportQueriesAsync(query);
            return result;
        }, "查詢租賃報表查詢記錄列表失敗");
    }

    /// <summary>
    /// 查詢單筆租賃報表查詢記錄
    /// </summary>
    [HttpGet("queries/{queryId}")]
    public async Task<ActionResult<ApiResponse<LeaseReportQueryDto>>> GetLeaseReportQuery(string queryId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseReportQueryService.GetLeaseReportQueryByIdAsync(queryId);
            return result;
        }, $"查詢租賃報表查詢記錄失敗: {queryId}");
    }

    /// <summary>
    /// 新增租賃報表查詢記錄
    /// </summary>
    [HttpPost("queries")]
    public async Task<ActionResult<ApiResponse<LeaseReportQueryDto>>> CreateLeaseReportQuery(
        [FromBody] CreateLeaseReportQueryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseReportQueryService.CreateLeaseReportQueryAsync(dto);
            return result;
        }, "新增租賃報表查詢記錄失敗");
    }

    /// <summary>
    /// 刪除租賃報表查詢記錄
    /// </summary>
    [HttpDelete("queries/{queryId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseReportQuery(string queryId)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseReportQueryService.DeleteLeaseReportQueryAsync(queryId);
        }, $"刪除租賃報表查詢記錄失敗: {queryId}");
    }

    /// <summary>
    /// 檢查租賃報表查詢記錄是否存在
    /// </summary>
    [HttpGet("queries/{queryId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaseReportQueryExists(string queryId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseReportQueryService.ExistsAsync(queryId);
            return result;
        }, $"檢查租賃報表查詢記錄是否存在失敗: {queryId}");
    }
}

