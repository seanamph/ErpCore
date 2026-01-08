namespace ErpCore.Application.DTOs.OtherModule;

/// <summary>
/// 測試結果 DTO
/// </summary>
public class TestResultDto
{
    public long TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public string? TestData { get; set; }
    public string? TestResult { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public int? Duration { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// 執行測試請求 DTO
/// </summary>
public class ExecuteTestRequestDto
{
    public string TestName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public Dictionary<string, object>? TestData { get; set; }
}

/// <summary>
/// 執行測試回應 DTO
/// </summary>
public class ExecuteTestResponseDto
{
    public long TestId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? TestResult { get; set; }
    public string? ErrorMessage { get; set; }
    public int? Duration { get; set; }
}

/// <summary>
/// 資料庫連線測試回應 DTO
/// </summary>
public class ConnectionTestResponseDto
{
    public string Status { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? ConnectionString { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// 測試結果查詢 DTO
/// </summary>
public class TestResultQueryDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? TestType { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

