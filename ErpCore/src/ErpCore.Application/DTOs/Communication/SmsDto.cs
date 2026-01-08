namespace ErpCore.Application.DTOs.Communication;

/// <summary>
/// 簡訊記錄 DTO
/// </summary>
public class SmsLogDto
{
    public long Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string? ErrorMessage { get; set; }
    public DateTime? SentAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Provider { get; set; }
    public string? ProviderMessageId { get; set; }
}

/// <summary>
/// 發送簡訊請求 DTO
/// </summary>
public class SendSmsRequestDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 簡訊記錄查詢 DTO
/// </summary>
public class SmsLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// 發送簡訊回應 DTO
/// </summary>
public class SendSmsResponseDto
{
    public long SmsLogId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? SentAt { get; set; }
}

