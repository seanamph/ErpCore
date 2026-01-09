namespace ErpCore.Application.DTOs.InvoiceSalesB2B;

/// <summary>
/// B2B電子發票 DTO (SYSG000_B2B - B2B電子發票列印)
/// </summary>
public class B2BElectronicInvoiceDto
{
    public long TKey { get; set; }
    public string InvoiceId { get; set; } = string.Empty;
    public string? PosId { get; set; }
    public string InvYm { get; set; } = string.Empty;
    public string? Track { get; set; }
    public string? InvNoB { get; set; }
    public string? InvNoE { get; set; }
    public string? PrintCode { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? PrizeType { get; set; }
    public decimal? PrizeAmt { get; set; }
    public string? CarrierIdClear { get; set; }
    public string? AwardPrint { get; set; }
    public string? AwardPos { get; set; }
    public DateTime? AwardDate { get; set; }
    public string B2BFlag { get; set; } = "Y";
    public string? TransferType { get; set; }
    public string? TransferStatus { get; set; }
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// B2B電子發票查詢 DTO
/// </summary>
public class B2BElectronicInvoiceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvoiceId { get; set; }
    public string? PosId { get; set; }
    public string? InvYm { get; set; }
    public string? Track { get; set; }
    public string? PrizeType { get; set; }
    public string? TransferType { get; set; }
    public string? Status { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

/// <summary>
/// 建立B2B電子發票 DTO
/// </summary>
public class CreateB2BElectronicInvoiceDto
{
    public string InvoiceId { get; set; } = string.Empty;
    public string? PosId { get; set; }
    public string InvYm { get; set; } = string.Empty;
    public string? Track { get; set; }
    public string? InvNoB { get; set; }
    public string? InvNoE { get; set; }
    public string? PrintCode { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? PrizeType { get; set; }
    public decimal? PrizeAmt { get; set; }
    public string? CarrierIdClear { get; set; }
    public string? AwardPrint { get; set; }
    public string? AwardPos { get; set; }
    public DateTime? AwardDate { get; set; }
    public string? TransferType { get; set; }
    public string? TransferStatus { get; set; }
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改B2B電子發票 DTO
/// </summary>
public class UpdateB2BElectronicInvoiceDto : CreateB2BElectronicInvoiceDto
{
    public long TKey { get; set; }
}

/// <summary>
/// B2B電子發票中獎清冊查詢 DTO
/// </summary>
public class B2BAwardListQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? InvYm { get; set; }
    public string? PrizeType { get; set; }
    public string? B2BFlag { get; set; } = "Y";
}

/// <summary>
/// B2B電子發票中獎清冊 DTO
/// </summary>
public class B2BElectronicInvoiceAwardDto
{
    public long TKey { get; set; }
    public string InvoiceId { get; set; } = string.Empty;
    public string? PosId { get; set; }
    public string InvYm { get; set; } = string.Empty;
    public string? Track { get; set; }
    public string? InvNoB { get; set; }
    public string? InvNoE { get; set; }
    public string? PrintCode { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? PrizeType { get; set; }
    public decimal? PrizeAmt { get; set; }
    public string? CarrierIdClear { get; set; }
    public string? AwardPrint { get; set; }
    public string? AwardPos { get; set; }
    public DateTime? AwardDate { get; set; }
}

/// <summary>
/// B2B電子發票列印設定 DTO
/// </summary>
public class B2BElectronicInvoicePrintSettingDto
{
    public string SettingId { get; set; } = string.Empty;
    public string PrintFormat { get; set; } = string.Empty;
    public string? BarcodeType { get; set; }
    public int? BarcodeSize { get; set; } = 40;
    public int? BarcodeMargin { get; set; } = 5;
    public int? ColCount { get; set; } = 2;
    public int? PageCount { get; set; } = 14;
    public string B2BFlag { get; set; } = "Y";
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 更新B2B電子發票列印設定 DTO
/// </summary>
public class UpdateB2BElectronicInvoicePrintSettingDto
{
    public string SettingId { get; set; } = string.Empty;
    public string PrintFormat { get; set; } = string.Empty;
    public string? BarcodeType { get; set; }
    public int? BarcodeSize { get; set; } = 40;
    public int? BarcodeMargin { get; set; } = 5;
    public int? ColCount { get; set; } = 2;
    public int? PageCount { get; set; } = 14;
    public string Status { get; set; } = "A";
}

/// <summary>
/// B2B電子發票手動列印 DTO
/// </summary>
public class B2BManualPrintDto
{
    public List<long> TKeys { get; set; } = new();
    public string PrintFormat { get; set; } = "A4";
}

/// <summary>
/// B2B電子發票中獎清冊列印 DTO
/// </summary>
public class B2BAwardPrintDto
{
    public string? InvYm { get; set; }
    public string? PrizeType { get; set; }
    public string PrintFormat { get; set; } = "A4";
}

/// <summary>
/// 列印資料 DTO
/// </summary>
public class B2BPrintDataDto
{
    public string ReportId { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
}

