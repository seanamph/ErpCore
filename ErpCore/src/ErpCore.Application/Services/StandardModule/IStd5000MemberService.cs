using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD5000 會員服務介面 (SYS5210-SYS52A0 - 會員管理)
/// </summary>
public interface IStd5000MemberService
{
    Task<PagedResult<Std5000MemberDto>> GetStd5000MemberListAsync(Std5000MemberQueryDto query);
    Task<Std5000MemberDto?> GetStd5000MemberByIdAsync(long tKey);
    Task<Std5000MemberDto?> GetStd5000MemberByMemberIdAsync(string memberId);
    Task<long> CreateStd5000MemberAsync(CreateStd5000MemberDto dto);
    Task UpdateStd5000MemberAsync(long tKey, UpdateStd5000MemberDto dto);
    Task DeleteStd5000MemberAsync(long tKey);
    Task<PagedResult<Std5000MemberPointDto>> GetMemberPointsAsync(Std5000MemberPointQueryDto query);
    Task<long> AddMemberPointAsync(CreateStd5000MemberPointDto dto);
}

