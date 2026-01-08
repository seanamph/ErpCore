using ErpCore.Application.DTOs.InventoryCheck;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.InventoryCheck;

/// <summary>
/// 盤點計劃服務介面
/// </summary>
public interface IStocktakingPlanService
{
    Task<PagedResult<StocktakingPlanDto>> GetStocktakingPlansAsync(StocktakingPlanQueryDto query);
    Task<StocktakingPlanDto> GetStocktakingPlanByIdAsync(string planId);
    Task<string> CreateStocktakingPlanAsync(CreateStocktakingPlanDto dto);
    Task UpdateStocktakingPlanAsync(string planId, UpdateStocktakingPlanDto dto);
    Task DeleteStocktakingPlanAsync(string planId);
    Task ApproveStocktakingPlanAsync(string planId);
    Task UploadStocktakingDataAsync(string planId, IFormFile? file, List<StocktakingTempDto>? data);
    Task CalculateStocktakingDiffAsync(string planId);
    Task<string> ConfirmStocktakingResultAsync(string planId);
    Task<StocktakingReportDto> GetStocktakingReportAsync(string planId, StocktakingReportQueryDto query);
}

/// <summary>
/// 盤點暫存資料 DTO
/// </summary>
public class StocktakingTempDto
{
    public string PlanId { get; set; } = string.Empty;
    public string? SPlanId { get; set; }
    public string ShopId { get; set; } = string.Empty;
    public string GoodsId { get; set; } = string.Empty;
    public string? Kind { get; set; }
    public string? ShelfNo { get; set; }
    public int? SerialNo { get; set; }
    public decimal Qty { get; set; }
    public decimal IQty { get; set; }
}

