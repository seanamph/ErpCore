using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BusinessReport;

/// <summary>
/// 銷退卡 Repository 介面 (SYSL310)
/// </summary>
public interface IReturnCardRepository
{
    /// <summary>
    /// 根據主鍵查詢銷退卡
    /// </summary>
    Task<ReturnCard?> GetByIdAsync(long tKey);

    /// <summary>
    /// 根據UUID查詢銷退卡
    /// </summary>
    Task<ReturnCard?> GetByUuidAsync(Guid uuid);

    /// <summary>
    /// 查詢銷退卡列表（分頁）
    /// </summary>
    Task<PagedResult<ReturnCard>> QueryAsync(ReturnCardQuery query);

    /// <summary>
    /// 新增銷退卡
    /// </summary>
    Task<long> CreateAsync(ReturnCard returnCard);

    /// <summary>
    /// 修改銷退卡
    /// </summary>
    Task UpdateAsync(ReturnCard returnCard);

    /// <summary>
    /// 刪除銷退卡
    /// </summary>
    Task DeleteAsync(long tKey);
}

/// <summary>
/// 銷退卡查詢條件
/// </summary>
public class ReturnCardQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public int? CardYear { get; set; }
    public int? CardMonth { get; set; }
}

