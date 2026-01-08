using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 潛客主檔服務介面 (SYSC165)
/// </summary>
public interface IProspectMasterService
{
    /// <summary>
    /// 查詢潛客主檔列表
    /// </summary>
    Task<PagedResult<ProspectMasterDto>> GetProspectMastersAsync(ProspectMasterQueryDto query);

    /// <summary>
    /// 查詢單筆潛客主檔
    /// </summary>
    Task<ProspectMasterDto> GetProspectMasterByIdAsync(string masterId);

    /// <summary>
    /// 新增潛客主檔
    /// </summary>
    Task<string> CreateProspectMasterAsync(CreateProspectMasterDto dto);

    /// <summary>
    /// 修改潛客主檔
    /// </summary>
    Task UpdateProspectMasterAsync(string masterId, UpdateProspectMasterDto dto);

    /// <summary>
    /// 刪除潛客主檔
    /// </summary>
    Task DeleteProspectMasterAsync(string masterId);

    /// <summary>
    /// 批次刪除潛客主檔
    /// </summary>
    Task BatchDeleteProspectMastersAsync(BatchDeleteProspectMasterDto dto);

    /// <summary>
    /// 更新潛客主檔狀態
    /// </summary>
    Task UpdateProspectMasterStatusAsync(string masterId, UpdateProspectMasterStatusDto dto);
}

