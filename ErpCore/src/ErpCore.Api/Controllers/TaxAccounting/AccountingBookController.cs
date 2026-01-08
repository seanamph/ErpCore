using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.TaxAccounting;
using ErpCore.Application.Services.TaxAccounting;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.TaxAccounting;

/// <summary>
/// 會計帳簿管理控制器 (SYST131-SYST134)
/// 提供現金流量分類資料的新增、修改、刪除、查詢功能
/// </summary>
[Route("api/v1/tax-accounting/accounting-books")]
public class AccountingBookController : BaseController
{
    private readonly ICashFlowLargeTypeService _largeTypeService;
    private readonly ICashFlowMediumTypeService _mediumTypeService;
    private readonly ICashFlowSubjectTypeService _subjectTypeService;
    private readonly ICashFlowSubTotalService _subTotalService;

    public AccountingBookController(
        ICashFlowLargeTypeService largeTypeService,
        ICashFlowMediumTypeService mediumTypeService,
        ICashFlowSubjectTypeService subjectTypeService,
        ICashFlowSubTotalService subTotalService,
        ILoggerService logger) : base(logger)
    {
        _largeTypeService = largeTypeService;
        _mediumTypeService = mediumTypeService;
        _subjectTypeService = subjectTypeService;
        _subTotalService = subTotalService;
    }

    #region SYST131 - 現金流量大分類設定

