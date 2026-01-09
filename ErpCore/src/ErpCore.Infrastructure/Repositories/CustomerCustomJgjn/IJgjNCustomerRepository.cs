using ErpCore.Domain.Entities.CustomerCustomJgjn;

namespace ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;

/// <summary>
/// JGJN客戶 Repository 介面
/// </summary>
public interface IJgjNCustomerRepository
{
    Task<JgjNCustomer?> GetByIdAsync(long tKey);
    Task<JgjNCustomer?> GetByCustomerIdAsync(string customerId);
    Task<IEnumerable<JgjNCustomer>> QueryAsync(JgjNCustomerQuery query);
    Task<int> GetCountAsync(JgjNCustomerQuery query);
    Task<long> CreateAsync(JgjNCustomer entity);
    Task UpdateAsync(JgjNCustomer entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// JGJN客戶查詢條件
/// </summary>
public class JgjNCustomerQuery
{
    public string? CustomerType { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

