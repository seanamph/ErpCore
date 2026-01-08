using ErpCore.Domain.Entities.System;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.System;

/// <summary>
/// 可管控欄位 Repository 介面 (SYS0510)
/// </summary>
public interface IControllableFieldRepository
{
    /// <summary>
    /// 查詢可管控欄位列表
    /// </summary>
    Task<PagedResult<ControllableField>> QueryAsync(ControllableFieldQuery query);

    /// <summary>
    /// 根據ID查詢可管控欄位
    /// </summary>
    Task<ControllableField?> GetByIdAsync(string fieldId);

    /// <summary>
    /// 新增可管控欄位
    /// </summary>
    Task<ControllableField> CreateAsync(ControllableField controllableField);

    /// <summary>
    /// 修改可管控欄位
    /// </summary>
    Task<ControllableField> UpdateAsync(ControllableField controllableField);

    /// <summary>
    /// 刪除可管控欄位
    /// </summary>
    Task DeleteAsync(string fieldId);

    /// <summary>
    /// 批量刪除可管控欄位
    /// </summary>
    Task<int> BatchDeleteAsync(List<string> fieldIds);

    /// <summary>
    /// 檢查可管控欄位是否存在
    /// </summary>
    Task<bool> ExistsAsync(string fieldId);

    /// <summary>
    /// 根據資料庫和表格查詢欄位列表
    /// </summary>
    Task<List<ControllableField>> GetByDbTableAsync(string dbName, string tableName);
}

/// <summary>
/// 可管控欄位查詢條件
/// </summary>
public class ControllableFieldQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? FieldId { get; set; }
    public string? FieldName { get; set; }
    public string? DbName { get; set; }
    public string? TableName { get; set; }
    public bool? IsActive { get; set; }
}

