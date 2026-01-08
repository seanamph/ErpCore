using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.ReportManagement;

/// <summary>
/// 收款處理功能控制器 (SYSR210-SYSR240)
/// 提供應收帳款維護作業
/// </summary>
[Route("api/v1/receipt")]
public class ReceivingProcessController : BaseController
{
    private readonly IAccountsReceivableService _accountsReceivableService;

    public ReceivingProcessController(
        IAccountsReceivableService accountsReceivableService,
        ILoggerService logger) : base(logger)
    {
        _accountsReceivableService = accountsReceivableService;
    }

    #region 應收帳款維護 (SYSR210-SYSR240)

    /// <summary>
    /// 查詢應收帳款列表
    /// </summary>
    [HttpGet("accountsreceivable")]
    public async Task<ActionResult<ApiResponse<IEnumerable<AccountsReceivableDto>>>> GetAccountsReceivable(
        [FromQuery] string? siteId = null,
        [FromQuery] string? shopId = null,
        [FromQuery] string? objectId = null,
        [FromQuery] string? receiptNo = null,
        [FromQuery] string? voucherNo = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? aritemId = null)
    {
        return await ExecuteAsync(async () =>
        {
            IEnumerable<AccountsReceivableDto> result;

            if (startDate.HasValue || endDate.HasValue)
            {
                result = await _accountsReceivableService.GetByReceiptDateRangeAsync(startDate, endDate);
            }
            else if (!string.IsNullOrEmpty(voucherNo))
            {
                result = await _accountsReceivableService.GetByVoucherNoAsync(voucherNo);
            }
            else if (!string.IsNullOrEmpty(receiptNo))
            {
                result = await _accountsReceivableService.GetByReceiptNoAsync(receiptNo);
            }
            else if (!string.IsNullOrEmpty(objectId))
            {
                result = await _accountsReceivableService.GetByObjectIdAsync(objectId);
            }
            else
            {
                result = await _accountsReceivableService.GetAllAsync();
            }

            // 進行額外過濾
            if (!string.IsNullOrEmpty(siteId))
            {
                result = result.Where(x => x.SiteId == siteId);
            }
            if (!string.IsNullOrEmpty(shopId))
            {
                result = result.Where(x => x.ShopId == shopId);
            }
            if (!string.IsNullOrEmpty(aritemId))
            {
                result = result.Where(x => x.AritemId == aritemId);
            }

            return result;
        }, "查詢應收帳款列表失敗");
    }

    /// <summary>
    /// 查詢單筆應收帳款
    /// </summary>
    [HttpGet("accountsreceivable/{tKey}")]
    public async Task<ActionResult<ApiResponse<AccountsReceivableDto>>> GetAccountsReceivable(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _accountsReceivableService.GetByIdAsync(tKey);
            return result;
        }, $"查詢應收帳款失敗: {tKey}");
    }

    /// <summary>
    /// 新增應收帳款
    /// </summary>
    [HttpPost("accountsreceivable")]
    public async Task<ActionResult<ApiResponse<AccountsReceivableDto>>> CreateAccountsReceivable(
        [FromBody] CreateAccountsReceivableDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _accountsReceivableService.CreateAsync(dto);
            return result;
        }, "新增應收帳款失敗");
    }

    /// <summary>
    /// 修改應收帳款
    /// </summary>
    [HttpPut("accountsreceivable/{tKey}")]
    public async Task<ActionResult<ApiResponse<AccountsReceivableDto>>> UpdateAccountsReceivable(
        long tKey,
        [FromBody] UpdateAccountsReceivableDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _accountsReceivableService.UpdateAsync(tKey, dto);
            return result;
        }, $"修改應收帳款失敗: {tKey}");
    }

    /// <summary>
    /// 刪除應收帳款
    /// </summary>
    [HttpDelete("accountsreceivable/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAccountsReceivable(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _accountsReceivableService.DeleteAsync(tKey);
        }, $"刪除應收帳款失敗: {tKey}");
    }

    #endregion
}

