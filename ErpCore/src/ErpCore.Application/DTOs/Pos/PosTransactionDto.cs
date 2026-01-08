namespace ErpCore.Application.DTOs.Pos;

/// <summary>
/// POS交易資料傳輸物件
/// </summary>
public class PosTransactionDto
{
    public long Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string StoreId { get; set; } = string.Empty;
    public string? PosId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? CustomerId { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? SyncAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<PosTransactionDetailDto>? Details { get; set; }
}

/// <summary>
/// POS交易明細 DTO
/// </summary>
public class PosTransactionDetailDto
{
    public long Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public int LineNo { get; set; }
    public string ProductId { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }
    public decimal? Discount { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// POS交易查詢 DTO
/// </summary>
public class PosTransactionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "DESC";
    public string? TransactionId { get; set; }
    public string? StoreId { get; set; }
    public string? PosId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? TransactionType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// POS同步請求 DTO
/// </summary>
public class PosSyncRequestDto
{
    public string StoreId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string SyncType { get; set; } = "Incremental"; // Incremental/Full
}

/// <summary>
/// POS同步結果 DTO
/// </summary>
public class PosSyncResultDto
{
    public long SyncLogId { get; set; }
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// POS同步記錄 DTO
/// </summary>
public class PosSyncLogDto
{
    public long Id { get; set; }
    public string SyncType { get; set; } = string.Empty;
    public string SyncDirection { get; set; } = string.Empty;
    public int RecordCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string? ErrorMessage { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// POS同步記錄查詢 DTO
/// </summary>
public class PosSyncLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "DESC";
    public string? SyncType { get; set; }
    public string? SyncDirection { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

