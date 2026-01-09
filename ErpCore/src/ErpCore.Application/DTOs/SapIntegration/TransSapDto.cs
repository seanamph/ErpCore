namespace ErpCore.Application.DTOs.SapIntegration;

/// <summary>
/// SAP整合資料 DTO (TransSAP系列)
/// </summary>
public class TransSapDto
{
    public long TKey { get; set; }
    public string TransId { get; set; } = string.Empty;
    public string TransType { get; set; } = string.Empty;
    public string SapSystemCode { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string Status { get; set; } = "P";
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// SAP整合資料查詢 DTO
/// </summary>
public class TransSapQueryDto
{
    public string? TransId { get; set; }
    public string? TransType { get; set; }
    public string? SapSystemCode { get; set; }
    public string? Status { get; set; }
    public DateTime? TransDateFrom { get; set; }
    public DateTime? TransDateTo { get; set; }
    public string? Keyword { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}

/// <summary>
/// 建立SAP整合資料 DTO
/// </summary>
public class CreateTransSapDto
{
    public string TransId { get; set; } = string.Empty;
    public string TransType { get; set; } = string.Empty;
    public string SapSystemCode { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string Status { get; set; } = "P";
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改SAP整合資料 DTO
/// </summary>
public class UpdateTransSapDto
{
    public string TransType { get; set; } = string.Empty;
    public string SapSystemCode { get; set; } = string.Empty;
    public DateTime TransDate { get; set; }
    public string Status { get; set; } = "P";
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public string? Memo { get; set; }
}

