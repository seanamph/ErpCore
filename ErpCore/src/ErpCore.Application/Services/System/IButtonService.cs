using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 系統功能按鈕服務介面 (SYS0440)
/// </summary>
public interface IButtonService
{
    /// <summary>
    /// 查詢按鈕列表
    /// </summary>
    Task<PagedResult<ButtonDto>> GetButtonsAsync(ButtonQueryDto query);

    /// <summary>
    /// 查詢單筆按鈕
    /// </summary>
    Task<ButtonDto> GetButtonByIdAsync(long tKey);

    /// <summary>
    /// 新增按鈕
    /// </summary>
    Task<long> CreateButtonAsync(CreateButtonDto dto);

    /// <summary>
    /// 修改按鈕
    /// </summary>
    Task UpdateButtonAsync(long tKey, UpdateButtonDto dto);

    /// <summary>
    /// 刪除按鈕
    /// </summary>
    Task DeleteButtonAsync(long tKey);

    /// <summary>
    /// 批次刪除按鈕
    /// </summary>
    Task BatchDeleteButtonsAsync(BatchDeleteButtonDto dto);
}
