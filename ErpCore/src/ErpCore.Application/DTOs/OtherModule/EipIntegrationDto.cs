namespace ErpCore.Application.DTOs.OtherModule;

/// <summary>
/// EIP整合設定 DTO
/// </summary>
public class EipIntegrationDto
{
    public long IntegrationId { get; set; }
    public string ProgId { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string EipUrl { get; set; } = string.Empty;
    public string? Fid { get; set; }
    public string? SingleField { get; set; }
    public string? MultiField { get; set; }
    public string? DetailTable { get; set; }
    public string? MultiMSeqNo { get; set; }
    public string Status { get; set; } = "1";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 傳送表單至EIP請求 DTO
/// </summary>
public class SendFormToEipRequestDto
{
    public string ProgId { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public Dictionary<string, object>? FormData { get; set; }
    public List<Dictionary<string, object>>? DetailData { get; set; }
}

/// <summary>
/// EIP交易記錄 DTO
/// </summary>
public class EipTransactionDto
{
    public long TransactionId { get; set; }
    public long IntegrationId { get; set; }
    public string ProgId { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string? FlowId { get; set; }
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 新增EIP整合設定 DTO
/// </summary>
public class CreateEipIntegrationDto
{
    public string ProgId { get; set; } = string.Empty;
    public string PageId { get; set; } = string.Empty;
    public string EipUrl { get; set; } = string.Empty;
    public string? Fid { get; set; }
    public string? SingleField { get; set; }
    public string? MultiField { get; set; }
    public string? DetailTable { get; set; }
    public string? MultiMSeqNo { get; set; }
    public string Status { get; set; } = "1";
}

/// <summary>
/// EIP整合查詢 DTO
/// </summary>
public class EipIntegrationQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ProgId { get; set; }
    public string? PageId { get; set; }
    public string? Status { get; set; }
}

