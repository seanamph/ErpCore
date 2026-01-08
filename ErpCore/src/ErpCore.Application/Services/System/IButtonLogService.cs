using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 按鈕操作記錄服務介面 (SYS0790)
/// </summary>
public interface IButtonLogService
{
    /// <summary>
    /// 查詢按鈕操作記錄列表
    /// </summary>
    Task<PagedResult<ButtonLogDto>> GetButtonLogsAsync(ButtonLogQueryDto query);

    /// <summary>
    /// 新增按鈕操作記錄
    /// </summary>
    Task<long> CreateButtonLogAsync(CreateButtonLogDto dto);

    /// <summary>
    /// 匯出按鈕操作記錄報表
    /// </summary>
    Task<byte[]> ExportButtonLogReportAsync(ButtonLogQueryDto query, string format);
}

