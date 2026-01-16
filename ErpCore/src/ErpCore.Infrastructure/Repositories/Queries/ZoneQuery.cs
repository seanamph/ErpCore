namespace ErpCore.Infrastructure.Repositories.Queries;

/// <summary>
/// 區域查詢參數（用於 Repository 層，避免依賴 Application 層）
/// </summary>
public class ZoneQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortField { get; set; } = "ZoneName";
    public string? SortOrder { get; set; } = "ASC";
    public string? ZoneName { get; set; }
    public string? CityId { get; set; }
    public string? ZipCode { get; set; }
    public string? Status { get; set; } = "1";
}

/// <summary>
/// 區域選項（用於 Repository 層）
/// </summary>
public class ZoneOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? ZipCode { get; set; }
}
