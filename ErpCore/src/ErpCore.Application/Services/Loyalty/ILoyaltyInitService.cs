using ErpCore.Application.DTOs.Loyalty;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Loyalty;

/// <summary>
/// 忠誠度系統初始化服務介面 (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
public interface ILoyaltyInitService
{
    Task<PagedResult<LoyaltySystemConfigDto>> GetConfigsAsync(LoyaltySystemConfigQueryDto query);
    Task<LoyaltySystemConfigDto?> GetConfigByIdAsync(string configId);
    Task<string> CreateConfigAsync(CreateLoyaltySystemConfigDto dto);
    Task UpdateConfigAsync(string configId, UpdateLoyaltySystemConfigDto dto);
    Task DeleteConfigAsync(string configId);
    Task<LoyaltySystemInitResponseDto> InitializeAsync(InitializeLoyaltySystemDto dto);
    Task<PagedResult<LoyaltySystemInitLogDto>> GetInitLogsAsync(LoyaltySystemInitLogQueryDto query);
}

/// <summary>
/// 忠誠度系統初始化記錄查詢 DTO
/// </summary>
public class LoyaltySystemInitLogQueryDto
{
    public string? InitId { get; set; }
    public string? InitStatus { get; set; }
    public DateTime? InitDateFrom { get; set; }
    public DateTime? InitDateTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

