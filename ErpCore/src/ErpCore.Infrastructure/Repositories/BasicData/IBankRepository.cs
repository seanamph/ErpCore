using ErpCore.Domain.Entities.BasicData;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.BasicData;

/// <summary>
/// 銀行 Repository 介面
/// </summary>
public interface IBankRepository
{
    /// <summary>
    /// 根據銀行代號查詢銀行
    /// </summary>
    Task<Bank?> GetByIdAsync(string bankId);

    /// <summary>
    /// 查詢銀行列表（分頁）
    /// </summary>
    Task<PagedResult<Bank>> QueryAsync(BankQuery query);

    /// <summary>
    /// 新增銀行
    /// </summary>
    Task<Bank> CreateAsync(Bank bank);

    /// <summary>
    /// 修改銀行
    /// </summary>
    Task<Bank> UpdateAsync(Bank bank);

    /// <summary>
    /// 刪除銀行
    /// </summary>
    Task DeleteAsync(string bankId);

    /// <summary>
    /// 檢查銀行是否存在
    /// </summary>
    Task<bool> ExistsAsync(string bankId);
}

/// <summary>
/// 銀行查詢條件
/// </summary>
public class BankQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? BankId { get; set; }
    public string? BankName { get; set; }
    public string? Status { get; set; }
    public string? BankKind { get; set; }
}

