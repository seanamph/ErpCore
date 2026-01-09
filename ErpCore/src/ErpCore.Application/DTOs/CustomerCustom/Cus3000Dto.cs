using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.CustomerCustom;

/// <summary>
/// CUS3000 會員 DTO (SYS3130-SYS3160 - 會員管理)
/// </summary>
public class Cus3000MemberDto
{
    public long TKey { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? CardNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? PhotoPath { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// CUS3000 會員查詢 DTO
/// </summary>
public class Cus3000MemberQueryDto
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

/// <summary>
/// CUS3000 會員建立 DTO
/// </summary>
public class CreateCus3000MemberDto
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? CardNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? PhotoPath { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// CUS3000 會員修改 DTO
/// </summary>
public class UpdateCus3000MemberDto
{
    public string MemberName { get; set; } = string.Empty;
    public string? CardNo { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Gender { get; set; }
    public string? PhotoPath { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// CUS3000 促銷活動 DTO (SYS3310-SYS3399 - 促銷活動管理)
/// </summary>
public class Cus3000PromotionDto
{
    public long TKey { get; set; }
    public string PromotionId { get; set; } = string.Empty;
    public string PromotionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// CUS3000 促銷活動查詢 DTO
/// </summary>
public class Cus3000PromotionQueryDto
{
    public string? PromotionId { get; set; }
    public string? PromotionName { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public DateTime? EndDateFrom { get; set; }
    public DateTime? EndDateTo { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// CUS3000 促銷活動建立 DTO
/// </summary>
public class CreateCus3000PromotionDto
{
    public string PromotionId { get; set; } = string.Empty;
    public string PromotionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// CUS3000 促銷活動修改 DTO
/// </summary>
public class UpdateCus3000PromotionDto
{
    public string PromotionName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// CUS3000 活動 DTO (SYS3510-SYS3580 - 活動管理)
/// </summary>
public class Cus3000ActivityDto
{
    public long TKey { get; set; }
    public string ActivityId { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// CUS3000 活動查詢 DTO
/// </summary>
public class Cus3000ActivityQueryDto
{
    public string? ActivityId { get; set; }
    public string? ActivityName { get; set; }
    public DateTime? ActivityDateFrom { get; set; }
    public DateTime? ActivityDateTo { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// CUS3000 活動建立 DTO
/// </summary>
public class CreateCus3000ActivityDto
{
    public string ActivityId { get; set; } = string.Empty;
    public string ActivityName { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// CUS3000 活動修改 DTO
/// </summary>
public class UpdateCus3000ActivityDto
{
    public string ActivityName { get; set; } = string.Empty;
    public DateTime ActivityDate { get; set; }
    public string Status { get; set; } = "A";
}

