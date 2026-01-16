namespace ErpCore.Infrastructure.Repositories.Queries;

/// <summary>
/// 城市查詢參數（用於 Repository 層，避免依賴 Application 層）
/// </summary>
public class CityQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortField { get; set; } = "CityName";
    public string? SortOrder { get; set; } = "ASC";
    public string? CityName { get; set; }
    public string? CountryCode { get; set; }
    public string? Status { get; set; } = "1";
}

/// <summary>
/// 城市選項（用於 Repository 層）
/// </summary>
public class CityOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
