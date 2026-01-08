namespace ErpCore.Application.DTOs.TaxAccounting;

/// <summary>
/// 傳票轉入記錄 DTO
/// </summary>
public class VoucherImportLogDto
{
    public long TKey { get; set; }
    public string ImportType { get; set; } = string.Empty;
    public string? FileName { get; set; }
    public string? FilePath { get; set; }
    public DateTime ImportDate { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int SkipCount { get; set; }
    public string Status { get; set; } = "P";
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 傳票轉入記錄查詢 DTO
/// </summary>
public class VoucherImportLogQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ImportType { get; set; }
    public string? Status { get; set; }
    public DateTime? ImportDateFrom { get; set; }
    public DateTime? ImportDateTo { get; set; }
    public string? FileName { get; set; }
}

/// <summary>
/// 傳票轉入明細 DTO
/// </summary>
public class VoucherImportDetailDto
{
    public long TKey { get; set; }
    public int? RowNumber { get; set; }
    public long? VoucherTKey { get; set; }
    public string Status { get; set; } = "P";
    public string? ErrorMessage { get; set; }
    public string? SourceData { get; set; }
}

/// <summary>
/// 傳票轉入記錄明細查詢結果 DTO
/// </summary>
public class VoucherImportLogDetailDto
{
    public VoucherImportLogDto ImportLog { get; set; } = new();
    public List<VoucherImportDetailDto> Details { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// 傳票轉入明細查詢 DTO
/// </summary>
public class VoucherImportDetailQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Status { get; set; }
}

/// <summary>
/// 日立傳票轉入 DTO
/// </summary>
public class ImportHtvDto
{
    public bool ValidateOnly { get; set; } = false;
}

/// <summary>
/// 轉入結果 DTO
/// </summary>
public class ImportResultDto
{
    public long ImportLogTKey { get; set; }
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int SkipCount { get; set; }
}

/// <summary>
/// 轉入進度 DTO
/// </summary>
public class ImportProgressDto
{
    public long TKey { get; set; }
    public string Status { get; set; } = "P";
    public int TotalCount { get; set; }
    public int ProcessedCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int SkipCount { get; set; }
    public double Progress { get; set; }
}

