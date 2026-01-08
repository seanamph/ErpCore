namespace ErpCore.Application.DTOs.Kiosk;

/// <summary>
/// Kiosk交易資料傳輸物件
/// </summary>
public class KioskTransactionDto
{
    public long Id { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string KioskId { get; set; } = string.Empty;
    public string FunctionCode { get; set; } = string.Empty;
    public string? FunctionCodeName { get; set; }
    public string? CardNumber { get; set; }
    public string? MemberId { get; set; }
    public DateTime TransactionDate { get; set; }
    public string? RequestData { get; set; }
    public string? ResponseData { get; set; }
    public string Status { get; set; } = "Success";
    public string? ReturnCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Kiosk交易查詢 DTO
/// </summary>
public class KioskTransactionQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string SortOrder { get; set; } = "DESC";
    public KioskTransactionFilters? Filters { get; set; }
}

/// <summary>
/// Kiosk交易查詢篩選條件
/// </summary>
public class KioskTransactionFilters
{
    public string? KioskId { get; set; }
    public string? FunctionCode { get; set; }
    public string? CardNumber { get; set; }
    public string? MemberId { get; set; }
    public string? Status { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
}

/// <summary>
/// Kiosk處理請求 DTO
/// </summary>
public class KioskProcessRequestDto
{
    public string FunCmdId { get; set; } = string.Empty;
    public string KioskId { get; set; } = string.Empty;
    public string? LoyalSysCard { get; set; }
    public string? SysCardPWD { get; set; }
    public string? Pid { get; set; }
    public string? Pnm { get; set; }
    public Dictionary<string, object>? OtherData { get; set; }
}

/// <summary>
/// Kiosk處理回應 DTO
/// </summary>
public class KioskProcessResponseDto
{
    public string ReturnCode { get; set; } = string.Empty;
    public string ReturnMessage { get; set; } = string.Empty;
    public Dictionary<string, object>? ResponseData { get; set; }
    public string TransactionId { get; set; } = string.Empty;
}

/// <summary>
/// Kiosk統計查詢 DTO
/// </summary>
public class KioskStatisticsQueryDto
{
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? KioskId { get; set; }
    public string? FunctionCode { get; set; }
    public string GroupBy { get; set; } = "Date"; // Date, Kiosk, FunctionCode
}

/// <summary>
/// Kiosk統計資料 DTO
/// </summary>
public class KioskStatisticsDto
{
    public string GroupKey { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public decimal SuccessRate { get; set; }
}

/// <summary>
/// Kiosk功能代碼統計 DTO
/// </summary>
public class KioskFunctionStatisticsDto
{
    public string FunctionCode { get; set; } = string.Empty;
    public string FunctionCodeName { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }
    public decimal SuccessRate { get; set; }
}

/// <summary>
/// Kiosk錯誤分析 DTO
/// </summary>
public class KioskErrorAnalysisDto
{
    public string ReturnCode { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}

/// <summary>
/// Kiosk報表匯出 DTO
/// </summary>
public class KioskReportExportDto
{
    public string ExportType { get; set; } = "Excel"; // Excel, PDF
    public KioskTransactionFilters? Filters { get; set; }
}

