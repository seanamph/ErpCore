using ErpCore.Application.DTOs.StoreMember;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StoreMember;

/// <summary>
/// 會員服務介面 (SYS3000 - 會員資料維護)
/// </summary>
public interface IMemberService
{
    /// <summary>
    /// 查詢會員列表
    /// </summary>
    Task<PagedResult<MemberDto>> GetMembersAsync(MemberQueryDto query);

    /// <summary>
    /// 查詢單筆會員
    /// </summary>
    Task<MemberDto> GetMemberByIdAsync(string memberId);

    /// <summary>
    /// 新增會員
    /// </summary>
    Task<string> CreateMemberAsync(CreateMemberDto dto);

    /// <summary>
    /// 修改會員
    /// </summary>
    Task UpdateMemberAsync(string memberId, UpdateMemberDto dto);

    /// <summary>
    /// 刪除會員
    /// </summary>
    Task DeleteMemberAsync(string memberId);

    /// <summary>
    /// 檢查會員編號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string memberId);

    /// <summary>
    /// 更新會員狀態
    /// </summary>
    Task UpdateStatusAsync(string memberId, string status);

    /// <summary>
    /// 查詢會員積分記錄
    /// </summary>
    Task<PagedResult<MemberPointDto>> GetMemberPointsAsync(string memberId, MemberPointQueryDto query);
}

