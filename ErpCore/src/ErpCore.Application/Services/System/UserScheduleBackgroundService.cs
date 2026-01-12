using ErpCore.Application.Services.System;
using ErpCore.Shared.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者排程背景服務 (SYS0116)
/// 定期檢查並執行待執行的排程
/// </summary>
public class UserScheduleBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerService _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public UserScheduleBackgroundService(
        IServiceProvider serviceProvider,
        ILoggerService logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInfo("使用者排程背景服務已啟動");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingSchedulesAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("執行排程服務時發生錯誤", ex);
            }

            // 等待指定時間後再次檢查
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInfo("使用者排程背景服務已停止");
    }

    private async Task ProcessPendingSchedulesAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var scheduleService = scope.ServiceProvider.GetRequiredService<IUserScheduleService>();

        try
        {
            // 查詢待執行的排程（執行時間已到或已過）
            var executeTime = DateTime.Now;
            var pendingSchedules = await scheduleService.GetPendingSchedulesAsync(executeTime);

            if (pendingSchedules.Any())
            {
                _logger.LogInfo($"找到 {pendingSchedules.Count} 筆待執行的排程");

                foreach (var schedule in pendingSchedules)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }

                    try
                    {
                        _logger.LogInfo($"開始執行排程: {schedule.ScheduleId}, 使用者: {schedule.UserId}, 類型: {schedule.ScheduleType}");
                        await scheduleService.ExecuteScheduleAsync(schedule.ScheduleId);
                        _logger.LogInfo($"排程執行完成: {schedule.ScheduleId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"執行排程失敗: {schedule.ScheduleId}", ex);
                        // 繼續執行下一個排程，不中斷整個流程
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("處理待執行排程時發生錯誤", ex);
        }
    }
}
