namespace ErpCore.Application.DTOs.InvoiceSales;

/// <summary>
/// 電子發票 DTO (SYSG210-SYSG2B0 - 電子發票列印)
/// </summary>
public class ElectronicInvoiceDto
{
    /// <summary>
    /// 主鍵
    /// </summary>
    public long TKey { get; set; }

    /// <summary>
    /// POS代碼
    /// </summary>
    public string? PosId { get; set; }

    /// <summary>
    /// 發票年月 (YYYYMM格式)
    /// </summary>
    public string InvYm { get; set; } = string.Empty;

    /// <summary>
    /// 字軌
    /// </summary>
    public string? Track { get; set; }

    /// <summary>
    /// 發票號碼起
    /// </summary>
    public string? InvNoB { get; set; }

    /// <summary>
    /// 發票號碼迄
    /// </summary>
    public string? InvNoE { get; set; }

    /// <summary>
    /// 列印條碼
    /// </summary>
    public string? PrintCode { get; set; }

    /// <summary>
    /// 發票日期
    /// </summary>
    public DateTime? InvoiceDate { get; set; }

    /// <summary>
    /// 獎項類型
    /// </summary>
    public string? PrizeType { get; set; }

    /// <summary>
    /// 獎項金額
    /// </summary>
    public decimal? PrizeAmt { get; set; }

    /// <summary>
    /// 載具識別碼（明碼）
    /// </summary>
    public string? CarrierIdClear { get; set; }

    /// <summary>
    /// 中獎列印標記
    /// </summary>
    public string? AwardPrint { get; set; }

    /// <summary>
    /// 中獎POS
    /// </summary>
    public string? AwardPos { get; set; }

    /// <summary>
    /// 中獎日期
    /// </summary>
    public DateTime? AwardDate { get; set; }

    /// <summary>
    /// 狀態 (A:啟用, I:停用)
    /// </summary>
    public string Status { get; set; } = "A";

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// 更新時間
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 電子發票查詢 DTO
/// </summary>
public class ElectronicInvoiceQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? InvYm { get; set; }
    public string? Track { get; set; }
    public string? PosId { get; set; }
    public string? PrizeType { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// 建立電子發票 DTO
/// </summary>
public class CreateElectronicInvoiceDto
{
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
    public string Status { get; set; } = "A";
}

/// <summary>
/// 修改電子發票 DTO
/// </summary>
public class UpdateElectronicInvoiceDto
{
    public long TKey { get; set; }
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
    public string Status { get; set; } = "A";
}

/// <summary>
/// 手動取號列印 DTO
/// </summary>
public class ManualPrintDto
{
    public List<long> TKeys { get; set; } = new();
    public string PrintFormat { get; set; } = "A4";
    public string? BarcodeType { get; set; }
}

/// <summary>
/// 列印資料 DTO
/// </summary>
public class PrintDataDto
{
    public string ReportId { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public byte[]? FileContent { get; set; }
}

/// <summary>
/// 中獎清冊查詢 DTO
/// </summary>
public class AwardListQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? InvYm { get; set; }
    public string? PrizeType { get; set; }
}

/// <summary>
/// 電子發票中獎 DTO
/// </summary>
public class ElectronicInvoiceAwardDto
{
    public long TKey { get; set; }
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
/// 中獎清冊列印 DTO
/// </summary>
public class AwardPrintDto
{
    public string? InvYm { get; set; }
    public string? PrizeType { get; set; }
    public string PrintFormat { get; set; } = "A4";
}

/// <summary>
/// 電子發票列印設定 DTO
/// </summary>
public class ElectronicInvoicePrintSettingDto
{
    public string SettingId { get; set; } = string.Empty;
    public string PrintFormat { get; set; } = string.Empty;
    public string? BarcodeType { get; set; }
    public int? BarcodeSize { get; set; } = 40;
    public int? BarcodeMargin { get; set; } = 5;
    public int? ColCount { get; set; } = 2;
    public int? PageCount { get; set; } = 14;
    public string Status { get; set; } = "A";
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// 更新電子發票列印設定 DTO
/// </summary>
public class UpdateElectronicInvoicePrintSettingDto
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

