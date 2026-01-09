using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Repositories;

namespace ErpCore.Infrastructure.Repositories.CustomerCustom;

/// <summary>
/// CUS3000.ESKYLAND 會員 Repository 介面
/// </summary>
public interface ICus3000EskylandMemberRepository : IRepository<Cus3000EskylandMember>
{
    /// <summary>
    /// 依會員編號查詢
    /// </summary>
    Task<Cus3000EskylandMember?> GetByMemberIdAsync(string memberId);

    /// <summary>
    /// 依會員卡號查詢
    /// </summary>
    Task<Cus3000EskylandMember?> GetByCardNoAsync(string cardNo);

    /// <summary>
    /// 查詢會員列表
    /// </summary>
    Task<IEnumerable<Cus3000EskylandMember>> QueryAsync(Cus3000EskylandMemberQuery query);

    /// <summary>
    /// 取得查詢總數
    /// </summary>
    Task<int> GetCountAsync(Cus3000EskylandMemberQuery query);
}

/// <summary>
/// CUS3000.ESKYLAND 會員查詢條件
/// </summary>
public class Cus3000EskylandMemberQuery
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

