using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.ReportManagement;

/// <summary>
/// 保證金 Repository 介面 (SYSR510-SYSR570)
/// </summary>
public interface IDepositsRepository
{
    Task<Deposits?> GetByIdAsync(long tKey);
    Task<Deposits?> GetByDepositNoAsync(string depositNo);
    Task<IEnumerable<Deposits>> GetAllAsync();
    Task<PagedResult<Deposits>> QueryAsync(DepositsQuery query);
    Task<IEnumerable<Deposits>> GetByObjectIdAsync(string objectId);
    Task<IEnumerable<Deposits>> GetByDepositStatusAsync(string depositStatus);
    Task<Deposits> CreateAsync(Deposits entity);
    Task<Deposits> UpdateAsync(Deposits entity);
    Task DeleteAsync(long tKey);
    Task<bool> ExistsAsync(string depositNo);
}

/// <summary>
/// 保證金查詢條件
/// </summary>
public class DepositsQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "DepositDate";
    public string? SortOrder { get; set; } = "DESC";
    public string? DepositNo { get; set; }
    public string? ObjectId { get; set; }
    public DateTime? DepositDateFrom { get; set; }
    public DateTime? DepositDateTo { get; set; }
    public string? DepositStatus { get; set; }
    public string? DepositType { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
}

