using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SystemConfig;

/// <summary>
/// 系統功能按鈕服務介面
/// </summary>
public interface IConfigButtonService
{
    /// <summary>
    /// 查詢按鈕列表
    /// </summary>
    Task<PagedResult<ConfigButtonDto>> GetConfigButtonsAsync(ConfigButtonQueryDto query);

    /// <summary>
    /// 查詢單筆按鈕
    /// </summary>
    Task<ConfigButtonDto> GetConfigButtonAsync(string buttonId);

    /// <summary>
    /// 新增按鈕
    /// </summary>
    Task<string> CreateConfigButtonAsync(CreateConfigButtonDto dto);

    /// <summary>
    /// 修改按鈕
    /// </summary>
    Task UpdateConfigButtonAsync(string buttonId, UpdateConfigButtonDto dto);

    /// <summary>
    /// 刪除按鈕
    /// </summary>
    Task DeleteConfigButtonAsync(string buttonId);

    /// <summary>
    /// 批次刪除按鈕
    /// </summary>
    Task DeleteConfigButtonsBatchAsync(BatchDeleteConfigButtonDto dto);
}

