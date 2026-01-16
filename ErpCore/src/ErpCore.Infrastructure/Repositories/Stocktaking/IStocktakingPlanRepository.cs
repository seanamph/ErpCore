using ErpCore.Domain.Entities.Stocktaking;
using ErpCore.Shared.Common;

namespace ErpCore.Infrastructure.Repositories.Stocktaking;

/// <summary>
/// 盤點計劃 Repository 介面
/// </summary>
public interface IStocktakingPlanRepository
{
    Task<StocktakingPlan?> GetByIdAsync(string planId);
    Task<IEnumerable<StocktakingPlan>> QueryAsync(StocktakingPlanQuery query);
    Task<int> GetCountAsync(StocktakingPlanQuery query);
    Task<string> CreateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops);
    Task UpdateAsync(StocktakingPlan entity, List<StocktakingPlanShop> shops);
    Task DeleteAsync(string planId);
    Task UpdateStatusAsync(string planId, string status);
    Task<string> GeneratePlanIdAsync();
    Task<IEnumerable<StocktakingPlanShop>> GetShopsByPlanIdAsync(string planId);
    Task<IEnumerable<StocktakingDetail>> GetDetailsByPlanIdAsync(string planId);
    Task<IEnumerable<StocktakingTemp>> GetTempByPlanIdAsync(string planId);
    Task CreateDetailAsync(StocktakingDetail detail);
    Task UpdateDetailAsync(StocktakingDetail detail);
    Task DeleteDetailsByPlanIdAsync(string planId);
    Task CreateTempAsync(StocktakingTemp temp);
    Task UpdateTempAsync(StocktakingTemp temp);
    Task DeleteTempByPlanIdAsync(string planId);
}

/// <summary>
/// 盤點計劃查詢條件
/// </summary>
public class StocktakingPlanQuery : PagedQuery
{
    public string? PlanId { get; set; }
    public DateTime? PlanDateFrom { get; set; }
    public DateTime? PlanDateTo { get; set; }
    public string? PlanStatus { get; set; }
    public string? ShopId { get; set; }
    public string? SakeType { get; set; }
    public string? SiteId { get; set; }
}
