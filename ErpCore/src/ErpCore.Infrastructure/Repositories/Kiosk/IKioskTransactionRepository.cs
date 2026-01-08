using ErpCore.Domain.Entities.Kiosk;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Kiosk;

/// <summary>
/// Kiosk交易 Repository 介面
/// </summary>
public interface IKioskTransactionRepository
{
    /// <summary>
    /// 根據交易編號查詢
    /// </summary>
    Task<KioskTransaction?> GetByTransactionIdAsync(string transactionId);

    /// <summary>
    /// 查詢Kiosk交易列表（分頁）
    /// </summary>
    Task<PagedResult<KioskTransaction>> QueryAsync(KioskTransactionQuery query);

    /// <summary>
    /// 新增Kiosk交易
    /// </summary>
    Task<KioskTransaction> CreateAsync(KioskTransaction transaction);

    /// <summary>
    /// 修改Kiosk交易
    /// </summary>
    Task<KioskTransaction> UpdateAsync(KioskTransaction transaction);
}

/// <summary>
/// Kiosk交易查詢條件
/// </summary>
public class KioskTransactionQuery
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
    public string? KioskId { get; set; }
    public string? FunctionCode { get; set; }
    public string? CardNumber { get; set; }
    public string? MemberId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

