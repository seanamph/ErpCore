using ErpCore.Domain.Entities.StandardModule;

namespace ErpCore.Infrastructure.Repositories.StandardModule;

/// <summary>
/// STD5000 會員積分 Repository 介面 (SYS5210-SYS52A0 - 會員積分管理)
/// </summary>
public interface IStd5000MemberPointRepository
{
    Task<Std5000MemberPoint?> GetByIdAsync(long tKey);
    Task<IEnumerable<Std5000MemberPoint>> GetByMemberIdAsync(string memberId);
    Task<IEnumerable<Std5000MemberPoint>> QueryAsync(Std5000MemberPointQuery query);
    Task<int> GetCountAsync(Std5000MemberPointQuery query);
    Task<long> CreateAsync(Std5000MemberPoint entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// STD5000 會員積分查詢條件
/// </summary>
public class Std5000MemberPointQuery
{
    public string? MemberId { get; set; }
    public string? TransType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

