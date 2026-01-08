using ErpCore.Application.DTOs.OtherModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.OtherModule;

/// <summary>
/// Lab測試服務介面
/// 提供測試和開發相關功能
/// </summary>
public interface ILabTestService
{
    /// <summary>
    /// 資料庫連線測試
    /// </summary>
    Task<ConnectionTestResponseDto> TestConnectionAsync();

    /// <summary>
    /// 執行測試
    /// </summary>
    Task<ExecuteTestResponseDto> ExecuteTestAsync(ExecuteTestRequestDto request);

    /// <summary>
    /// 查詢測試結果列表
    /// </summary>
    Task<PagedResult<TestResultDto>> GetTestResultsAsync(TestResultQueryDto query);

    /// <summary>
    /// 根據測試ID取得測試結果
    /// </summary>
    Task<TestResultDto?> GetTestResultByIdAsync(long testId);

    /// <summary>
    /// 刪除測試結果
    /// </summary>
    Task DeleteTestResultAsync(long testId);
}

