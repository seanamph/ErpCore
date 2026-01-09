using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Loyalty;
using ErpCore.Application.Services.Loyalty;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Loyalty;

/// <summary>
/// 忠誠度系統維護控制器 (LPS - 忠誠度系統維護)
/// </summary>
[Route("api/v1/loyalty-point-transactions")]
public class LoyaltyMaintenanceController : BaseController
{
    private readonly ILoyaltyMaintenanceService _service;

    public LoyaltyMaintenanceController(
        ILoyaltyMaintenanceService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 查詢點數交易列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<LoyaltyPointTransactionDto>>>> GetTransactions(
        [FromQuery] LoyaltyPointTransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransactionsAsync(query);
            return result;
        }, "查詢點數交易列表失敗");
    }

    /// <summary>
    /// 查詢單筆點數交易
    /// </summary>
    [HttpGet("{rrn}")]
    public async Task<ActionResult<ApiResponse<LoyaltyPointTransactionDto>>> GetTransaction(string rrn)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTransactionByRrnAsync(rrn);
            return result;
        }, $"查詢點數交易失敗: {rrn}");
    }

    /// <summary>
    /// 新增點數交易
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateTransaction(
        [FromBody] CreateLoyaltyPointTransactionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateTransactionAsync(dto);
            return result;
        }, "新增點數交易失敗");
    }

    /// <summary>
    /// 取消點數交易
    /// </summary>
    [HttpPost("{rrn}/void")]
    public async Task<ActionResult<ApiResponse<bool>>> VoidTransaction(
        string rrn,
        [FromBody] VoidLoyaltyPointTransactionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.VoidTransactionAsync(rrn, dto);
            return true;
        }, $"取消點數交易失敗: {rrn}");
    }

    /// <summary>
    /// 查詢會員列表
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<ApiResponse<PagedResult<LoyaltyMemberDto>>>> GetMembers(
        [FromQuery] LoyaltyMemberQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMembersAsync(query);
            return result;
        }, "查詢會員列表失敗");
    }

    /// <summary>
    /// 查詢單筆會員
    /// </summary>
    [HttpGet("members/{cardNo}")]
    public async Task<ActionResult<ApiResponse<LoyaltyMemberDto>>> GetMember(string cardNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMemberByCardNoAsync(cardNo);
            return result;
        }, $"查詢會員失敗: {cardNo}");
    }

    /// <summary>
    /// 查詢會員點數
    /// </summary>
    [HttpGet("members/{cardNo}/points")]
    public async Task<ActionResult<ApiResponse<LoyaltyMemberPointsDto>>> GetMemberPoints(string cardNo)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetMemberPointsAsync(cardNo);
            return result;
        }, $"查詢會員點數失敗: {cardNo}");
    }

    /// <summary>
    /// 新增會員
    /// </summary>
    [HttpPost("members")]
    public async Task<ActionResult<ApiResponse<string>>> CreateMember(
        [FromBody] CreateLoyaltyMemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.CreateMemberAsync(dto);
            return result;
        }, "新增會員失敗");
    }

    /// <summary>
    /// 修改會員
    /// </summary>
    [HttpPut("members/{cardNo}")]
    public async Task<ActionResult<ApiResponse<bool>>> UpdateMember(
        string cardNo,
        [FromBody] UpdateLoyaltyMemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.UpdateMemberAsync(cardNo, dto);
            return true;
        }, $"修改會員失敗: {cardNo}");
    }
}

