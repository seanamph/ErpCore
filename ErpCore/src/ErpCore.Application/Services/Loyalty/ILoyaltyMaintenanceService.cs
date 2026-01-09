using ErpCore.Application.DTOs.Loyalty;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Loyalty;

/// <summary>
/// 忠誠度系統維護服務介面 (LPS - 忠誠度系統維護)
/// </summary>
public interface ILoyaltyMaintenanceService
{
    Task<PagedResult<LoyaltyPointTransactionDto>> GetTransactionsAsync(LoyaltyPointTransactionQueryDto query);
    Task<LoyaltyPointTransactionDto?> GetTransactionByRrnAsync(string rrn);
    Task<long> CreateTransactionAsync(CreateLoyaltyPointTransactionDto dto);
    Task VoidTransactionAsync(string rrn, VoidLoyaltyPointTransactionDto dto);
    Task<PagedResult<LoyaltyMemberDto>> GetMembersAsync(LoyaltyMemberQueryDto query);
    Task<LoyaltyMemberDto?> GetMemberByCardNoAsync(string cardNo);
    Task<LoyaltyMemberPointsDto?> GetMemberPointsAsync(string cardNo);
    Task<string> CreateMemberAsync(CreateLoyaltyMemberDto dto);
    Task UpdateMemberAsync(string cardNo, UpdateLoyaltyMemberDto dto);
}

/// <summary>
/// 建立忠誠度會員 DTO
/// </summary>
public class CreateLoyaltyMemberDto
{
    public string CardNo { get; set; } = string.Empty;
    public string? MemberName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ExpDate { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改忠誠度會員 DTO
/// </summary>
public class UpdateLoyaltyMemberDto
{
    public string? MemberName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? ExpDate { get; set; }
    public string Status { get; set; } = "A";
}

