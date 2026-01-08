using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Lease;

/// <summary>
/// 租賃費用管理控制器 (SYSE310-SYSE430)
/// </summary>
[ApiController]
[Route("api/v1/lease-syse/fees")]
public class LeaseSYSEFeeController : BaseController
{
    private readonly ILeaseFeeService _leaseFeeService;
    private readonly ILeaseFeeItemService _leaseFeeItemService;

    public LeaseSYSEFeeController(
        ILeaseFeeService leaseFeeService,
        ILeaseFeeItemService leaseFeeItemService,
        ILoggerService logger) : base(logger)
    {
        _leaseFeeService = leaseFeeService;
        _leaseFeeItemService = leaseFeeItemService;
    }

    #region 費用主檔 (LeaseFee)

    /// <summary>
    /// 查詢費用列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseFeeDto>>>> GetLeaseFees(
        [FromQuery] LeaseFeeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeService.GetLeaseFeesAsync(query);
            return result;
        }, "查詢費用列表失敗");
    }

    /// <summary>
    /// 查詢單筆費用
    /// </summary>
    [HttpGet("{feeId}")]
    public async Task<ActionResult<ApiResponse<LeaseFeeDto>>> GetLeaseFee(string feeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeService.GetLeaseFeeByIdAsync(feeId);
            return result;
        }, $"查詢費用失敗: {feeId}");
    }

    /// <summary>
    /// 根據租賃編號查詢費用
    /// </summary>
    [HttpGet("leases/{leaseId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LeaseFeeDto>>>> GetLeaseFeesByLeaseId(string leaseId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeService.GetLeaseFeesByLeaseIdAsync(leaseId);
            return result;
        }, $"查詢費用失敗: {leaseId}");
    }

    /// <summary>
    /// 新增費用
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<LeaseFeeDto>>> CreateLeaseFee(
        [FromBody] CreateLeaseFeeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeService.CreateLeaseFeeAsync(dto);
            return result;
        }, "新增費用失敗");
    }

    /// <summary>
    /// 修改費用
    /// </summary>
    [HttpPut("{feeId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseFee(
        string feeId,
        [FromBody] UpdateLeaseFeeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseFeeService.UpdateLeaseFeeAsync(feeId, dto);
        }, $"修改費用失敗: {feeId}");
    }

    /// <summary>
    /// 刪除費用
    /// </summary>
    [HttpDelete("{feeId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseFee(string feeId)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseFeeService.DeleteLeaseFeeAsync(feeId);
        }, $"刪除費用失敗: {feeId}");
    }

    /// <summary>
    /// 更新費用狀態
    /// </summary>
    [HttpPut("{feeId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseFeeStatus(
        string feeId,
        [FromBody] UpdateLeaseFeeStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseFeeService.UpdateLeaseFeeStatusAsync(feeId, dto.Status);
        }, $"更新費用狀態失敗: {feeId}");
    }

    /// <summary>
    /// 更新費用已付金額
    /// </summary>
    [HttpPut("{feeId}/paid-amount")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseFeePaidAmount(
        string feeId,
        [FromBody] UpdateLeaseFeePaidAmountDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseFeeService.UpdateLeaseFeePaidAmountAsync(feeId, dto.PaidAmount, dto.PaidDate);
        }, $"更新費用已付金額失敗: {feeId}");
    }

    /// <summary>
    /// 檢查費用是否存在
    /// </summary>
    [HttpGet("{feeId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaseFeeExists(string feeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeService.ExistsAsync(feeId);
            return result;
        }, $"檢查費用是否存在失敗: {feeId}");
    }

    #endregion

    #region 費用項目主檔 (LeaseFeeItem)

    /// <summary>
    /// 查詢費用項目列表
    /// </summary>
    [HttpGet("items")]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseFeeItemDto>>>> GetLeaseFeeItems(
        [FromQuery] LeaseFeeItemQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeItemService.GetLeaseFeeItemsAsync(query);
            return result;
        }, "查詢費用項目列表失敗");
    }

    /// <summary>
    /// 查詢單筆費用項目
    /// </summary>
    [HttpGet("items/{feeItemId}")]
    public async Task<ActionResult<ApiResponse<LeaseFeeItemDto>>> GetLeaseFeeItem(string feeItemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeItemService.GetLeaseFeeItemByIdAsync(feeItemId);
            return result;
        }, $"查詢費用項目失敗: {feeItemId}");
    }

    /// <summary>
    /// 新增費用項目
    /// </summary>
    [HttpPost("items")]
    public async Task<ActionResult<ApiResponse<LeaseFeeItemDto>>> CreateLeaseFeeItem(
        [FromBody] CreateLeaseFeeItemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeItemService.CreateLeaseFeeItemAsync(dto);
            return result;
        }, "新增費用項目失敗");
    }

    /// <summary>
    /// 修改費用項目
    /// </summary>
    [HttpPut("items/{feeItemId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseFeeItem(
        string feeItemId,
        [FromBody] UpdateLeaseFeeItemDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseFeeItemService.UpdateLeaseFeeItemAsync(feeItemId, dto);
        }, $"修改費用項目失敗: {feeItemId}");
    }

    /// <summary>
    /// 刪除費用項目
    /// </summary>
    [HttpDelete("items/{feeItemId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseFeeItem(string feeItemId)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseFeeItemService.DeleteLeaseFeeItemAsync(feeItemId);
        }, $"刪除費用項目失敗: {feeItemId}");
    }

    /// <summary>
    /// 更新費用項目狀態
    /// </summary>
    [HttpPut("items/{feeItemId}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseFeeItemStatus(
        string feeItemId,
        [FromBody] UpdateLeaseFeeItemStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseFeeItemService.UpdateLeaseFeeItemStatusAsync(feeItemId, dto.Status);
        }, $"更新費用項目狀態失敗: {feeItemId}");
    }

    /// <summary>
    /// 檢查費用項目是否存在
    /// </summary>
    [HttpGet("items/{feeItemId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaseFeeItemExists(string feeItemId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseFeeItemService.ExistsAsync(feeItemId);
            return result;
        }, $"檢查費用項目是否存在失敗: {feeItemId}");
    }

    #endregion
}

