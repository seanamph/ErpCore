using ErpCore.Domain.Entities.StandardModule;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 交易 Repository 介面 (SYS5310-SYS53C6 - 交易管理)
/// </summary>
public interface IStd5000TransactionRepository
{
    Task<Std5000Transaction?> GetByIdAsync(long tKey);
    Task<Std5000Transaction?> GetByTransIdAsync(string transId);
    Task<IEnumerable<Std5000Transaction>> QueryAsync(Std5000TransactionQuery query);
    Task<int> GetCountAsync(Std5000TransactionQuery query);
    Task<long> CreateAsync(Std5000Transaction entity);
    Task UpdateAsync(Std5000Transaction entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// STD5000 交易查詢條件
/// </summary>
public class Std5000TransactionQuery
{
    public string? TransId { get; set; }
    public string? TransType { get; set; }
    public string? MemberId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

