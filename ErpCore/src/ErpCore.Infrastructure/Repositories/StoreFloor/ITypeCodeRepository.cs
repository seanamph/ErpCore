using ErpCore.Domain.Entities.StoreFloor;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.StoreFloor;

/// <summary>
/// 類型代碼 Repository 介面 (SYS6405-SYS6490 - 類型代碼維護)
/// </summary>
public interface ITypeCodeRepository
{
    /// <summary>
    /// 根據主鍵查詢類型代碼
    /// </summary>
    Task<TypeCode?> GetByIdAsync(long tKey);

    /// <summary>
    /// 查詢類型代碼列表（分頁）
    /// </summary>
    Task<PagedResult<TypeCode>> QueryAsync(TypeCodeQuery query);

    /// <summary>
    /// 查詢類型代碼總數
    /// </summary>
    Task<int> GetCountAsync(TypeCodeQuery query);

    /// <summary>
    /// 新增類型代碼
    /// </summary>
    Task<TypeCode> CreateAsync(TypeCode typeCode);

    /// <summary>
    /// 修改類型代碼
    /// </summary>
    Task<TypeCode> UpdateAsync(TypeCode typeCode);

    /// <summary>
    /// 刪除類型代碼
    /// </summary>
    Task DeleteAsync(long tKey);

    /// <summary>
    /// 檢查類型代碼是否存在
    /// </summary>
    Task<bool> ExistsAsync(string typeCode, string? category);

    /// <summary>
    /// 取得類型代碼使用次數
    /// </summary>
    Task<int> GetUsageCountAsync(string typeCode, string? category);
}

/// <summary>
/// 類型代碼查詢條件
/// </summary>
public class TypeCodeQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? TypeCode { get; set; }
    public string? TypeName { get; set; }
    public string? Category { get; set; }
    public string? Status { get; set; }
}

