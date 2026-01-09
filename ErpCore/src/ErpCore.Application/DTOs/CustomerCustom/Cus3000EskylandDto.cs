using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.CustomerCustom;

/// <summary>
/// CUS3000.ESKYLAND 會員 DTO
/// </summary>
public class Cus3000EskylandMemberDto
{
    public long TKey { get; set; }
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? CardNo { get; set; }
    public string? EskylandSpecificField { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// CUS3000.ESKYLAND 會員查詢 DTO
/// </summary>
public class Cus3000EskylandMemberQueryDto
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
/// CUS3000.ESKYLAND 會員建立 DTO
/// </summary>
public class CreateCus3000EskylandMemberDto
{
    public string MemberId { get; set; } = string.Empty;
    public string MemberName { get; set; } = string.Empty;
    public string? CardNo { get; set; }
    public string? EskylandSpecificField { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// CUS3000.ESKYLAND 會員修改 DTO
/// </summary>
public class UpdateCus3000EskylandMemberDto
{
    public string MemberName { get; set; } = string.Empty;
    public string? CardNo { get; set; }
    public string? EskylandSpecificField { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "A";
}

