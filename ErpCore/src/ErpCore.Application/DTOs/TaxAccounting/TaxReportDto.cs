namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 傳票查詢 DTO (SYST411)
/// </summary>
public class TaxReportVoucherQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? VoucherIdFrom { get; set; }
    public string? VoucherIdTo { get; set; }
    public List<string>? VoucherKinds { get; set; }
    public List<string>? VoucherStatuses { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? CreatedDateFrom { get; set; }
    public DateTime? CreatedDateTo { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 財務報表查詢 DTO (SYST421)
/// </summary>
public class FinancialReportQueryDto
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public List<string>? StypeIds { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 稅務統計報表查詢 DTO (SYST452)
/// </summary>
public class TaxStatisticsQueryDto
{
    public string ReportType { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? SiteId { get; set; }
}

/// <summary>
/// 列印傳票 DTO
/// </summary>
public class PrintVoucherDto
{
    public List<string> VoucherIds { get; set; } = new();
    public string PrintType { get; set; } = "2"; // 1:直印, 2:一般, 3:橫印
    public string PrintSig { get; set; } = "N";
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

