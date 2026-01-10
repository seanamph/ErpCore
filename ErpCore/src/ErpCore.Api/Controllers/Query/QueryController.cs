using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.Query;

/// <summary>
/// 查詢功能維護控制器 (SYSQ000)
/// 提供查詢功能資料的新增、修改、刪除、查詢功能
/// </summary>
[Route("api/v1/query-functions")]
public class QueryController : BaseController
{
    private readonly IQueryFunctionService _queryFunctionService;

    public QueryController(
        IQueryFunctionService queryFunctionService,
        ILoggerService logger) : base(logger)
    {
        _queryFunctionService = queryFunctionService;
    }

    /// <summary>
    /// 查詢功能列表
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<QueryFunctionDto>>>> GetQueryFunctions([FromQuery] QueryFunctionQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryFunctionService.GetQueryFunctionsAsync(query);
            return result;
        }, "查詢功能列表查詢失敗");
    }

    /// <summary>
    /// 查詢單筆功能
    /// </summary>
    [HttpGet("{tKey}")]
    public async Task<ActionResult<ApiResponse<QueryFunctionDto>>> GetQueryFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryFunctionService.GetQueryFunctionByIdAsync(tKey);
            return result;
        }, $"查詢功能查詢失敗: {tKey}");
    }

    /// <summary>
    /// 根據查詢代碼查詢
    /// </summary>
    [HttpGet("by-id/{queryId}")]
    public async Task<ActionResult<ApiResponse<QueryFunctionDto>>> GetQueryFunctionById(string queryId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryFunctionService.GetQueryFunctionByQueryIdAsync(queryId);
            return result;
        }, $"查詢功能查詢失敗: {queryId}");
    }

    /// <summary>
    /// 新增查詢功能
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<long>>> CreateQueryFunction([FromBody] CreateQueryFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryFunctionService.CreateQueryFunctionAsync(dto);
            return result;
        }, "新增查詢功能失敗");
    }

    /// <summary>
    /// 修改查詢功能
    /// </summary>
    [HttpPut("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateQueryFunction(long tKey, [FromBody] UpdateQueryFunctionDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _queryFunctionService.UpdateQueryFunctionAsync(tKey, dto);
        }, $"修改查詢功能失敗: {tKey}");
    }

    /// <summary>
    /// 刪除查詢功能
    /// </summary>
    [HttpDelete("{tKey}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteQueryFunction(long tKey)
    {
        return await ExecuteAsync(async () =>
        {
            await _queryFunctionService.DeleteQueryFunctionAsync(tKey);
        }, $"刪除查詢功能失敗: {tKey}");
    }

    /// <summary>
    /// 執行查詢功能
    /// </summary>
    [HttpPost("{tKey}/execute")]
    public async Task<ActionResult<ApiResponse<QueryResultDto>>> ExecuteQueryFunction(long tKey, [FromBody] ExecuteQueryDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _queryFunctionService.ExecuteQueryFunctionAsync(tKey, dto);
            return result;
        }, $"執行查詢功能失敗: {tKey}");
    }

    /// <summary>
    /// 啟用/停用查詢功能
    /// </summary>
    [HttpPut("{tKey}/status")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateStatus(long tKey, [FromBody] UpdateStatusDto dto)
    {
        return await ExecuteAsync(async () =>
        {
            await _queryFunctionService.UpdateStatusAsync(tKey, dto.Status);
        }, $"更新查詢功能狀態失敗: {tKey}");
    }
}

