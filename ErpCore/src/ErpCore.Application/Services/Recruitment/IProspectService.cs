using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 潛客服務介面 (SYSC180)
/// </summary>
public interface IProspectService
{
    /// <summary>
    /// 查詢潛客列表
    /// </summary>
    Task<PagedResult<ProspectDto>> GetProspectsAsync(ProspectQueryDto query);

    /// <summary>
    /// 查詢單筆潛客
    /// </summary>
    Task<ProspectDto> GetProspectByIdAsync(string prospectId);

    /// <summary>
    /// 新增潛客
    /// </summary>
    Task<string> CreateProspectAsync(CreateProspectDto dto);

    /// <summary>
    /// 修改潛客
    /// </summary>
    Task UpdateProspectAsync(string prospectId, UpdateProspectDto dto);

    /// <summary>
    /// 刪除潛客
    /// </summary>
    Task DeleteProspectAsync(string prospectId);

    /// <summary>
    /// 批次刪除潛客
    /// </summary>
    Task BatchDeleteProspectsAsync(BatchDeleteProspectDto dto);

    /// <summary>
    /// 更新潛客狀態
    /// </summary>
    Task UpdateProspectStatusAsync(string prospectId, UpdateProspectStatusDto dto);
}

