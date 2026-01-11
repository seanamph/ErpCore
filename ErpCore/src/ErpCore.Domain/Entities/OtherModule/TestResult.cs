namespace ErpCore.Domain.Entities.OtherModule;

/// <summary>
/// 測試結果實體
/// </summary>
public class TestResult
{
    public long TestId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string TestType { get; set; } = string.Empty;
    public string? TestData { get; set; }
    public string? Result { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public int? Duration { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

