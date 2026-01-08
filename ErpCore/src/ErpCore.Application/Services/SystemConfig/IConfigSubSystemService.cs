using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SystemConfig;

/// <summary>
/// 子系統項目服務介面
/// </summary>
public interface IConfigSubSystemService
{
    /// <summary>
    /// 查詢子系統列表
    /// </summary>
    Task<PagedResult<ConfigSubSystemDto>> GetConfigSubSystemsAsync(ConfigSubSystemQueryDto query);

    /// <summary>
    /// 查詢單筆子系統
    /// </summary>
    Task<ConfigSubSystemDto> GetConfigSubSystemAsync(string subSystemId);

    /// <summary>
    /// 新增子系統
    /// </summary>
    Task<string> CreateConfigSubSystemAsync(CreateConfigSubSystemDto dto);

    /// <summary>
    /// 修改子系統
    /// </summary>
    Task UpdateConfigSubSystemAsync(string subSystemId, UpdateConfigSubSystemDto dto);

    /// <summary>
    /// 刪除子系統
    /// </summary>
    Task DeleteConfigSubSystemAsync(string subSystemId);

    /// <summary>
    /// 批次刪除子系統
    /// </summary>
    Task DeleteConfigSubSystemsBatchAsync(BatchDeleteConfigSubSystemDto dto);
}

