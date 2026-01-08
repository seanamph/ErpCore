using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Lease;

/// <summary>
/// 租賃資料維護控制器 (SYSE110-SYSE140)
/// </summary>
[ApiController]
[Route("api/v1/lease-syse/data")]
public class LeaseSYSEDataController : BaseController
{
    private readonly ILeaseTermService _leaseTermService;
    private readonly ILeaseAccountingCategoryService _leaseAccountingCategoryService;

    public LeaseSYSEDataController(
        ILeaseTermService leaseTermService,
        ILeaseAccountingCategoryService leaseAccountingCategoryService,
        ILoggerService logger) : base(logger)
    {
        _leaseTermService = leaseTermService;
        _leaseAccountingCategoryService = leaseAccountingCategoryService;
    }

    #region 租賃條件 (LeaseTerm)

    /// <summary>
    /// 查詢租賃條件列表
    /// </summary>
    [HttpGet("terms")]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseTermDto>>>> GetLeaseTerms(
        [FromQuery] LeaseTermQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseTermService.GetLeaseTermsAsync(query);
            return result;
        }, "查詢租賃條件列表失敗");
    }

    /// <summary>
    /// 查詢單筆租賃條件
    /// </summary>
    [HttpGet("terms/{tKey}")]
    public async Task<ActionResult<ApiResponse<LeaseTermDto>>> GetLeaseTerm(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseTermService.GetLeaseTermByIdAsync(tKey);
            return result;
        }, $"查詢租賃條件失敗: {tKey}");
    }

    /// <summary>
    /// 根據租賃編號和版本查詢租賃條件
    /// </summary>
    [HttpGet("leases/{leaseId}/versions/{version}/terms")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LeaseTermDto>>>> GetLeaseTermsByLeaseIdAndVersion(
        string leaseId, string version)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseTermService.GetLeaseTermsByLeaseIdAndVersionAsync(leaseId, version);
            return result;
        }, $"查詢租賃條件失敗: {leaseId}/{version}");
    }

    /// <summary>
    /// 新增租賃條件
    /// </summary>
    [HttpPost("terms")]
    public async Task<ActionResult<ApiResponse<LeaseTermDto>>> CreateLeaseTerm(
        [FromBody] CreateLeaseTermDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseTermService.CreateLeaseTermAsync(dto);
            return result;
        }, "新增租賃條件失敗");
    }

    /// <summary>
    /// 修改租賃條件
    /// </summary>
    [HttpPut("terms/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseTerm(
        long tKey,
        [FromBody] UpdateLeaseTermDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseTermService.UpdateLeaseTermAsync(tKey, dto);
        }, $"修改租賃條件失敗: {tKey}");
    }

    /// <summary>
    /// 刪除租賃條件
    /// </summary>
    [HttpDelete("terms/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseTerm(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseTermService.DeleteLeaseTermAsync(tKey);
        }, $"刪除租賃條件失敗: {tKey}");
    }

    /// <summary>
    /// 根據租賃編號和版本刪除租賃條件
    /// </summary>
    [HttpDelete("leases/{leaseId}/versions/{version}/terms")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseTermsByLeaseIdAndVersion(
        string leaseId, string version)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseTermService.DeleteLeaseTermsByLeaseIdAndVersionAsync(leaseId, version);
        }, $"刪除租賃條件失敗: {leaseId}/{version}");
    }

    /// <summary>
    /// 檢查租賃條件是否存在
    /// </summary>
    [HttpGet("terms/{tKey}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaseTermExists(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseTermService.ExistsAsync(tKey);
            return result;
        }, $"檢查租賃條件是否存在失敗: {tKey}");
    }

    #endregion

    #region 租賃會計分類 (LeaseAccountingCategory)

    /// <summary>
    /// 查詢租賃會計分類列表
    /// </summary>
    [HttpGet("accounting-categories")]
    public async Task<ActionResult<ApiResponse<PagedResult<LeaseAccountingCategoryDto>>>> GetLeaseAccountingCategories(
        [FromQuery] LeaseAccountingCategoryQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseAccountingCategoryService.GetLeaseAccountingCategoriesAsync(query);
            return result;
        }, "查詢租賃會計分類列表失敗");
    }

    /// <summary>
    /// 查詢單筆租賃會計分類
    /// </summary>
    [HttpGet("accounting-categories/{tKey}")]
    public async Task<ActionResult<ApiResponse<LeaseAccountingCategoryDto>>> GetLeaseAccountingCategory(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseAccountingCategoryService.GetLeaseAccountingCategoryByIdAsync(tKey);
            return result;
        }, $"查詢租賃會計分類失敗: {tKey}");
    }

    /// <summary>
    /// 根據租賃編號和版本查詢租賃會計分類
    /// </summary>
    [HttpGet("leases/{leaseId}/versions/{version}/accounting-categories")]
    public async Task<ActionResult<ApiResponse<IEnumerable<LeaseAccountingCategoryDto>>>> GetLeaseAccountingCategoriesByLeaseIdAndVersion(
        string leaseId, string version)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseAccountingCategoryService.GetLeaseAccountingCategoriesByLeaseIdAndVersionAsync(leaseId, version);
            return result;
        }, $"查詢租賃會計分類失敗: {leaseId}/{version}");
    }

    /// <summary>
    /// 新增租賃會計分類
    /// </summary>
    [HttpPost("accounting-categories")]
    public async Task<ActionResult<ApiResponse<LeaseAccountingCategoryDto>>> CreateLeaseAccountingCategory(
        [FromBody] CreateLeaseAccountingCategoryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseAccountingCategoryService.CreateLeaseAccountingCategoryAsync(dto);
            return result;
        }, "新增租賃會計分類失敗");
    }

    /// <summary>
    /// 修改租賃會計分類
    /// </summary>
    [HttpPut("accounting-categories/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLeaseAccountingCategory(
        long tKey,
        [FromBody] UpdateLeaseAccountingCategoryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseAccountingCategoryService.UpdateLeaseAccountingCategoryAsync(tKey, dto);
        }, $"修改租賃會計分類失敗: {tKey}");
    }

    /// <summary>
    /// 刪除租賃會計分類
    /// </summary>
    [HttpDelete("accounting-categories/{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseAccountingCategory(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseAccountingCategoryService.DeleteLeaseAccountingCategoryAsync(tKey);
        }, $"刪除租賃會計分類失敗: {tKey}");
    }

    /// <summary>
    /// 根據租賃編號和版本刪除租賃會計分類
    /// </summary>
    [HttpDelete("leases/{leaseId}/versions/{version}/accounting-categories")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLeaseAccountingCategoriesByLeaseIdAndVersion(
        string leaseId, string version)
    {
        return await ExecuteAsync(async () =>
        {
            await _leaseAccountingCategoryService.DeleteLeaseAccountingCategoriesByLeaseIdAndVersionAsync(leaseId, version);
        }, $"刪除租賃會計分類失敗: {leaseId}/{version}");
    }

    /// <summary>
    /// 檢查租賃會計分類是否存在
    /// </summary>
    [HttpGet("accounting-categories/{tKey}/exists")]
    public async Task<ActionResult<ApiResponse<bool>>> LeaseAccountingCategoryExists(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _leaseAccountingCategoryService.ExistsAsync(tKey);
            return result;
        }, $"檢查租賃會計分類是否存在失敗: {tKey}");
    }

    #endregion
}

