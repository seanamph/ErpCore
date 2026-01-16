namespace ErpCore.Infrastructure.Repositories.Queries;

/// <summary>
/// 選單查詢參數（用於 Repository 層，避免依賴 Application 層）
/// </summary>
public class MenuQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? SortField { get; set; } = "MenuName";
    public string? SortOrder { get; set; } = "ASC";
    public string? MenuName { get; set; }
    public string? SystemId { get; set; }
    public string? Status { get; set; } = "1";
}

/// <summary>
/// 選單選項（用於 Repository 層）
/// </summary>
public class MenuOption
{
    public string Value { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
}
