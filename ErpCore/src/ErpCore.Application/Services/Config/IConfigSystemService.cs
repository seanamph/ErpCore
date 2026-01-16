using ErpCore.Application.DTOs.Config;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Config;

/// <summary>
/// 主系統項目服務介面 (CFG0410)
/// </summary>
public interface IConfigSystemService
{
    /// <summary>
    /// 查詢主系統列表
    /// </summary>
    Task<PagedResult<ConfigSystemDto>> GetConfigSystemsAsync(ConfigSystemQueryDto query);

    /// <summary>
    /// 查詢單筆主系統
    /// </summary>
    Task<ConfigSystemDto> GetConfigSystemByIdAsync(string systemId);

    /// <summary>
    /// 新增主系統
    /// </summary>
    Task<string> CreateConfigSystemAsync(CreateConfigSystemDto dto);

    /// <summary>
    /// 修改主系統
    /// </summary>
    Task UpdateConfigSystemAsync(string systemId, UpdateConfigSystemDto dto);

    /// <summary>
    /// 刪除主系統
    /// </summary>
    Task DeleteConfigSystemAsync(string systemId);

    /// <summary>
    /// 批次刪除主系統
    /// </summary>
    Task DeleteConfigSystemsBatchAsync(BatchDeleteConfigSystemDto dto);

    /// <summary>
    /// 更新主系統狀態
    /// </summary>
    Task UpdateStatusAsync(string systemId, UpdateConfigSystemStatusDto dto);
}
