using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.BusinessReport;

/// <summary>
/// 業務報表管理作業控制器 (SYSL145)
/// </summary>
[Route("api/v1/business-report-management")]
public class BusinessReportManagementController : BaseController
{
    private readonly IBusinessReportManagementService _service;

    public BusinessReportManagementController(
        IBusinessReportManagementService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢業務報表管理列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<BusinessReportManagementDto>>>> GetBusinessReportManagements(
        [FromQuery] BusinessReportManagementQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportManagementsAsync(query);
            return result;
        }, "查詢業務報表管理列表失敗");
    }

    /// <summary>
    /// 查詢單筆業務報表管理
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<BusinessReportManagementDto>>> GetBusinessReportManagement(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetBusinessReportManagementByIdAsync(tKey);
            return result;
        }, $"查詢業務報表管理失敗: {tKey}");
    }

    /// <summary>
    /// 新增業務報表管理
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateBusinessReportManagement(
        [FromBody] CreateBusinessReportManagementDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateBusinessReportManagementAsync(dto);
            return result;
        }, "新增業務報表管理失敗");
    }

    /// <summary>
    /// 修改業務報表管理
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateBusinessReportManagement(
        long tKey,
        [FromBody] UpdateBusinessReportManagementDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateBusinessReportManagementAsync(tKey, dto);
        }, $"修改業務報表管理失敗: {tKey}");
    }

    /// <summary>
    /// 刪除業務報表管理
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteBusinessReportManagement(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteBusinessReportManagementAsync(tKey);
        }, $"刪除業務報表管理失敗: {tKey}");
    }

    /// <summary>
    /// 批次刪除業務報表管理
    /// </summary>
    [HttpDelete("batch")]
    public async Task<ActionResult<ApiResponse<int>>> BatchDeleteBusinessReportManagement(
        [FromBody] BatchDeleteDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.BatchDeleteBusinessReportManagementAsync(dto.TKeys);
            return result;
        }, "批次刪除業務報表管理失敗");
    }

    /// <summary>
    /// 載入管理權限資料
    /// </summary>
    [HttpGet("load-management")]
    public async Task<ActionResult<ApiResponse<List<BusinessReportManagementDto>>>> LoadManagementData()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.LoadManagementDataAsync();
            return result;
        }, "載入管理權限資料失敗");
    }

    /// <summary>
    /// 檢查重複資料
    /// </summary>
    [HttpPost("check-duplicate")]
    public async Task<ActionResult<ApiResponse<CheckDuplicateResultDto>>> CheckDuplicate(
        [FromBody] CheckDuplicateDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CheckDuplicateAsync(dto);
            return result;
        }, "檢查重複資料失敗");
    }
}

