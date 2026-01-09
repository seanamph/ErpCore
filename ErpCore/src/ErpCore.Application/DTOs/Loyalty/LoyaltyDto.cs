namespace ErpCore.Application.DTOs.Loyalty;

/// <summary>
/// 忠誠度系統設定 DTO (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
public class LoyaltySystemConfigDto
{
    public string ConfigId { get; set; } = string.Empty;
    public string ConfigName { get; set; } = string.Empty;
    public string? ConfigValue { get; set; }
    public string ConfigType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 忠誠度系統設定查詢 DTO
/// </summary>
public class LoyaltySystemConfigQueryDto
{
    public string? ConfigId { get; set; }
    public string? ConfigType { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立忠誠度系統設定 DTO
/// </summary>
public class CreateLoyaltySystemConfigDto
{
    public string ConfigId { get; set; } = string.Empty;
    public string ConfigName { get; set; } = string.Empty;
    public string? ConfigValue { get; set; }
    public string ConfigType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改忠誠度系統設定 DTO
/// </summary>
public class UpdateLoyaltySystemConfigDto
{
    public string ConfigName { get; set; } = string.Empty;
    public string? ConfigValue { get; set; }
    public string ConfigType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 忠誠度系統初始化請求 DTO
/// </summary>
public class InitializeLoyaltySystemDto
{
    public string? MemberMid { get; set; }
    public string? MemberTid { get; set; }
    public string? PosServerIp { get; set; }
    public List<LoyaltySystemConfigItemDto> Configs { get; set; } = new();
}

/// <summary>
/// 忠誠度系統設定項目 DTO
/// </summary>
public class LoyaltySystemConfigItemDto
{
    public string ConfigId { get; set; } = string.Empty;
    public string ConfigValue { get; set; } = string.Empty;
}

/// <summary>
/// 忠誠度系統初始化回應 DTO
/// </summary>
public class LoyaltySystemInitResponseDto
{
    public string InitId { get; set; } = string.Empty;
    public string InitStatus { get; set; } = string.Empty;
    public string? InitMessage { get; set; }
}

/// <summary>
/// 忠誠度系統初始化記錄 DTO
/// </summary>
public class LoyaltySystemInitLogDto
{
    public long TKey { get; set; }
    public string InitId { get; set; } = string.Empty;
    public string InitStatus { get; set; } = string.Empty;
    public DateTime InitDate { get; set; }
    public string? InitMessage { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 忠誠度點數交易 DTO (LPS - 忠誠度系統維護)
/// </summary>
public class LoyaltyPointTransactionDto
{
    public long TKey { get; set; }
    public string RRN { get; set; } = string.Empty;
    public string CardNo { get; set; } = string.Empty;
    public string? TraceNo { get; set; }
    public string? ExpDate { get; set; }
    public decimal AwardPoints { get; set; }
    public decimal RedeemPoints { get; set; }
    public string? ReversalFlag { get; set; }
    public decimal? Amount { get; set; }
    public string? VoidFlag { get; set; }
    public string? AuthCode { get; set; }
    public DateTime? ForceDate { get; set; }
    public string? Invoice { get; set; }
    public string? TransType { get; set; }
    public string? TxnType { get; set; }
    public DateTime TransTime { get; set; }
    public string Status { get; set; } = "SUCCESS";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 忠誠度點數交易查詢 DTO
/// </summary>
public class LoyaltyPointTransactionQueryDto
{
    public string? RRN { get; set; }
    public string? CardNo { get; set; }
    public string? TransType { get; set; }
    public string? Status { get; set; }
    public DateTime? TransTimeFrom { get; set; }
    public DateTime? TransTimeTo { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立忠誠度點數交易 DTO
/// </summary>
public class CreateLoyaltyPointTransactionDto
{
    public string CardNo { get; set; } = string.Empty;
    public string? TraceNo { get; set; }
    public string? ExpDate { get; set; }
    public decimal AwardPoints { get; set; }
    public decimal RedeemPoints { get; set; }
    public decimal? Amount { get; set; }
    public string? Invoice { get; set; }
    public string? TransType { get; set; }
    public string? TxnType { get; set; }
    public DateTime? ForceDate { get; set; }
}

/// <summary>
/// 取消忠誠度點數交易 DTO
/// </summary>
public class VoidLoyaltyPointTransactionDto
{
    public string ReversalFlag { get; set; } = "Y";
    public string VoidFlag { get; set; } = "Y";
}

/// <summary>
/// 忠誠度會員 DTO
/// </summary>
public class LoyaltyMemberDto
{
    public string CardNo { get; set; } = string.Empty;
    public string? MemberName { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public decimal TotalPoints { get; set; }
    public decimal AvailablePoints { get; set; }
    public string? ExpDate { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 忠誠度會員查詢 DTO
/// </summary>
public class LoyaltyMemberQueryDto
{
    public string? CardNo { get; set; }
    public string? MemberName { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 會員點數資訊 DTO
/// </summary>
public class LoyaltyMemberPointsDto
{
    public string CardNo { get; set; } = string.Empty;
    public decimal TotalPoints { get; set; }
    public decimal AvailablePoints { get; set; }
    public string? ExpDate { get; set; }
}

