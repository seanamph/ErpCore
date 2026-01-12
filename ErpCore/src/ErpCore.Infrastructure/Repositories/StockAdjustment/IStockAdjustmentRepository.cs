using System.Data;
using ErpCore.Domain.Entities.StockAdjustment;

namespace ErpCore.Infrastructure.Repositories.StockAdjustment;

/// <summary>
/// 庫存調整單 Repository 介面
/// </summary>
public interface IStockAdjustmentRepository
{
    Task<InventoryAdjustment?> GetByIdAsync(string adjustmentId);
    Task<InventoryAdjustmentDetail?> GetDetailByIdAsync(Guid detailId);
    Task<IEnumerable<InventoryAdjustmentDetail>> GetDetailsByAdjustmentIdAsync(string adjustmentId);
    Task<IEnumerable<InventoryAdjustment>> QueryAsync(InventoryAdjustmentQuery query);
    Task<int> GetCountAsync(InventoryAdjustmentQuery query);
    Task<string> CreateAsync(InventoryAdjustment entity, List<InventoryAdjustmentDetail> details);
    Task UpdateAsync(InventoryAdjustment entity, List<InventoryAdjustmentDetail> details);
    Task DeleteAsync(string adjustmentId);
    Task UpdateStatusAsync(string adjustmentId, string status, IDbTransaction? transaction = null);
    Task<string> GenerateAdjustmentIdAsync();
    Task<IEnumerable<AdjustmentReason>> GetAdjustmentReasonsAsync();
}

/// <summary>
/// 查詢條件
/// </summary>
public class InventoryAdjustmentQuery
{
    public string? AdjustmentId { get; set; }
    public string? ShopId { get; set; }
    public string? Status { get; set; }
    public DateTime? AdjustmentDateFrom { get; set; }
    public DateTime? AdjustmentDateTo { get; set; }
    public string? AdjustmentUser { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// 調整原因
/// </summary>
public class AdjustmentReason
{
    public string ReasonId { get; set; } = string.Empty;
    public string ReasonName { get; set; } = string.Empty;
    public string? ReasonType { get; set; }
    public string Status { get; set; } = string.Empty;
}

