using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.CustomerCustom;

/// <summary>
/// CUS3000 會員服務介面 (SYS3130-SYS3160 - 會員管理)
/// </summary>
public interface ICus3000MemberService
{
    Task<PagedResult<Cus3000MemberDto>> GetCus3000MemberListAsync(Cus3000MemberQueryDto query);
    Task<Cus3000MemberDto?> GetCus3000MemberByIdAsync(long tKey);
    Task<Cus3000MemberDto?> GetCus3000MemberByMemberIdAsync(string memberId);
    Task<Cus3000MemberDto?> GetCus3000MemberByCardNoAsync(string cardNo);
    Task<long> CreateCus3000MemberAsync(CreateCus3000MemberDto dto);
    Task UpdateCus3000MemberAsync(long tKey, UpdateCus3000MemberDto dto);
    Task DeleteCus3000MemberAsync(long tKey);
}

