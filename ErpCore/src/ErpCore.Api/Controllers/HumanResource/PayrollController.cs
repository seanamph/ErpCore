using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.HumanResource;
using ErpCore.Application.Services.HumanResource;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.HumanResource;

/// <summary>
/// 薪資管理控制器 (SYSH210)
/// 提供員工薪資資料維護功能
/// </summary>
[Route("api/v1/human-resource/payroll")]
public class PayrollController : BaseController
{
    private readonly IPayrollService _service;

    public PayrollController(
        IPayrollService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢薪資列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<PayrollDto>>>> GetPayrolls(
        [FromQuery] PayrollQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPayrollsAsync(query);
            return result;
        }, "查詢薪資列表失敗");
    }

    /// <summary>
    /// 查詢單筆薪資
    /// </summary>
    [HttpGet("{payrollId}")]
    public async Task<ActionResult<ApiResponse<PayrollDto>>> GetPayroll(string payrollId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetPayrollByIdAsync(payrollId);
            return result;
        }, $"查詢薪資失敗: {payrollId}");
    }

    /// <summary>
    /// 新增薪資
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreatePayroll(
        [FromBody] CreatePayrollDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreatePayrollAsync(dto);
            return result;
        }, "新增薪資失敗");
    }

    /// <summary>
    /// 修改薪資
    /// </summary>
    [HttpPut("{payrollId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdatePayroll(
        string payrollId,
        [FromBody] UpdatePayrollDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdatePayrollAsync(payrollId, dto);
        }, $"修改薪資失敗: {payrollId}");
    }

    /// <summary>
    /// 刪除薪資
    /// </summary>
    [HttpDelete("{payrollId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeletePayroll(string payrollId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeletePayrollAsync(payrollId);
        }, $"刪除薪資失敗: {payrollId}");
    }

    /// <summary>
    /// 確認薪資
    /// </summary>
    [HttpPost("{payrollId}/confirm")]
    public async Task<ActionResult<ApiResponse<object>>> ConfirmPayroll(string payrollId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.ConfirmPayrollAsync(payrollId);
        }, $"確認薪資失敗: {payrollId}");
    }

    /// <summary>
    /// 計算薪資（不儲存）
    /// </summary>
    [HttpPost("calculate")]
    public async Task<ActionResult<ApiResponse<PayrollCalculationResultDto>>> CalculatePayroll(
        [FromBody] CalculatePayrollDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CalculatePayrollAsync(dto);
            return result;
        }, "計算薪資失敗");
    }
}

