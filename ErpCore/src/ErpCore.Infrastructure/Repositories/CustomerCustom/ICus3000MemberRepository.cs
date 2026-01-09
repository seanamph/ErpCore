using ErpCore.Domain.Entities.CustomerCustom;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000 會員 Repository 介面 (SYS3130-SYS3160 - 會員管理)
/// </summary>
public interface ICus3000MemberRepository
{
    Task<Cus3000Member?> GetByIdAsync(long tKey);
    Task<Cus3000Member?> GetByMemberIdAsync(string memberId);
    Task<Cus3000Member?> GetByCardNoAsync(string cardNo);
    Task<IEnumerable<Cus3000Member>> QueryAsync(Cus3000MemberQuery query);
    Task<int> GetCountAsync(Cus3000MemberQuery query);
    Task<long> CreateAsync(Cus3000Member entity);
    Task UpdateAsync(Cus3000Member entity);
    Task DeleteAsync(long tKey);
}

/// <summary>
/// CUS3000 會員查詢條件
/// </summary>
public class Cus3000MemberQuery
{
    public string? MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? CardNo { get; set; }
    public string? Phone { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

