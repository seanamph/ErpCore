namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// SAP銀行往來查詢 DTO (SYST510)
/// </summary>
public class SapBankTotalQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? CompId { get; set; }
    public string? SapStypeId { get; set; }
}

/// <summary>
/// SAP銀行往來 DTO
/// </summary>
public class SapBankTotalDto
{
    public long TKey { get; set; }
    public string SapDate { get; set; } = string.Empty;
    public string? SapStypeId { get; set; }
    public string CompId { get; set; } = string.Empty;
    public decimal BankAmt { get; set; }
    public decimal BankBalance { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// 產生CSV DTO
/// </summary>
public class GenerateCsvDto
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public string CompId { get; set; } = string.Empty;
}

/// <summary>
/// CSV檔案 DTO
/// </summary>
public class CsvFileDto
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
}

/// <summary>
/// 稅務報表列印記錄 DTO
/// </summary>
public class TaxReportPrintDto
{
    public long TKey { get; set; }
    public string ReportType { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? CompId { get; set; }
    public string? FileName { get; set; }
    public string? FileFormat { get; set; }
    public string PrintStatus { get; set; } = string.Empty;
    public int PrintCount { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 稅務報表列印記錄查詢 DTO
/// </summary>
public class TaxReportPrintQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? ReportType { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? PrintStatus { get; set; }
}

/// <summary>
/// 新增稅務報表列印記錄 DTO
/// </summary>
public class CreateTaxReportPrintDto
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime ReportDate { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? CompId { get; set; }
    public string? FileName { get; set; }
    public string? FileFormat { get; set; }
    public string PrintStatus { get; set; } = "1";
    public string? Notes { get; set; }
}

/// <summary>
/// 修改稅務報表列印記錄 DTO
/// </summary>
public class UpdateTaxReportPrintDto
{
    public DateTime ReportDate { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? CompId { get; set; }
    public string? FileName { get; set; }
    public string? FileFormat { get; set; }
    public string PrintStatus { get; set; } = "1";
    public string? Notes { get; set; }
}

