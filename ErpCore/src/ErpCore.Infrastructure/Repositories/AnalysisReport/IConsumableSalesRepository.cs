using ErpCore.Domain.Entities.AnalysisReport;
using ErpCore.Shared.Common;
using System.Data;

namespace ErpCore.Infrastructure.Repositories.AnalysisReport;

/// <summary>
/// 耗材出售單 Repository 介面 (SYSA297)
/// </summary>
public interface IConsumableSalesRepository
{
    /// <summary>
    /// 查詢耗材出售單列表
    /// </summary>
    Task<PagedResult<ConsumableSales>> GetSalesAsync(ConsumableSalesQuery query);

    /// <summary>
    /// 根據交易單號查詢耗材出售單
    /// </summary>
    Task<ConsumableSales?> GetSalesByTxnNoAsync(string txnNo);

    /// <summary>
    /// 新增耗材出售單
    /// </summary>
    Task<string> CreateSalesAsync(ConsumableSales sales);

    /// <summary>
    /// 更新耗材出售單
    /// </summary>
    Task UpdateSalesAsync(ConsumableSales sales);

    /// <summary>
    /// 刪除耗材出售單
    /// </summary>
    Task DeleteSalesAsync(string txnNo);

    /// <summary>
    /// 檢查交易單號是否存在
    /// </summary>
    Task<bool> ExistsAsync(string txnNo);

    /// <summary>
    /// 生成交易單號
    /// </summary>
    Task<string> GenerateTxnNoAsync(string siteId, DateTime date);

    /// <summary>
    /// 取得耗材庫存數量
    /// </summary>
    Task<decimal> GetInventoryQuantityAsync(string consumableId, string siteId);

    /// <summary>
    /// 更新耗材庫存數量
    /// </summary>
    Task UpdateInventoryQuantityAsync(string consumableId, string siteId, decimal quantityChange, System.Data.IDbTransaction? transaction = null);
}

/// <summary>
/// 耗材出售單查詢條件
/// </summary>
public class ConsumableSalesQuery : PagedQuery
{
    public string? TxnNo { get; set; }
    public string? SiteId { get; set; }
    public string? Status { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SortField { get; set; }
    public string? SortOrder { get; set; }
}
