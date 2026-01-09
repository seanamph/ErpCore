using ErpCore.Domain.Entities.StandardModule;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 會員 Repository 介面 (SYS5210-SYS52A0 - 會員管理)
/// </summary>
public interface IStd5000MemberRepository
{
    Task<Std5000Member?> GetByIdAsync(long tKey);
    Task<Std5000Member?> GetByMemberIdAsync(string memberId);
    Task<IEnumerable<Std5000Member>> QueryAsync(Std5000MemberQuery query);
    Task<int> GetCountAsync(Std5000MemberQuery query);
    Task<long> CreateAsync(Std5000Member entity);
    Task UpdateAsync(Std5000Member entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// STD5000 會員查詢條件
/// </summary>
public class Std5000MemberQuery
{
    public string? MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? MemberType { get; set; }
    public string? Status { get; set; }
    public string? ShopId { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

