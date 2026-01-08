using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.SystemConfig;

/// <summary>
/// 系統作業服務介面
/// </summary>
public interface IConfigProgramService
{
    /// <summary>
    /// 查詢作業列表
    /// </summary>
    Task<PagedResult<ConfigProgramDto>> GetConfigProgramsAsync(ConfigProgramQueryDto query);

    /// <summary>
    /// 查詢單筆作業
    /// </summary>
    Task<ConfigProgramDto> GetConfigProgramAsync(string programId);

    /// <summary>
    /// 新增作業
    /// </summary>
    Task<string> CreateConfigProgramAsync(CreateConfigProgramDto dto);

    /// <summary>
    /// 修改作業
    /// </summary>
    Task UpdateConfigProgramAsync(string programId, UpdateConfigProgramDto dto);

    /// <summary>
    /// 刪除作業
    /// </summary>
    Task DeleteConfigProgramAsync(string programId);

    /// <summary>
    /// 批次刪除作業
    /// </summary>
    Task DeleteConfigProgramsBatchAsync(BatchDeleteConfigProgramDto dto);
}

