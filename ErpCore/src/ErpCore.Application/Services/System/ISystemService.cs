using ErpCore.Application.DTOs.System;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 主系統項目服務介面 (SYS0410)
/// </summary>
public interface ISystemService
{
    /// <summary>
    /// 查詢主系統列表
    /// </summary>
    Task<PagedResult<SystemDto>> GetSystemsAsync(SystemQueryDto query);

    /// <summary>
    /// 查詢單筆主系統
    /// </summary>
    Task<SystemDto> GetSystemAsync(string systemId);

    /// <summary>
    /// 新增主系統
    /// </summary>
    Task<string> CreateSystemAsync(CreateSystemDto dto);

    /// <summary>
    /// 修改主系統
    /// </summary>
    Task UpdateSystemAsync(string systemId, UpdateSystemDto dto);

    /// <summary>
    /// 刪除主系統
    /// </summary>
    Task DeleteSystemAsync(string systemId);

    /// <summary>
    /// 批次刪除主系統
    /// </summary>
    Task DeleteSystemsBatchAsync(BatchDeleteSystemsDto dto);

    /// <summary>
    /// 取得系統型態選項
    /// </summary>
    Task<List<SystemTypeOptionDto>> GetSystemTypesAsync();
}
