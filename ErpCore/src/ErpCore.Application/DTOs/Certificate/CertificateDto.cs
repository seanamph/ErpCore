namespace ErpCore.Application.DTOs.Certificate;

/// <summary>
/// 憑證 DTO (SYSK110-SYSK150)
/// </summary>
public class VoucherDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string? VoucherTypeName { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string? ShopName { get; set; }
    public string Status { get; set; } = "D";
    public string? StatusName { get; set; }
    public string? ApplyUserId { get; set; }
    public string? ApplyUserName { get; set; }
    public DateTime? ApplyDate { get; set; }
    public string? ApproveUserId { get; set; }
    public string? ApproveUserName { get; set; }
    public DateTime? ApproveDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalDebitAmount { get; set; }
    public decimal TotalCreditAmount { get; set; }
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? SiteName { get; set; }
    public string? OrgId { get; set; }
    public string? OrgName { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<VoucherDetailDto>? Details { get; set; }
}

/// <summary>
/// 憑證明細 DTO
/// </summary>
public class VoucherDetailDto
{
    public long TKey { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public int LineNum { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public string? AccountName { get; set; }
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
    public string? Memo { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 建立憑證 DTO
/// </summary>
public class CreateVoucherDto
{
    public string VoucherId { get; set; } = string.Empty;
    public DateTime VoucherDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string Status { get; set; } = "D";
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public List<CreateVoucherDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 建立憑證明細 DTO
/// </summary>
public class CreateVoucherDetailDto
{
    public int LineNum { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public decimal DebitAmount { get; set; }
    public decimal CreditAmount { get; set; }
    public string? Description { get; set; }
    public string? Memo { get; set; }
}

/// <summary>
/// 修改憑證 DTO
/// </summary>
public class UpdateVoucherDto
{
    public DateTime VoucherDate { get; set; }
    public string VoucherType { get; set; } = string.Empty;
    public string ShopId { get; set; } = string.Empty;
    public string Status { get; set; } = "D";
    public string? Memo { get; set; }
    public string? SiteId { get; set; }
    public string? OrgId { get; set; }
    public string CurrencyId { get; set; } = "TWD";
    public decimal ExchangeRate { get; set; } = 1;
    public List<CreateVoucherDetailDto> Details { get; set; } = new();
}

/// <summary>
/// 憑證查詢 DTO
/// </summary>
public class VoucherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? VoucherId { get; set; }
    public string? VoucherType { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
}

/// <summary>
/// 審核憑證 DTO
/// </summary>
public class ApproveVoucherDto
{
    public string ApproveUserId { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

/// <summary>
/// 取消憑證 DTO
/// </summary>
public class CancelVoucherDto
{
    public string? Memo { get; set; }
}

/// <summary>
/// 憑證檢查結果 DTO (SYSK210-SYSK230)
/// </summary>
public class VoucherCheckResultDto
{
    public string VoucherId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// 憑證處理結果 DTO (SYSK210-SYSK230)
/// </summary>
public class VoucherProcessResultDto
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public List<VoucherProcessItemDto> Results { get; set; } = new();
}

/// <summary>
/// 憑證處理項目 DTO
/// </summary>
public class VoucherProcessItemDto
{
    public int RowNum { get; set; }
    public string VoucherId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// 憑證報表查詢 DTO (SYSK310-SYSK500)
/// </summary>
public class VoucherReportQueryDto
{
    public DateTime? VoucherDateFrom { get; set; }
    public DateTime? VoucherDateTo { get; set; }
    public string? VoucherType { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 憑證統計報表 DTO
/// </summary>
public class VoucherStatisticsReportDto
{
    public VoucherStatisticsSummaryDto Summary { get; set; } = new();
    public List<VoucherStatisticsGroupDto> Groups { get; set; } = new();
}

/// <summary>
/// 憑證統計摘要 DTO
/// </summary>
public class VoucherStatisticsSummaryDto
{
    public int TotalCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalDebitAmount { get; set; }
    public decimal TotalCreditAmount { get; set; }
}

/// <summary>
/// 憑證統計群組 DTO
/// </summary>
public class VoucherStatisticsGroupDto
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// 列印憑證 DTO (SYSK210-SYSK230)
/// </summary>
public class PrintVoucherDto
{
    public List<string> VoucherIds { get; set; } = new();
    public string PrintFormat { get; set; } = "PDF";
    public string? TemplateId { get; set; }
}

/// <summary>
/// 列印結果 DTO
/// </summary>
public class PrintResultDto
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

/// <summary>
/// 匯出憑證 DTO (SYSK210-SYSK230)
/// </summary>
public class ExportVoucherDto
{
    public List<string> VoucherIds { get; set; } = new();
    public string ExportFormat { get; set; } = "EXCEL";
    public bool IncludeDetails { get; set; } = true;
}

/// <summary>
/// 批量更新憑證狀態 DTO (SYSK210-SYSK230)
/// </summary>
public class BatchUpdateVoucherStatusDto
{
    public List<string> VoucherIds { get; set; } = new();
    public string Status { get; set; } = string.Empty;
    public string? Memo { get; set; }
}

