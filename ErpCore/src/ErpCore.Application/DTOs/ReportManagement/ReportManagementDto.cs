namespace ErpCore.Application.DTOs.ReportManagement;

/// <summary>
/// 收款項目 DTO (SYSR110-SYSR120)
/// </summary>
public class ArItemsDto
{
    public long TKey { get; set; }
    public string SiteId { get; set; } = string.Empty;
    public string? SiteName { get; set; }
    public string AritemId { get; set; } = string.Empty;
    public string AritemName { get; set; } = string.Empty;
    public string? StypeId { get; set; }
    public string? StypeName { get; set; }
    public string? Notes { get; set; }
    public string? Buser { get; set; }
    public DateTime? Btime { get; set; }
    public int? Cpriority { get; set; }
    public string? Cgroup { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立收款項目 DTO
/// </summary>
public class CreateArItemsDto
{
    public string SiteId { get; set; } = string.Empty;
    public string AritemId { get; set; } = string.Empty;
    public string AritemName { get; set; } = string.Empty;
    public string? StypeId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改收款項目 DTO
/// </summary>
public class UpdateArItemsDto
{
    public string AritemName { get; set; } = string.Empty;
    public string? StypeId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 收款項目查詢 DTO
/// </summary>
public class ArItemsQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public string? AritemId { get; set; }
    public string? AritemName { get; set; }
    public string? StypeId { get; set; }
}

// ============================================
// 應收帳款 DTO (SYSR210-SYSR240)
// ============================================

/// <summary>
/// 應收帳款 DTO (SYSR210-SYSR240)
/// </summary>
public class AccountsReceivableDto
{
    public long TKey { get; set; }
    public long? VoucherM_TKey { get; set; }
    public string? ObjectId { get; set; }
    public string? ObjectName { get; set; }
    public string? AcctKey { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public decimal ReceiptAmount { get; set; }
    public string? AritemId { get; set; }
    public string? AritemName { get; set; }
    public string? ReceiptNo { get; set; }
    public string? VoucherNo { get; set; }
    public string? VoucherStatus { get; set; }
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? CurrencyId { get; set; }
    public decimal ExchangeRate { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立應收帳款 DTO
/// </summary>
public class CreateAccountsReceivableDto
{
    public long? VoucherM_TKey { get; set; }
    public string? ObjectId { get; set; }
    public string? AcctKey { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public decimal ReceiptAmount { get; set; }
    public string? AritemId { get; set; }
    public string? ReceiptNo { get; set; }
    public string? VoucherNo { get; set; }
    public string? VoucherStatus { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? CurrencyId { get; set; }
    public decimal ExchangeRate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改應收帳款 DTO
/// </summary>
public class UpdateAccountsReceivableDto
{
    public DateTime? ReceiptDate { get; set; }
    public decimal ReceiptAmount { get; set; }
    public string? AritemId { get; set; }
    public string? ReceiptNo { get; set; }
    public string? VoucherNo { get; set; }
    public string? VoucherStatus { get; set; }
    public string? CurrencyId { get; set; }
    public decimal ExchangeRate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 應收帳款查詢 DTO
/// </summary>
public class AccountsReceivableQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SiteId { get; set; }
    public string? ShopId { get; set; }
    public string? ObjectId { get; set; }
    public string? ReceiptNo { get; set; }
    public string? VoucherNo { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? AritemId { get; set; }
}

// ============================================
// 收款沖帳傳票 DTO (SYSR310-SYSR450)
// ============================================

/// <summary>
/// 收款沖帳傳票 DTO (SYSR310-SYSR450)
/// </summary>
public class ReceiptVoucherTransferDto
{
    public long TKey { get; set; }
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string? ObjectId { get; set; }
    public string? ObjectName { get; set; }
    public string? AcctKey { get; set; }
    public decimal ReceiptAmount { get; set; }
    public string? AritemId { get; set; }
    public string? AritemName { get; set; }
    public string? VoucherNo { get; set; }
    public long? VoucherM_TKey { get; set; }
    public string TransferStatus { get; set; } = "P";
    public string? TransferStatusName { get; set; }
    public DateTime? TransferDate { get; set; }
    public string? TransferUser { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立收款沖帳傳票 DTO
/// </summary>
public class CreateReceiptVoucherTransferDto
{
    public string ReceiptNo { get; set; } = string.Empty;
    public DateTime ReceiptDate { get; set; }
    public string? ObjectId { get; set; }
    public string? AcctKey { get; set; }
    public decimal ReceiptAmount { get; set; }
    public string? AritemId { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改收款沖帳傳票 DTO
/// </summary>
public class UpdateReceiptVoucherTransferDto
{
    public string? ReceiptNo { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string? ObjectId { get; set; }
    public string? AcctKey { get; set; }
    public decimal? ReceiptAmount { get; set; }
    public string? AritemId { get; set; }
    public string? VoucherNo { get; set; }
    public long? VoucherM_TKey { get; set; }
    public string? TransferStatus { get; set; }
    public DateTime? TransferDate { get; set; }
    public string? TransferUser { get; set; }
    public string? ErrorMessage { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 收款沖帳傳票查詢 DTO
/// </summary>
public class ReceiptVoucherTransferQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "ReceiptDate";
    public string? SortOrder { get; set; } = "DESC";
    public string? ReceiptNo { get; set; }
    public string? VoucherNo { get; set; }
    public DateTime? ReceiptDateFrom { get; set; }
    public DateTime? ReceiptDateTo { get; set; }
    public string? TransferStatus { get; set; }
    public string? ObjectId { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 批次拋轉收款沖帳傳票 DTO
/// </summary>
public class BatchTransferReceiptVoucherDto
{
    public List<string> ReceiptNos { get; set; } = new();
}

/// <summary>
/// 批次拋轉結果 DTO
/// </summary>
public class BatchTransferResultDto
{
    public int Total { get; set; }
    public int Success { get; set; }
    public int Failed { get; set; }
    public List<TransferResultItemDto> Results { get; set; } = new();
}

/// <summary>
/// 拋轉結果項目 DTO
/// </summary>
public class TransferResultItemDto
{
    public string ReceiptNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? VoucherNo { get; set; }
    public string? ErrorMessage { get; set; }
}

// ============================================
// 保證金 DTO (SYSR510-SYSR570)
// ============================================

/// <summary>
/// 保證金 DTO (SYSR510-SYSR570)
/// </summary>
public class DepositsDto
{
    public long TKey { get; set; }
    public string DepositNo { get; set; } = string.Empty;
    public DateTime DepositDate { get; set; }
    public string? ObjectId { get; set; }
    public string? ObjectName { get; set; }
    public decimal DepositAmount { get; set; }
    public string? DepositType { get; set; }
    public string? DepositTypeName { get; set; }
    public string DepositStatus { get; set; } = "A";
    public string? DepositStatusName { get; set; }
    public DateTime? ReturnDate { get; set; }
    public decimal ReturnAmount { get; set; }
    public string? VoucherNo { get; set; }
    public long? VoucherM_TKey { get; set; }
    public string? VoucherStatus { get; set; }
    public DateTime? CheckDueDate { get; set; }
    public string? ShopId { get; set; }
    public string? ShopName { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立保證金 DTO
/// </summary>
public class CreateDepositsDto
{
    public DateTime DepositDate { get; set; }
    public string? ObjectId { get; set; }
    public decimal DepositAmount { get; set; }
    public string? DepositType { get; set; }
    public DateTime? CheckDueDate { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 修改保證金 DTO
/// </summary>
public class UpdateDepositsDto
{
    public decimal? DepositAmount { get; set; }
    public string? DepositType { get; set; }
    public DateTime? CheckDueDate { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 保證金查詢 DTO
/// </summary>
public class DepositsQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "DepositDate";
    public string? SortOrder { get; set; } = "DESC";
    public string? DepositNo { get; set; }
    public string? ObjectId { get; set; }
    public DateTime? DepositDateFrom { get; set; }
    public DateTime? DepositDateTo { get; set; }
    public string? DepositStatus { get; set; }
    public string? DepositType { get; set; }
    public string? ShopId { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 保證金退還 DTO
/// </summary>
public class ReturnDepositsDto
{
    public DateTime ReturnDate { get; set; }
    public decimal ReturnAmount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 保證金存款 DTO
/// </summary>
public class DepositAmountDto
{
    public decimal DepositAmount { get; set; }
    public DateTime DepositDate { get; set; }
    public string? Notes { get; set; }
}

