using Microsoft.AspNetCore.Mvc;
using ErpCore.Api.Controllers.Base;
using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Application.Services.OtherModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Api.Controllers.OtherModule;

/// <summary>
/// Lab測試控制器
/// 提供測試和開發相關功能
/// </summary>
[Route("api/v1/other-module/lab")]
public class LabTestController : BaseController
{
    private readonly ILabTestService _service;

    public LabTestController(
        ILabTestService service,
        ILoggerService logger) : base(logger)
    {
        _service = service;
    }

    /// <summary>
    /// 資料庫連線測試
    /// </summary>
    [HttpPost("test/connection")]
    public async Task<ActionResult<ApiResponse<ConnectionTestResponseDto>>> TestConnection()
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.TestConnectionAsync();
            return result;
        }, "資料庫連線測試失敗");
    }

    /// <summary>
    /// 執行測試
    /// </summary>
    [HttpPost("test/execute")]
    public async Task<ActionResult<ApiResponse<ExecuteTestResponseDto>>> ExecuteTest([FromBody] ExecuteTestRequestDto request)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.ExecuteTestAsync(request);
            return result;
        }, "執行測試失敗");
    }

    /// <summary>
    /// 查詢測試結果列表
    /// </summary>
    [HttpGet("test-results")]
    public async Task<ActionResult<ApiResponse<PagedResult<TestResultDto>>>> GetTestResults([FromQuery] TestResultQueryDto query)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTestResultsAsync(query);
            return result;
        }, "查詢測試結果列表失敗");
    }

    /// <summary>
    /// 根據測試ID取得測試結果
    /// </summary>
    [HttpGet("test-results/{testId}")]
    public async Task<ActionResult<ApiResponse<TestResultDto>>> GetTestResultById(long testId)
    {
        return await ExecuteAsync(async () =>
        {
            var result = await _service.GetTestResultByIdAsync(testId);
            if (result == null)
            {
                throw new InvalidOperationException($"測試結果不存在: {testId}");
            }
            return result;
        }, "取得測試結果失敗");
    }

    /// <summary>
    /// 刪除測試結果
    /// </summary>
    [HttpDelete("test-results/{testId}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteTestResult(long testId)
    {
        return await ExecuteAsync(async () =>
        {
            await _service.DeleteTestResultAsync(testId);
            return (object)null!;
        }, "刪除測試結果失敗");
    }
}