    /// <summary>
    /// 查詢現金流量大分類列表
    /// </summary>
    [HttpGet("large-types")]
    public async Task<ActionResult<ApiResponse<PagedResult<CashFlowLargeTypeDto>>>> GetLargeTypes(
        [FromQuery] CashFlowLargeTypeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _largeTypeService.GetCashFlowLargeTypesAsync(query);
            return result;
        }, "查詢現金流量大分類列表失敗");
    }

    /// <summary>
    /// 查詢單筆現金流量大分類
    /// </summary>
    [HttpGet("large-types/{cashLTypeId}")]
    public async Task<ActionResult<ApiResponse<CashFlowLargeTypeDto>>> GetLargeType(string cashLTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _largeTypeService.GetCashFlowLargeTypeByIdAsync(cashLTypeId);
            return result;
        }, $"查詢現金流量大分類失敗: {cashLTypeId}");
    }

    /// <summary>
    /// 新增現金流量大分類
    /// </summary>
    [HttpPost("large-types")]
    public async Task<ActionResult<ApiResponse<string>>> CreateLargeType(
        [FromBody] CreateCashFlowLargeTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _largeTypeService.CreateCashFlowLargeTypeAsync(dto);
            return result;
        }, "新增現金流量大分類失敗");
    }

    /// <summary>
    /// 修改現金流量大分類
    /// </summary>
    [HttpPut("large-types/{cashLTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLargeType(
        string cashLTypeId,
        [FromBody] UpdateCashFlowLargeTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _largeTypeService.UpdateCashFlowLargeTypeAsync(cashLTypeId, dto);
            return new object();
        }, $"修改現金流量大分類失敗: {cashLTypeId}");
    }

    /// <summary>
    /// 刪除現金流量大分類
    /// </summary>
    [HttpDelete("large-types/{cashLTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLargeType(string cashLTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            await _largeTypeService.DeleteCashFlowLargeTypeAsync(cashLTypeId);
            return new object();
        }, $"刪除現金流量大分類失敗: {cashLTypeId}");
    }

    /// <summary>
    /// 檢查大分類代號是否存在
    /// </summary>
    [HttpGet("large-types/{cashLTypeId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckLargeTypeExists(string cashLTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _largeTypeService.ExistsAsync(cashLTypeId);
            return result;
        }, $"檢查大分類代號是否存在失敗: {cashLTypeId}");
    }

    #endregion

    #region SYST132 - 現金流量中分類設定

    /// <summary>
    /// 查詢現金流量中分類列表
    /// </summary>
    [HttpGet("medium-types")]
    public async Task<ActionResult<ApiResponse<PagedResult<CashFlowMediumTypeDto>>>> GetMediumTypes(
        [FromQuery] CashFlowMediumTypeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _mediumTypeService.GetCashFlowMediumTypesAsync(query);
            return result;
        }, "查詢現金流量中分類列表失敗");
    }

    /// <summary>
    /// 查詢單筆現金流量中分類
    /// </summary>
    [HttpGet("medium-types/{cashLTypeId}/{cashMTypeId}")]
    public async Task<ActionResult<ApiResponse<CashFlowMediumTypeDto>>> GetMediumType(
        string cashLTypeId, string cashMTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _mediumTypeService.GetCashFlowMediumTypeByIdAsync(cashLTypeId, cashMTypeId);
            return result;
        }, $"查詢現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}");
    }

    /// <summary>
    /// 新增現金流量中分類
    /// </summary>
    [HttpPost("medium-types")]
    public async Task<ActionResult<ApiResponse<string>>> CreateMediumType(
        [FromBody] CreateCashFlowMediumTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _mediumTypeService.CreateCashFlowMediumTypeAsync(dto);
            return result;
        }, "新增現金流量中分類失敗");
    }

    /// <summary>
    /// 修改現金流量中分類
    /// </summary>
    [HttpPut("medium-types/{cashLTypeId}/{cashMTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateMediumType(
        string cashLTypeId, string cashMTypeId,
        [FromBody] UpdateCashFlowMediumTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _mediumTypeService.UpdateCashFlowMediumTypeAsync(cashLTypeId, cashMTypeId, dto);
            return new object();
        }, $"修改現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}");
    }

    /// <summary>
    /// 刪除現金流量中分類
    /// </summary>
    [HttpDelete("medium-types/{cashLTypeId}/{cashMTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteMediumType(
        string cashLTypeId, string cashMTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            await _mediumTypeService.DeleteCashFlowMediumTypeAsync(cashLTypeId, cashMTypeId);
            return new object();
        }, $"刪除現金流量中分類失敗: {cashLTypeId}/{cashMTypeId}");
    }

    /// <summary>
    /// 檢查中分類代號是否存在
    /// </summary>
    [HttpGet("medium-types/{cashLTypeId}/{cashMTypeId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckMediumTypeExists(
        string cashLTypeId, string cashMTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _mediumTypeService.ExistsAsync(cashLTypeId, cashMTypeId);
            return result;
        }, $"檢查中分類代號是否存在失敗: {cashLTypeId}/{cashMTypeId}");
    }

    #endregion

    #region SYST133 - 現金流量科目設定

    /// <summary>
    /// 查詢現金流量科目設定列表
    /// </summary>
    [HttpGet("subject-types")]
    public async Task<ActionResult<ApiResponse<PagedResult<CashFlowSubjectTypeDto>>>> GetSubjectTypes(
        [FromQuery] CashFlowSubjectTypeQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subjectTypeService.GetCashFlowSubjectTypesAsync(query);
            return result;
        }, "查詢現金流量科目設定列表失敗");
    }

    /// <summary>
    /// 查詢單筆現金流量科目設定
    /// </summary>
    [HttpGet("subject-types/{cashMTypeId}/{cashSTypeId}")]
    public async Task<ActionResult<ApiResponse<CashFlowSubjectTypeDto>>> GetSubjectType(
        string cashMTypeId, string cashSTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subjectTypeService.GetCashFlowSubjectTypeByIdAsync(cashMTypeId, cashSTypeId);
            return result;
        }, $"查詢現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}");
    }

    /// <summary>
    /// 新增現金流量科目設定
    /// </summary>
    [HttpPost("subject-types")]
    public async Task<ActionResult<ApiResponse<string>>> CreateSubjectType(
        [FromBody] CreateCashFlowSubjectTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subjectTypeService.CreateCashFlowSubjectTypeAsync(dto);
            return result;
        }, "新增現金流量科目設定失敗");
    }

    /// <summary>
    /// 修改現金流量科目設定
    /// </summary>
    [HttpPut("subject-types/{cashMTypeId}/{cashSTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSubjectType(
        string cashMTypeId, string cashSTypeId,
        [FromBody] UpdateCashFlowSubjectTypeDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _subjectTypeService.UpdateCashFlowSubjectTypeAsync(cashMTypeId, cashSTypeId, dto);
            return new object();
        }, $"修改現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}");
    }

    /// <summary>
    /// 刪除現金流量科目設定
    /// </summary>
    [HttpDelete("subject-types/{cashMTypeId}/{cashSTypeId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSubjectType(
        string cashMTypeId, string cashSTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            await _subjectTypeService.DeleteCashFlowSubjectTypeAsync(cashMTypeId, cashSTypeId);
            return new object();
        }, $"刪除現金流量科目設定失敗: {cashMTypeId}/{cashSTypeId}");
    }

    /// <summary>
    /// 檢查科目設定是否存在
    /// </summary>
    [HttpGet("subject-types/{cashMTypeId}/{cashSTypeId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckSubjectTypeExists(
        string cashMTypeId, string cashSTypeId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subjectTypeService.ExistsAsync(cashMTypeId, cashSTypeId);
            return result;
        }, $"檢查科目設定是否存在失敗: {cashMTypeId}/{cashSTypeId}");
    }

    #endregion

    #region SYST134 - 現金流量小計設定

    /// <summary>
    /// 查詢現金流量小計設定列表
    /// </summary>
    [HttpGet("sub-totals")]
    public async Task<ActionResult<ApiResponse<PagedResult<CashFlowSubTotalDto>>>> GetSubTotals(
        [FromQuery] CashFlowSubTotalQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subTotalService.GetCashFlowSubTotalsAsync(query);
            return result;
        }, "查詢現金流量小計設定列表失敗");
    }

    /// <summary>
    /// 查詢單筆現金流量小計設定
    /// </summary>
    [HttpGet("sub-totals/{cashLTypeId}/{cashSubId}")]
    public async Task<ActionResult<ApiResponse<CashFlowSubTotalDto>>> GetSubTotal(
        string cashLTypeId, string cashSubId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subTotalService.GetCashFlowSubTotalByIdAsync(cashLTypeId, cashSubId);
            return result;
        }, $"查詢現金流量小計設定失敗: {cashLTypeId}/{cashSubId}");
    }

    /// <summary>
    /// 新增現金流量小計設定
    /// </summary>
    [HttpPost("sub-totals")]
    public async Task<ActionResult<ApiResponse<string>>> CreateSubTotal(
        [FromBody] CreateCashFlowSubTotalDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subTotalService.CreateCashFlowSubTotalAsync(dto);
            return result;
        }, "新增現金流量小計設定失敗");
    }

    /// <summary>
    /// 修改現金流量小計設定
    /// </summary>
    [HttpPut("sub-totals/{cashLTypeId}/{cashSubId}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSubTotal(
        string cashLTypeId, string cashSubId,
        [FromBody] UpdateCashFlowSubTotalDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _subTotalService.UpdateCashFlowSubTotalAsync(cashLTypeId, cashSubId, dto);
            return new object();
        }, $"修改現金流量小計設定失敗: {cashLTypeId}/{cashSubId}");
    }

    /// <summary>
    /// 刪除現金流量小計設定
    /// </summary>
    [HttpDelete("sub-totals/{cashLTypeId}/{cashSubId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteSubTotal(
        string cashLTypeId, string cashSubId)
    {
        return await ExecuteAsync(async () =>
        {
            await _subTotalService.DeleteCashFlowSubTotalAsync(cashLTypeId, cashSubId);
            return new object();
        }, $"刪除現金流量小計設定失敗: {cashLTypeId}/{cashSubId}");
    }

    /// <summary>
    /// 檢查小計設定是否存在
    /// </summary>
    [HttpGet("sub-totals/{cashLTypeId}/{cashSubId}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> CheckSubTotalExists(
        string cashLTypeId, string cashSubId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _subTotalService.ExistsAsync(cashLTypeId, cashSubId);
            return result;
        }, $"檢查小計設定是否存在失敗: {cashLTypeId}/{cashSubId}");
    }

    #endregion
}

