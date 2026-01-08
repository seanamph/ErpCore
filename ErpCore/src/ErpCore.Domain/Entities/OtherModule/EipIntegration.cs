namespace ErpCore.Domain.Entities.OtherModule;

/// <summary>
/// EIP整合設定實體
/// </summary>
public class EipIntegration
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
/// EIP交易記錄實體
/// </summary>
public class EipTransaction
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

