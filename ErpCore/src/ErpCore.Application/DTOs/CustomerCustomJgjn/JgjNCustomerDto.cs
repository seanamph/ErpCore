using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.CustomerCustomJgjn;

/// <summary>
/// JGJN客戶 DTO
/// </summary>
public class JgjNCustomerDto
{
    public long TKey { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerType { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// JGJN客戶查詢 DTO
/// </summary>
public class JgjNCustomerQueryDto
{
    public string? CustomerType { get; set; }
    public string? Status { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// JGJN客戶建立 DTO
/// </summary>
public class CreateJgjNCustomerDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerType { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

/// <summary>
/// JGJN客戶修改 DTO
/// </summary>
public class UpdateJgjNCustomerDto
{
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerType { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string Status { get; set; } = "A";
    public string? Memo { get; set; }
}

