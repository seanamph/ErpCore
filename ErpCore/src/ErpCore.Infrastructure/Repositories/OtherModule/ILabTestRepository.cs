using ErpCore.Domain.Entities.OtherModule;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.OtherModule;

/// <summary>
/// Lab測試 Repository 介面
/// </summary>
public interface ILabTestRepository
{
    /// <summary>
    /// 根據測試ID查詢測試結果
    /// </summary>
    Task<TestResult?> GetByIdAsync(long testId);

    /// <summary>
    /// 查詢測試結果列表
    /// </summary>
    Task<PagedResult<TestResult>> QueryAsync(TestResultQuery query);

    /// <summary>
    /// 新增測試結果
    /// </summary>
    Task<TestResult> CreateAsync(TestResult testResult);

    /// <summary>
    /// 修改測試結果
    /// </summary>
    Task<TestResult> UpdateAsync(TestResult testResult);

    /// <summary>
    /// 刪除測試結果
    /// </summary>
    Task DeleteAsync(long testId);
}

/// <summary>
/// 測試結果查詢條件
/// </summary>
public class TestResultQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TestType { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

