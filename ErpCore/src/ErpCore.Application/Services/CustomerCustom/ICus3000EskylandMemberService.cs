using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerCustom;

/// <summary>
/// CUS3000.ESKYLAND 會員服務介面
/// </summary>
public interface ICus3000EskylandMemberService
{
    /// <summary>
    /// 查詢會員列表
    /// </summary>
    Task<PagedResult<Cus3000EskylandMemberDto>> GetCus3000EskylandMemberListAsync(Cus3000EskylandMemberQueryDto query);

    /// <summary>
    /// 查詢單筆會員
    /// </summary>
    Task<Cus3000EskylandMemberDto?> GetCus3000EskylandMemberByIdAsync(long tKey);

    /// <summary>
    /// 查詢會員（依會員編號）
    /// </summary>
    Task<Cus3000EskylandMemberDto?> GetCus3000EskylandMemberByMemberIdAsync(string memberId);

    /// <summary>
    /// 查詢會員（依會員卡號）
    /// </summary>
    Task<Cus3000EskylandMemberDto?> GetCus3000EskylandMemberByCardNoAsync(string cardNo);

    /// <summary>
    /// 新增會員
    /// </summary>
    Task<long> CreateCus3000EskylandMemberAsync(CreateCus3000EskylandMemberDto dto);

    /// <summary>
    /// 修改會員
    /// </summary>
    Task UpdateCus3000EskylandMemberAsync(long tKey, UpdateCus3000EskylandMemberDto dto);

    /// <summary>
    /// 刪除會員
    /// </summary>
    Task DeleteCus3000EskylandMemberAsync(long tKey);
}

