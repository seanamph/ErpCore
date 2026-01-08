using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 潛客主檔 Repository 介面 (SYSC165)
/// </summary>
public interface IProspectMasterRepository
{
    /// <summary>
    /// 根據主檔代碼查詢潛客主檔
    /// </summary>
    Task<ProspectMaster?> GetByIdAsync(string masterId);

    /// <summary>
    /// 查詢潛客主檔列表（分頁）
    /// </summary>
    Task<PagedResult<ProspectMaster>> QueryAsync(ProspectMasterQuery query);

    /// <summary>
    /// 新增潛客主檔
    /// </summary>
    Task<ProspectMaster> CreateAsync(ProspectMaster prospectMaster);

    /// <summary>
    /// 修改潛客主檔
    /// </summary>
    Task<ProspectMaster> UpdateAsync(ProspectMaster prospectMaster);

    /// <summary>
    /// 刪除潛客主檔
    /// </summary>
    Task DeleteAsync(string masterId);

    /// <summary>
    /// 檢查主檔代碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string masterId);

    /// <summary>
    /// 檢查是否有關聯的潛客資料
    /// </summary>
    Task<bool> HasRelatedProspectsAsync(string masterId);
}

/// <summary>
/// 潛客主檔查詢條件
/// </summary>
public class ProspectMasterQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? MasterId { get; set; }
    public string? MasterName { get; set; }
    public string? MasterType { get; set; }
    public string? Category { get; set; }
    public string? Industry { get; set; }
    public string? BusinessType { get; set; }
    public string? Status { get; set; }
    public string? Source { get; set; }
}

