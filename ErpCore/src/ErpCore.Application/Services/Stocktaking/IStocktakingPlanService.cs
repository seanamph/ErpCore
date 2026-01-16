using Microsoft.AspNetCore.Http;
using ErpCore.Application.DTOs.Stocktaking;
using ErpCore.Shared.Common;

namespace ErpCore.Application.Services.Stocktaking;

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
    Task UploadStocktakingDataAsync(string planId, IFormFile file);
    Task CalculateStocktakingDiffAsync(string planId);
    Task<string> ConfirmStocktakingResultAsync(string planId);
    Task<StocktakingReportDto> GetStocktakingReportAsync(string planId, StocktakingReportQueryDto query);
}
