using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Accounting;
using ErpCore.Application.Services.Accounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Accounting;

/// <summary>
/// 會計科目控制器 (SYSN110)
/// 提供會計科目資料的新增、修改、刪除、查詢功能
/// </summary>
[Route("api/v1/accounting/account-subjects")]
public class AccountSubjectController : BaseController
{
    private readonly IAccountSubjectService _service;

    public AccountSubjectController(
        IAccountSubjectService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢會計科目列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<AccountSubjectDto>>>> GetAccountSubjects(
        [FromQuery] AccountSubjectQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAccountSubjectsAsync(query);
            return result;
        }, "查詢會計科目列表失敗");
    }

    /// <summary>
    /// 根據科目代號查詢會計科目
    /// </summary>
    [HttpGet("{stypeId}")]
    public async Task<ActionResult<ApiResponse<AccountSubjectDto>>> GetAccountSubject(string stypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetAccountSubjectByIdAsync(stypeId);
            return result;
        }, "查詢會計科目失敗");
    }

    /// <summary>
    /// 新增會計科目
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<string>>> CreateAccountSubject(
        [FromBody] CreateAccountSubjectDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateAccountSubjectAsync(dto);
            return result;
        }, "新增會計科目失敗");
    }

    /// <summary>
    /// 修改會計科目
    /// </summary>
    [HttpPut("{stypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateAccountSubject(
        string stypeId,
        [FromBody] UpdateAccountSubjectDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateAccountSubjectAsync(stypeId, dto);
        }, "修改會計科目失敗");
    }

    /// <summary>
    /// 刪除會計科目
    /// </summary>
    [HttpDelete("{stypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteAccountSubject(string stypeId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteAccountSubjectAsync(stypeId);
        }, "刪除會計科目失敗");
    }

    /// <summary>
    /// 檢查科目代號是否存在
    /// </summary>
    [HttpGet("{stypeId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckExists(string stypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExistsAsync(stypeId);
            return result;
        }, "檢查科目代號是否存在失敗");
    }

    /// <summary>
    /// 檢查未沖帳餘額
    /// </summary>
    [HttpGet("{stypeId}/check-unsettled-balance")]
    public async Task<ActionResult<ApiResponse<UnsettledBalanceDto>>> CheckUnsettledBalance(string stypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CheckUnsettledBalanceAsync(stypeId);
            return result;
        }, "檢查未沖帳餘額失敗");
    }
}

