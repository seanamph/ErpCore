using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 參數 Repository 介面
/// </summary>
public interface IParameterRepository
{
    /// <summary>
    /// 根據參數標題和標籤查詢參數
    /// </summary>
    Task<Parameter?> GetByKeyAsync(string title, string tag);

    /// <summary>
    /// 查詢參數列表（分頁）
    /// </summary>
    Task<PagedResult<Parameter>> QueryAsync(ParameterQuery query);

    /// <summary>
    /// 根據標題查詢參數列表
    /// </summary>
    Task<List<Parameter>> GetByTitleAsync(string title);

    /// <summary>
    /// 新增參數
    /// </summary>
    Task<Parameter> CreateAsync(Parameter parameter);

    /// <summary>
    /// 修改參數
    /// </summary>
    Task<Parameter> UpdateAsync(Parameter parameter);

    /// <summary>
    /// 刪除參數
    /// </summary>
    Task DeleteAsync(string title, string tag);

    /// <summary>
    /// 檢查參數是否存在
    /// </summary>
    Task<bool> ExistsAsync(string title, string tag);
}

/// <summary>
/// 參數查詢條件
/// </summary>
public class ParameterQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? Title { get; set; }
    public string? Tag { get; set; }
    public string? Status { get; set; }
    public string? SystemId { get; set; }
}

