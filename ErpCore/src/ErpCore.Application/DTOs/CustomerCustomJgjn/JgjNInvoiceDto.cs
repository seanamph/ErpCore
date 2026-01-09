using ErpCore.Shared.Common;

namespace ErpCore.Application.DTOs.CustomerCustomJgjn;

/// <summary>
/// JGJN發票 DTO
/// </summary>
public class JgjNInvoiceDto
{
    public long TKey { get; set; }
    public string InvoiceId { get; set; } = string.Empty;
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public string Currency { get; set; } = "TWD";
    public string Status { get; set; } = "PENDING";
    public string? PrintStatus { get; set; }
    public DateTime? PrintDate { get; set; }
    public string? FilePath { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// JGJN發票查詢 DTO
/// </summary>
public class JgjNInvoiceQueryDto
{
    public string? CustomerId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Status { get; set; }
    public string? PrintStatus { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// JGJN發票建立 DTO
/// </summary>
public class CreateJgjNInvoiceDto
{
    public string InvoiceId { get; set; } = string.Empty;
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public string Currency { get; set; } = "TWD";
    public string Status { get; set; } = "PENDING";
    public string? Memo { get; set; }
}

/// <summary>
/// JGJN發票修改 DTO
/// </summary>
public class UpdateJgjNInvoiceDto
{
    public string? InvoiceNo { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public string Currency { get; set; } = "TWD";
    public string Status { get; set; } = "PENDING";
    public string? Memo { get; set; }
}

/// <summary>
/// JGJN發票列印 DTO
/// </summary>
public class PrintJgjNInvoiceDto
{
    public long TKey { get; set; }
    public string? FilePath { get; set; }
}

