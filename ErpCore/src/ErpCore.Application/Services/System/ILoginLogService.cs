using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 使用者異常登入記錄服務介面 (SYS0760)
/// </summary>
public interface ILoginLogService
{
    /// <summary>
    /// 查詢異常登入記錄列表
    /// </summary>
    Task<PagedResult<LoginLogDto>> GetLoginLogsAsync(LoginLogQueryDto query);

    /// <summary>
    /// 取得異常事件代碼選項
    /// </summary>
    Task<List<EventTypeDto>> GetEventTypesAsync();

    /// <summary>
    /// 刪除異常登入記錄
    /// </summary>
    Task<int> DeleteLoginLogsAsync(List<long> tKeys, string currentUserId);

    /// <summary>
    /// 產生異常登入報表
    /// </summary>
    Task<byte[]> GenerateReportAsync(LoginLogReportDto reportDto, string format);
}
