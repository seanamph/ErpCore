using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 訪談服務介面 (SYSC222)
/// </summary>
public interface IInterviewService
{
    /// <summary>
    /// 查詢訪談列表
    /// </summary>
    Task<PagedResult<InterviewDto>> GetInterviewsAsync(InterviewQueryDto query);

    /// <summary>
    /// 根據潛客查詢訪談列表
    /// </summary>
    Task<PagedResult<InterviewDto>> GetInterviewsByProspectIdAsync(string prospectId, InterviewQueryDto query);

    /// <summary>
    /// 查詢單筆訪談
    /// </summary>
    Task<InterviewDto> GetInterviewByIdAsync(long interviewId);

    /// <summary>
    /// 新增訪談
    /// </summary>
    Task<long> CreateInterviewAsync(CreateInterviewDto dto);

    /// <summary>
    /// 修改訪談
    /// </summary>
    Task UpdateInterviewAsync(long interviewId, UpdateInterviewDto dto);

    /// <summary>
    /// 刪除訪談
    /// </summary>
    Task DeleteInterviewAsync(long interviewId);

    /// <summary>
    /// 批次刪除訪談
    /// </summary>
    Task BatchDeleteInterviewsAsync(BatchDeleteInterviewDto dto);

    /// <summary>
    /// 更新訪談狀態
    /// </summary>
    Task UpdateInterviewStatusAsync(long interviewId, UpdateInterviewStatusDto dto);
}

