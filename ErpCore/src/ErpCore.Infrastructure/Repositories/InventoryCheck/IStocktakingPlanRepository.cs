using ErpCore.Domain.Entities.InventoryCheck;

namespace ErpCore.Infrastructure.Repositories.InventoryCheck;

/// <summary>
/// 盤點計劃 Repository 介面
/// </summary>
public interface IStocktakingPlanRepository
{
    Task<StocktakingPlan?> GetByIdAsync(string planId);
    Task<IEnumerable<StocktakingPlanShop>> GetShopsByPlanIdAsync(string planId);
    Task<IEnumerable<StocktakingDetail>> GetDetailsByPlanIdAsync(string planId);
    Task<IEnumerable<StocktakingTemp>> GetTempByPlanIdAsync(string planId);
    Task<IEnumerable<StocktakingPlan>> QueryAsync(StocktakingPlanQuery query);
    Task<int> GetCountAsync(StocktakingPlanQuery query);
    Task<string> CreateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops);
    Task UpdateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops);
    Task DeleteAsync(string planId);
    Task UpdateStatusAsync(string planId, string status, global::System.Data.IDbTransaction? transaction = null);
    Task<string> GeneratePlanIdAsync();
    Task BulkInsertTempAsync(List<StocktakingTemp> tempList, global::System.Data.IDbTransaction? transaction = null);
    Task<IEnumerable<StocktakingDetail>> CalculateDiffAsync(string planId, string shopId);
}

/// <summary>
/// 查詢條件
/// </summary>
public class StocktakingPlanQuery
{
    public string? PlanId { get; set; }
    public DateTime? PlanDateFrom { get; set; }
    public DateTime? PlanDateTo { get; set; }
    public string? PlanStatus { get; set; }
    public string? ShopId { get; set; }
    public string? SakeType { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

