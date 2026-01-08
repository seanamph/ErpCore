namespace ErpCore.Application.DTOs.Transfer;

/// <summary>
/// 調撥驗收單 DTO
/// </summary>
public class TransferReceiptDto
{
    public string ReceiptId { get; set; } = string.Empty;
    public string TransferId { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string FromShopId { get; set; } = string.Empty;
    public string ToShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReceiptUserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public string? Memo { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettledDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TransferReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 調撥驗收單明細 DTO
/// </summary>
public class TransferReceiptDetailDto
{
    public Guid DetailId { get; set; }
    public string ReceiptId { get; set; } = string.Empty;
    public Guid? TransferDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal TransferQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立調撥驗收單 DTO
/// </summary>
public class CreateTransferReceiptDto
{
    public string TransferId { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string? ReceiptUserId { get; set; }
    public string? Memo { get; set; }
    public List<CreateTransferReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立調撥驗收單明細 DTO
/// </summary>
public class CreateTransferReceiptDetailDto
{
    public Guid? TransferDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改調撥驗收單 DTO
/// </summary>
public class UpdateTransferReceiptDto
{
    public DateTime ReceiptDate { get; set; }
    public string? ReceiptUserId { get; set; }
    public string? Memo { get; set; }
    public List<UpdateTransferReceiptDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改調撥驗收單明細 DTO
/// </summary>
public class UpdateTransferReceiptDetailDto
{
    public Guid? DetailId { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢調撥驗收單 DTO
/// </summary>
public class TransferReceiptQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReceiptId { get; set; }
    public string? TransferId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? ReceiptDateFrom { get; set; }
    public DateTime? ReceiptDateTo { get; set; }
}

/// <summary>
/// 待驗收調撥單查詢 DTO
/// </summary>
public class PendingTransferOrderQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TransferId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public DateTime? TransferDateFrom { get; set; }
    public DateTime? TransferDateTo { get; set; }
}

/// <summary>
/// 待驗收調撥單 DTO
/// </summary>
public class PendingTransferOrderDto
{
    public string TransferId { get; set; } = string.Empty;
    public DateTime TransferDate { get; set; }
    public string FromShopId { get; set; } = string.Empty;
    public string ToShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal PendingReceiptQty { get; set; }
}

