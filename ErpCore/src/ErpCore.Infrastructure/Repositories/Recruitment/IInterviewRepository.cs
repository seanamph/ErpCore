using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 訪談 Repository 介面 (SYSC222)
/// </summary>
public interface IInterviewRepository
{
    /// <summary>
    /// 根據訪談ID查詢訪談
    /// </summary>
    Task<Interview?> GetByIdAsync(long interviewId);

    /// <summary>
    /// 查詢訪談列表（分頁）
    /// </summary>
    Task<PagedResult<Interview>> QueryAsync(InterviewQuery query);

    /// <summary>
    /// 根據潛客代碼查詢訪談列表
    /// </summary>
    Task<PagedResult<Interview>> QueryByProspectIdAsync(string prospectId, InterviewQuery query);

    /// <summary>
    /// 新增訪談
    /// </summary>
    Task<Interview> CreateAsync(Interview interview);

    /// <summary>
    /// 修改訪談
    /// </summary>
    Task<Interview> UpdateAsync(Interview interview);

    /// <summary>
    /// 刪除訪談
    /// </summary>
    Task DeleteAsync(long interviewId);

    /// <summary>
    /// 檢查訪談是否存在
    /// </summary>
    Task<bool> ExistsAsync(long interviewId);
}

/// <summary>
/// 訪談查詢條件
/// </summary>
public class InterviewQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ProspectId { get; set; }
    public DateTime? InterviewDateFrom { get; set; }
    public DateTime? InterviewDateTo { get; set; }
    public string? InterviewResult { get; set; }
    public string? Status { get; set; }
    public string? Interviewer { get; set; }
}

