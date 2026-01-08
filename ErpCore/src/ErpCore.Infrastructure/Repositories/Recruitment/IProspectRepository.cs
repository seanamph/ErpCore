using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Recruitment;

/// <summary>
/// 潛客 Repository 介面 (SYSC180)
/// </summary>
public interface IProspectRepository
{
    /// <summary>
    /// 根據潛客代碼查詢潛客
    /// </summary>
    Task<Prospect?> GetByIdAsync(string prospectId);

    /// <summary>
    /// 查詢潛客列表（分頁）
    /// </summary>
    Task<PagedResult<Prospect>> QueryAsync(ProspectQuery query);

    /// <summary>
    /// 新增潛客
    /// </summary>
    Task<Prospect> CreateAsync(Prospect prospect);

    /// <summary>
    /// 修改潛客
    /// </summary>
    Task<Prospect> UpdateAsync(Prospect prospect);

    /// <summary>
    /// 刪除潛客
    /// </summary>
    Task DeleteAsync(string prospectId);

    /// <summary>
    /// 檢查潛客代碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string prospectId);

    /// <summary>
    /// 檢查是否有關聯的訪談記錄
    /// </summary>
    Task<bool> HasRelatedInterviewsAsync(string prospectId);
}

/// <summary>
/// 潛客查詢條件
/// </summary>
public class ProspectQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ProspectId { get; set; }
    public string? ProspectName { get; set; }
    public string? Status { get; set; }
    public string? SiteId { get; set; }
    public string? RecruitId { get; set; }
    public DateTime? ContactDateFrom { get; set; }
    public DateTime? ContactDateTo { get; set; }
}

