namespace ErpCore.Application.DTOs.Transfer;

/// <summary>
/// 調撥驗退單 DTO
/// </summary>
public class TransferReturnDto
{
    public string ReturnId { get; set; } = string.Empty;
    public string TransferId { get; set; } = string.Empty;
    public string? ReceiptId { get; set; }
    public DateTime ReturnDate { get; set; }
    public string FromShopId { get; set; } = string.Empty;
    public string ToShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReturnUserId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalQty { get; set; }
    public string? ReturnReason { get; set; }
    public string? Memo { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettledDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TransferReturnDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 調撥驗退單明細 DTO
/// </summary>
public class TransferReturnDetailDto
{
    public Guid DetailId { get; set; }
    public string ReturnId { get; set; } = string.Empty;
    public Guid? TransferDetailId { get; set; }
    public Guid? ReceiptDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal TransferQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal ReturnQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public string? ReturnReason { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立調撥驗退單 DTO
/// </summary>
public class CreateTransferReturnDto
{
    public string TransferId { get; set; } = string.Empty;
    public string? ReceiptId { get; set; }
    public DateTime ReturnDate { get; set; }
    public string? ReturnUserId { get; set; }
    public string? ReturnReason { get; set; }
    public string? Memo { get; set; }
    public List<CreateTransferReturnDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立調撥驗退單明細 DTO
/// </summary>
public class CreateTransferReturnDetailDto
{
    public Guid? TransferDetailId { get; set; }
    public Guid? ReceiptDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal ReturnQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? ReturnReason { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改調撥驗退單 DTO
/// </summary>
public class UpdateTransferReturnDto
{
    public DateTime ReturnDate { get; set; }
    public string? ReturnUserId { get; set; }
    public string? ReturnReason { get; set; }
    public string? Memo { get; set; }
    public List<UpdateTransferReturnDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改調撥驗退單明細 DTO
/// </summary>
public class UpdateTransferReturnDetailDto
{
    public Guid? DetailId { get; set; }
    public decimal ReturnQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? ReturnReason { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢調撥驗退單 DTO
/// </summary>
public class TransferReturnQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReturnId { get; set; }
    public string? TransferId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? ReturnDateFrom { get; set; }
    public DateTime? ReturnDateTo { get; set; }
}

/// <summary>
/// 待驗退調撥單查詢 DTO
/// </summary>
public class PendingTransferQueryDto
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
/// 待驗退調撥單 DTO
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
    public decimal ReturnQty { get; set; }
    public decimal PendingReturnQty { get; set; }
}

