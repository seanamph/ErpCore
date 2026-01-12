using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.AnalysisReport;
using ErpCore.Application.Services.AnalysisReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.AnalysisReport;

/// <summary>
/// 單位領用申請單控制器 (SYSA210)
/// </summary>
[Route("api/v1/material-applies")]
public class MaterialApplyController : BaseController
{
    private readonly IMaterialApplyService _service;
    private readonly IUserContext _userContext;

    public MaterialApplyController(
        IMaterialApplyService service,
        IUserContext userContext,
        ILoggerService logger) : base(logger)
    {
        _service = service;
        _userContext = userContext;
    }

    /// <summary>
    /// 取得當前使用者 ID
    /// </summary>
    private string GetCurrentUserId() => _userContext.GetUserId() ?? "SYSTEM";

    /// <summary>
    /// 查詢領用申請列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<MaterialApplyDto>>>> GetList(
        [FromQuery] MaterialApplyQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetListAsync(query);
            return result;
        }, "查詢單位領用申請單列表失敗");
    }

    /// <summary>
    /// 查詢單筆領用申請（含明細）
    /// </summary>
    [HttpGet("{applyId}")]
    public async Task<ActionResult<ApiResponse<MaterialApplyDetailDto>>> GetByApplyId(string applyId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetByApplyIdAsync(applyId);
            return result;
        }, "查詢單位領用申請單詳細資料失敗");
    }

    /// <summary>
    /// 新增領用申請
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<MaterialApplyDetailDto>>> Create(
        [FromBody] CreateMaterialApplyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await _service.CreateAsync(dto, userId);
            return result;
        }, "新增單位領用申請單失敗");
    }

    /// <summary>
    /// 修改領用申請
    /// </summary>
    [HttpPut("{applyId}")]
    public async Task<ActionResult<ApiResponse<MaterialApplyDetailDto>>> Update(
        string applyId,
        [FromBody] UpdateMaterialApplyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await _service.UpdateAsync(applyId, dto, userId);
            return result;
        }, "修改單位領用申請單失敗");
    }

    /// <summary>
    /// 刪除領用申請
    /// </summary>
    [HttpDelete("{applyId}")]
    public async Task<ActionResult<ApiResponse>> Delete(string applyId)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            await _service.DeleteAsync(applyId, userId);
            return new { success = true };
        }, "刪除單位領用申請單失敗");
    }

    /// <summary>
    /// 審核領用申請
    /// </summary>
    [HttpPost("{applyId}/approve")]
    public async Task<ActionResult<ApiResponse<MaterialApplyDetailDto>>> Approve(
        string applyId,
        [FromBody] ApproveMaterialApplyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await _service.ApproveAsync(applyId, dto, userId);
            return result;
        }, "審核單位領用申請單失敗");
    }

    /// <summary>
    /// 發料作業
    /// </summary>
    [HttpPost("{applyId}/issue")]
    public async Task<ActionResult<ApiResponse<MaterialApplyDetailDto>>> Issue(
        string applyId,
        [FromBody] IssueMaterialApplyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await _service.IssueAsync(applyId, dto, userId);
            return result;
        }, "發料作業失敗");
    }

    /// <summary>
    /// 批次新增領用申請
    /// </summary>
    [HttpPost("batch")]
    public async Task<ActionResult<ApiResponse<List<MaterialApplyDetailDto>>>> BatchCreate(
        [FromBody] BatchCreateMaterialApplyDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var userId = GetCurrentUserId();
            var result = await _service.BatchCreateAsync(dto, userId);
            return result;
        }, "批次新增單位領用申請單失敗");
    }

    /// <summary>
    /// 產生領用單號
    /// </summary>
    [HttpGet("generate-apply-id")]
    public async Task<ActionResult<ApiResponse<string>>> GenerateApplyId()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GenerateApplyIdAsync();
            return result;
        }, "產生領用單號失敗");
    }
}
