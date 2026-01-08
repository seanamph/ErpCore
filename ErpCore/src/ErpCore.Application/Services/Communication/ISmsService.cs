using ErpCore.Application.DTOs.Communication;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Communication;

/// <summary>
/// 簡訊服務介面
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// 發送簡訊
    /// </summary>
    Task<SendSmsResponseDto> SendSmsAsync(SendSmsRequestDto request);

    /// <summary>
    /// 查詢簡訊記錄列表
    /// </summary>
    Task<PagedResult<SmsLogDto>> GetSmsLogsAsync(SmsLogQueryDto query);

    /// <summary>
    /// 根據ID查詢簡訊記錄
    /// </summary>
    Task<SmsLogDto?> GetSmsLogByIdAsync(long id);
}

