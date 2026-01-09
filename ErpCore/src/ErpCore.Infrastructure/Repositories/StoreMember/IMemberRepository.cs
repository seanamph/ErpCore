using ErpCore.Domain.Entities.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.StoreMember;

/// <summary>
/// 會員 Repository 介面 (SYS3000 - 會員資料維護)
/// </summary>
public interface IMemberRepository
{
    /// <summary>
    /// 根據會員編號查詢會員
    /// </summary>
    Task<Member?> GetByIdAsync(string memberId);

    /// <summary>
    /// 查詢會員列表（分頁）
    /// </summary>
    Task<PagedResult<Member>> QueryAsync(MemberQuery query);

    /// <summary>
    /// 查詢會員總數
    /// </summary>
    Task<int> GetCountAsync(MemberQuery query);

    /// <summary>
    /// 新增會員
    /// </summary>
    Task<Member> CreateAsync(Member member);

    /// <summary>
    /// 修改會員
    /// </summary>
    Task<Member> UpdateAsync(Member member);

    /// <summary>
    /// 刪除會員（軟刪除，將狀態設為停用）
    /// </summary>
    Task DeleteAsync(string memberId);

    /// <summary>
    /// 檢查會員編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string memberId);

    /// <summary>
    /// 更新會員狀態
    /// </summary>
    Task UpdateStatusAsync(string memberId, string status);

    /// <summary>
    /// 更新會員照片路徑
    /// </summary>
    Task UpdatePhotoPathAsync(string memberId, string photoPath);

    /// <summary>
    /// 查詢會員積分記錄
    /// </summary>
    Task<PagedResult<MemberPoint>> GetMemberPointsAsync(string memberId, MemberPointQuery query);
}

/// <summary>
/// 會員查詢條件
/// </summary>
public class MemberQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? PersonalId { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? MemberLevel { get; set; }
    public string? Status { get; set; }
    public string? CardNo { get; set; }
}

/// <summary>
/// 會員積分查詢條件
/// </summary>
public class MemberPointQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? TransactionDateFrom { get; set; }
    public DateTime? TransactionDateTo { get; set; }
    public string? TransactionType { get; set; }
}

