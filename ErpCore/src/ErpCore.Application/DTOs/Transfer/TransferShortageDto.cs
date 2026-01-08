namespace ErpCore.Application.DTOs.Transfer;

/// <summary>
/// 調撥短溢單 DTO
/// </summary>
public class TransferShortageDto
{
    public string ShortageId { get; set; } = string.Empty;
    public string TransferId { get; set; } = string.Empty;
    public string? ReceiptId { get; set; }
    public DateTime ShortageDate { get; set; }
    public string FromShopId { get; set; } = string.Empty;
    public string ToShopId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ProcessType { get; set; }
    public string? ProcessUserId { get; set; }
    public DateTime? ProcessDate { get; set; }
    public string? ApproveUserId { get; set; }
    public DateTime? ApproveDate { get; set; }
    public decimal TotalShortageQty { get; set; }
    public decimal TotalAmount { get; set; }
    public string? ShortageReason { get; set; }
    public string? Memo { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettledDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<TransferShortageDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 調撥短溢單明細 DTO
/// </summary>
public class TransferShortageDetailDto
{
    public Guid DetailId { get; set; }
    public string ShortageId { get; set; } = string.Empty;
    public Guid? TransferDetailId { get; set; }
    public Guid? ReceiptDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal TransferQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal ShortageQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? Amount { get; set; }
    public string? ShortageReason { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 建立調撥短溢單 DTO
/// </summary>
public class CreateTransferShortageDto
{
    public string TransferId { get; set; } = string.Empty;
    public string? ReceiptId { get; set; }
    public DateTime ShortageDate { get; set; }
    public string? ProcessType { get; set; }
    public string? ShortageReason { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public List<CreateTransferShortageDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立調撥短溢單明細 DTO
/// </summary>
public class CreateTransferShortageDetailDto
{
    public Guid? TransferDetailId { get; set; }
    public Guid? ReceiptDetailId { get; set; }
    public int LineNum { get; set; }
    public string GoodsId { get; set; } = string.Empty;
    public string? BarcodeId { get; set; }
    public decimal TransferQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal ShortageQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? ShortageReason { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改調撥短溢單 DTO
/// </summary>
public class UpdateTransferShortageDto
{
    public DateTime ShortageDate { get; set; }
    public string? ProcessType { get; set; }
    public string? ShortageReason { get; set; }
    public string? Memo { get; set; }
    public string? UpdatedBy { get; set; }
    public List<UpdateTransferShortageDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 修改調撥短溢單明細 DTO
/// </summary>
public class UpdateTransferShortageDetailDto
{
    public Guid? DetailId { get; set; }
    public decimal ShortageQty { get; set; }
    public decimal? UnitPrice { get; set; }
    public string? ShortageReason { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 查詢調撥短溢單 DTO
/// </summary>
public class TransferShortageQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ShortageId { get; set; }
    public string? TransferId { get; set; }
    public string? FromShopId { get; set; }
    public string? ToShopId { get; set; }
    public string? Status { get; set; }
    public string? ProcessType { get; set; }
    public DateTime? ShortageDateFrom { get; set; }
    public DateTime? ShortageDateTo { get; set; }
}

/// <summary>
/// 審核調撥短溢單 DTO
/// </summary>
public class ApproveTransferShortageDto
{
    public string ApproveUserId { get; set; } = string.Empty;
    public DateTime ApproveDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 處理調撥短溢單 DTO
/// </summary>
public class ProcessTransferShortageDto
{
    public string ProcessUserId { get; set; } = string.Empty;
    public DateTime ProcessDate { get; set; }
    public string ProcessType { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

