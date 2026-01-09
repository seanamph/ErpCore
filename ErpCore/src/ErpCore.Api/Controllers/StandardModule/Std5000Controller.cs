using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Application.Services.StandardModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.StandardModule;

/// <summary>
/// STD5000 標準模組控制器 (SYS5110-SYS56B0 - 標準模組系列)
/// </summary>
[Route("api/v1/std5000")]
public class Std5000Controller : BaseController
{
    private readonly IStd5000BaseDataService _baseDataService;
    private readonly IStd5000MemberService _memberService;
    private readonly IStd5000TransactionService _transactionService;

    public Std5000Controller(
        IStd5000BaseDataService baseDataService,
        IStd5000MemberService memberService,
        IStd5000TransactionService transactionService,
        ILoggerService logger) : base(logger)
    {
        _baseDataService = baseDataService;
        _memberService = memberService;
        _transactionService = transactionService;
    }

    #region 基礎資料管理 (SYS5110-SYS5150)

    /// <summary>
    /// 查詢STD5000基礎資料列表
    /// </summary>
    [HttpGet("basedata")]
    public async Task<ActionResult<ApiResponse<PagedResult<Std5000BaseDataDto>>>> GetStd5000BaseDataList(
        [FromQuery] Std5000BaseDataQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _baseDataService.GetStd5000BaseDataListAsync(query);
            return result;
        }, "查詢STD5000基礎資料列表失敗");
    }

    /// <summary>
    /// 查詢單筆STD5000基礎資料
    /// </summary>
    [HttpGet("basedata/{tKey}")]
    public async Task<ActionResult<ApiResponse<Std5000BaseDataDto>>> GetStd5000BaseData(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _baseDataService.GetStd5000BaseDataByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }
            return result;
        }, $"查詢STD5000基礎資料失敗: {tKey}");
    }

    /// <summary>
    /// 新增STD5000基礎資料
    /// </summary>
    [HttpPost("basedata")]
    public async Task<ActionResult<ApiResponse<long>>> CreateStd5000BaseData(
        [FromBody] CreateStd5000BaseDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _baseDataService.CreateStd5000BaseDataAsync(dto);
            return result;
        }, "新增STD5000基礎資料失敗");
    }

    /// <summary>
    /// 修改STD5000基礎資料
    /// </summary>
    [HttpPut("basedata/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStd5000BaseData(
        long tKey,
        [FromBody] UpdateStd5000BaseDataDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _baseDataService.UpdateStd5000BaseDataAsync(tKey, dto);
        }, $"修改STD5000基礎資料失敗: {tKey}");
    }

    /// <summary>
    /// 刪除STD5000基礎資料
    /// </summary>
    [HttpDelete("basedata/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStd5000BaseData(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _baseDataService.DeleteStd5000BaseDataAsync(tKey);
        }, $"刪除STD5000基礎資料失敗: {tKey}");
    }

    #endregion

    #region 會員管理 (SYS5210-SYS52A0)

    /// <summary>
    /// 查詢STD5000會員列表
    /// </summary>
    [HttpGet("members")]
    public async Task<ActionResult<ApiResponse<PagedResult<Std5000MemberDto>>>> GetStd5000MemberList(
        [FromQuery] Std5000MemberQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetStd5000MemberListAsync(query);
            return result;
        }, "查詢STD5000會員列表失敗");
    }

    /// <summary>
    /// 查詢單筆STD5000會員
    /// </summary>
    [HttpGet("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<Std5000MemberDto>>> GetStd5000Member(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetStd5000MemberByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"會員不存在: {tKey}");
            }
            return result;
        }, $"查詢STD5000會員失敗: {tKey}");
    }

    /// <summary>
    /// 新增STD5000會員
    /// </summary>
    [HttpPost("members")]
    public async Task<ActionResult<ApiResponse<long>>> CreateStd5000Member(
        [FromBody] CreateStd5000MemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.CreateStd5000MemberAsync(dto);
            return result;
        }, "新增STD5000會員失敗");
    }

    /// <summary>
    /// 修改STD5000會員
    /// </summary>
    [HttpPut("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStd5000Member(
        long tKey,
        [FromBody] UpdateStd5000MemberDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _memberService.UpdateStd5000MemberAsync(tKey, dto);
        }, $"修改STD5000會員失敗: {tKey}");
    }

    /// <summary>
    /// 刪除STD5000會員
    /// </summary>
    [HttpDelete("members/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStd5000Member(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _memberService.DeleteStd5000MemberAsync(tKey);
        }, $"刪除STD5000會員失敗: {tKey}");
    }

    /// <summary>
    /// 查詢會員積分列表
    /// </summary>
    [HttpGet("members/points")]
    public async Task<ActionResult<ApiResponse<PagedResult<Std5000MemberPointDto>>>> GetMemberPoints(
        [FromQuery] Std5000MemberPointQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.GetMemberPointsAsync(query);
            return result;
        }, "查詢會員積分列表失敗");
    }

    /// <summary>
    /// 新增會員積分
    /// </summary>
    [HttpPost("members/points")]
    public async Task<ActionResult<ApiResponse<long>>> AddMemberPoint(
        [FromBody] CreateStd5000MemberPointDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _memberService.AddMemberPointAsync(dto);
            return result;
        }, "新增會員積分失敗");
    }

    #endregion

    #region 交易管理 (SYS5310-SYS53C6)

    /// <summary>
    /// 查詢STD5000交易列表
    /// </summary>
    [HttpGet("transactions")]
    public async Task<ActionResult<ApiResponse<PagedResult<Std5000TransactionDto>>>> GetStd5000TransactionList(
        [FromQuery] Std5000TransactionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _transactionService.GetStd5000TransactionListAsync(query);
            return result;
        }, "查詢STD5000交易列表失敗");
    }

    /// <summary>
    /// 查詢單筆STD5000交易
    /// </summary>
    [HttpGet("transactions/{tKey}")]
    public async Task<ActionResult<ApiResponse<Std5000TransactionDto>>> GetStd5000Transaction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _transactionService.GetStd5000TransactionByIdAsync(tKey);
            if (result == null)
            {
                throw new InvalidOperationException($"交易不存在: {tKey}");
            }
            return result;
        }, $"查詢STD5000交易失敗: {tKey}");
    }

    /// <summary>
    /// 新增STD5000交易
    /// </summary>
    [HttpPost("transactions")]
    public async Task<ActionResult<ApiResponse<long>>> CreateStd5000Transaction(
        [FromBody] CreateStd5000TransactionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _transactionService.CreateStd5000TransactionAsync(dto);
            return result;
        }, "新增STD5000交易失敗");
    }

    /// <summary>
    /// 修改STD5000交易
    /// </summary>
    [HttpPut("transactions/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStd5000Transaction(
        long tKey,
        [FromBody] UpdateStd5000TransactionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _transactionService.UpdateStd5000TransactionAsync(tKey, dto);
        }, $"修改STD5000交易失敗: {tKey}");
    }

    /// <summary>
    /// 刪除STD5000交易
    /// </summary>
    [HttpDelete("transactions/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteStd5000Transaction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _transactionService.DeleteStd5000TransactionAsync(tKey);
        }, $"刪除STD5000交易失敗: {tKey}");
    }

    #endregion
}

