using ErpCore.Domain.Entities.UiComponent;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.UiComponent;

/// <summary>
/// UI組件儲存庫介面
/// </summary>
public interface IUIComponentRepository
{
    Task<UIComponent?> GetByIdAsync(long componentId);
    Task<UIComponent?> GetByCodeAndVersionAsync(string componentCode, string componentVersion);
    Task<PagedResult<UIComponent>> QueryAsync(UIComponentQuery query);
    Task<UIComponent> CreateAsync(UIComponent entity);
    Task<bool> UpdateAsync(UIComponent entity);
    Task<bool> DeleteAsync(long componentId);
    Task<List<UIComponentUsage>> GetUsagesAsync(long componentId);
    Task<UIComponentUsage> CreateUsageAsync(UIComponentUsage usage);
    Task<bool> UpdateUsageAsync(UIComponentUsage usage);
}

/// <summary>
/// UI組件查詢參數
/// </summary>
public class UIComponentQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? ComponentCode { get; set; }
    public string? ComponentType { get; set; }
    public string? ComponentVersion { get; set; }
    public string? Status { get; set; }
}

