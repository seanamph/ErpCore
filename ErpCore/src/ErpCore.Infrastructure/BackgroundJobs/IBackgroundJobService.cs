namespace ErpCore.Infrastructure.BackgroundJobs;

/// <summary>
/// 背景工作服務介面
/// 提供背景工作的排程、執行等功能
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>
    /// 排程一次性背景工作
    /// </summary>
    /// <param name="jobId">工作ID</param>
    /// <param name="action">要執行的動作</param>
    /// <param name="delay">延遲時間</param>
    /// <returns>是否排程成功</returns>
    Task<bool> ScheduleAsync(string jobId, Func<Task> action, TimeSpan delay);

    /// <summary>
    /// 排程重複執行的背景工作
    /// </summary>
    /// <param name="jobId">工作ID</param>
    /// <param name="action">要執行的動作</param>
    /// <param name="cronExpression">Cron 表達式</param>
    /// <returns>是否排程成功</returns>
    Task<bool> ScheduleRecurringAsync(string jobId, Func<Task> action, string cronExpression);

    /// <summary>
    /// 取消背景工作
    /// </summary>
    /// <param name="jobId">工作ID</param>
    /// <returns>是否取消成功</returns>
    Task<bool> CancelAsync(string jobId);

    /// <summary>
    /// 檢查背景工作是否存在
    /// </summary>
    /// <param name="jobId">工作ID</param>
    /// <returns>是否存在</returns>
    Task<bool> ExistsAsync(string jobId);
}

