namespace ErpCore.Application.DTOs.Procurement;

/// <summary>
/// 採購擴展功能 DTO (SYSP610)
/// </summary>
public class PurchaseExtendedFunctionDto
{
    public long TKey { get; set; }
    public string ExtFunctionId { get; set; } = string.Empty;
    public string ExtFunctionName { get; set; } = string.Empty;
    public string? ExtFunctionType { get; set; }
    public string? ExtFunctionDesc { get; set; }
    public string? ExtFunctionConfig { get; set; }
    public string? ParameterConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立採購擴展功能 DTO
/// </summary>
public class CreatePurchaseExtendedFunctionDto
{
    public string ExtFunctionId { get; set; } = string.Empty;
    public string ExtFunctionName { get; set; } = string.Empty;
    public string? ExtFunctionType { get; set; }
    public string? ExtFunctionDesc { get; set; }
    public string? ExtFunctionConfig { get; set; }
    public string? ParameterConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改採購擴展功能 DTO
/// </summary>
public class UpdatePurchaseExtendedFunctionDto
{
    public string ExtFunctionName { get; set; } = string.Empty;
    public string? ExtFunctionType { get; set; }
    public string? ExtFunctionDesc { get; set; }
    public string? ExtFunctionConfig { get; set; }
    public string? ParameterConfig { get; set; }
    public string Status { get; set; } = "A";
    public int SeqNo { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢採購擴展功能 DTO
/// </summary>
public class PurchaseExtendedFunctionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ExtFunctionId { get; set; }
    public string? ExtFunctionName { get; set; }
    public string? ExtFunctionType { get; set; }
    public string? Status { get; set; }
}
