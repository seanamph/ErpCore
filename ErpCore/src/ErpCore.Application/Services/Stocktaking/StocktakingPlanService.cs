using ErpCore.Application.DTOs.Stocktaking;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Stocktaking;
using ErpCore.Infrastructure.Repositories.Stocktaking;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.Stocktaking;

/// <summary>
/// 盤點計劃服務實作
/// </summary>
public class StocktakingPlanService : BaseService, IStocktakingPlanService
{
    private readonly IStocktakingPlanRepository _repository;

    public StocktakingPlanService(
        IStocktakingPlanRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<StocktakingPlanDto>> GetStocktakingPlansAsync(StocktakingPlanQueryDto query)
    {
        try
        {
            var repositoryQuery = new StocktakingPlanQuery
            {
                PlanId = query.PlanId,
                PlanDateFrom = query.PlanDateFrom,
                PlanDateTo = query.PlanDateTo,
                PlanStatus = query.PlanStatus,
                ShopId = query.ShopId,
                SakeType = query.SakeType,
                SiteId = query.SiteId,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new StocktakingPlanDto
            {
                PlanId = x.PlanId,
                PlanDate = x.PlanDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                SakeType = x.SakeType,
                SakeDept = x.SakeDept,
                PlanStatus = x.PlanStatus,
                PlanStatusName = GetPlanStatusName(x.PlanStatus),
                SiteId = x.SiteId,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            // 載入店舖數量
            foreach (var dto in dtos)
            {
                var shops = await _repository.GetShopsByPlanIdAsync(dto.PlanId);
                dto.ShopCount = shops.Count();
            }

            return new PagedResult<StocktakingPlanDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢盤點計劃列表失敗", ex);
            throw;
        }
    }

    public async Task<StocktakingPlanDto> GetStocktakingPlanByIdAsync(string planId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            var shops = await _repository.GetShopsByPlanIdAsync(planId);
            var details = await _repository.GetDetailsByPlanIdAsync(planId);

            var dto = new StocktakingPlanDto
            {
                PlanId = entity.PlanId,
                PlanDate = entity.PlanDate,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                SakeType = entity.SakeType,
                SakeDept = entity.SakeDept,
                PlanStatus = entity.PlanStatus,
                PlanStatusName = GetPlanStatusName(entity.PlanStatus),
                SiteId = entity.SiteId,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                Shops = shops.Select(s => new StocktakingPlanShopDto
                {
                    TKey = s.TKey,
                    PlanId = s.PlanId,
                    ShopId = s.ShopId,
                    Status = s.Status,
                    StatusName = GetShopStatusName(s.Status),
                    InvStatus = s.InvStatus,
                    CreatedBy = s.CreatedBy,
                    CreatedAt = s.CreatedAt
                }).ToList(),
                Details = details.Select(d => new StocktakingDetailDto
                {
                    DetailId = d.DetailId,
                    PlanId = d.PlanId,
                    ShopId = d.ShopId,
                    GoodsId = d.GoodsId,
                    BookQty = d.BookQty,
                    PhysicalQty = d.PhysicalQty,
                    DiffQty = d.DiffQty,
                    UnitCost = d.UnitCost,
                    DiffAmount = d.DiffAmount,
                    Kind = d.Kind,
                    ShelfNo = d.ShelfNo,
                    SerialNo = d.SerialNo,
                    Notes = d.Notes,
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt
                }).ToList()
            };

            dto.ShopCount = dto.Shops.Count;
            dto.TotalDiffQty = dto.Details.Sum(d => d.DiffQty);
            dto.TotalDiffAmount = dto.Details.Sum(d => d.DiffAmount ?? 0);

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢盤點計劃失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateStocktakingPlanAsync(CreateStocktakingPlanDto dto)
    {
        try
        {
            var planId = await _repository.GeneratePlanIdAsync();

            var entity = new StocktakingPlan
            {
                PlanId = planId,
                PlanDate = dto.PlanDate,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                SakeType = dto.SakeType,
                SakeDept = dto.SakeDept,
                PlanStatus = "0",
                SiteId = dto.SiteId,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var shops = dto.ShopIds.Select(shopId => new StocktakingPlanShop
            {
                PlanId = planId,
                ShopId = shopId,
                Status = "0",
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now
            }).ToList();

            await _repository.CreateAsync(entity, shops);

            return planId;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增盤點計劃失敗", ex);
            throw;
        }
    }

    public async Task UpdateStocktakingPlanAsync(string planId, UpdateStocktakingPlanDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            if (entity.PlanStatus != "0")
            {
                throw new InvalidOperationException("只有未審核狀態的盤點計劃可以修改");
            }

            entity.PlanDate = dto.PlanDate;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.SakeType = dto.SakeType;
            entity.SakeDept = dto.SakeDept;
            entity.SiteId = dto.SiteId;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            var shops = dto.ShopIds.Select(shopId => new StocktakingPlanShop
            {
                PlanId = planId,
                ShopId = shopId,
                Status = "0",
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now
            }).ToList();

            await _repository.UpdateAsync(entity, shops);
        }
        catch (Exception ex)
        {
            _logger.LogError("修改盤點計劃失敗", ex);
            throw;
        }
    }

    public async Task DeleteStocktakingPlanAsync(string planId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            if (entity.PlanStatus != "0")
            {
                throw new InvalidOperationException("只有未審核狀態的盤點計劃可以刪除");
            }

            await _repository.DeleteAsync(planId);
        }
        catch (Exception ex)
        {
            _logger.LogError("刪除盤點計劃失敗", ex);
            throw;
        }
    }

    public async Task ApproveStocktakingPlanAsync(string planId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            if (entity.PlanStatus != "0")
            {
                throw new InvalidOperationException("只有未審核狀態的盤點計劃可以審核");
            }

            await _repository.UpdateStatusAsync(planId, "1");
        }
        catch (Exception ex)
        {
            _logger.LogError("審核盤點計劃失敗", ex);
            throw;
        }
    }

    public async Task UploadStocktakingDataAsync(string planId, IFormFile file)
    {
        try
        {
            // TODO: 實作檔案上傳邏輯
            // 解析檔案內容並寫入 StocktakingTemp 表
            throw new NotImplementedException("檔案上傳功能待實作");
        }
        catch (Exception ex)
        {
            _logger.LogError("上傳盤點資料失敗", ex);
            throw;
        }
    }

    public async Task CalculateStocktakingDiffAsync(string planId)
    {
        try
        {
            // TODO: 實作盤點差異計算邏輯
            // 從 StocktakingTemp 讀取實盤數量
            // 從庫存表讀取帳面數量
            // 計算差異並寫入 StocktakingDetails
            throw new NotImplementedException("盤點差異計算功能待實作");
        }
        catch (Exception ex)
        {
            _logger.LogError("計算盤點差異失敗", ex);
            throw;
        }
    }

    public async Task<string> ConfirmStocktakingResultAsync(string planId)
    {
        try
        {
            // TODO: 實作確認盤點結果邏輯
            // 產生庫存調整單
            throw new NotImplementedException("確認盤點結果功能待實作");
        }
        catch (Exception ex)
        {
            _logger.LogError("確認盤點結果失敗", ex);
            throw;
        }
    }

    public async Task<StocktakingReportDto> GetStocktakingReportAsync(string planId, StocktakingReportQueryDto query)
    {
        try
        {
            var plan = await _repository.GetByIdAsync(planId);
            if (plan == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            var details = await _repository.GetDetailsByPlanIdAsync(planId);

            if (!string.IsNullOrEmpty(query.ShopId))
            {
                details = details.Where(d => d.ShopId == query.ShopId);
            }

            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                details = details.Where(d => d.GoodsId == query.GoodsId);
            }

            if (!query.IncludeZero)
            {
                details = details.Where(d => d.DiffQty != 0);
            }

            var report = new StocktakingReportDto
            {
                PlanId = plan.PlanId,
                PlanDate = plan.PlanDate,
                SiteId = plan.SiteId
            };

            if (query.ReportType == "SUMMARY")
            {
                var summary = details
                    .GroupBy(d => d.ShopId)
                    .Select(g => new StocktakingReportSummaryDto
                    {
                        ShopId = g.Key,
                        ItemCount = g.Count(),
                        DiffItemCount = g.Count(d => d.DiffQty != 0),
                        TotalBookQty = g.Sum(d => d.BookQty),
                        TotalPhysicalQty = g.Sum(d => d.PhysicalQty),
                        TotalDiffQty = g.Sum(d => d.DiffQty),
                        TotalDiffAmount = g.Sum(d => d.DiffAmount ?? 0)
                    }).ToList();

                report.Summary = summary;
            }
            else
            {
                report.Details = details.Select(d => new StocktakingDetailDto
                {
                    DetailId = d.DetailId,
                    PlanId = d.PlanId,
                    ShopId = d.ShopId,
                    GoodsId = d.GoodsId,
                    BookQty = d.BookQty,
                    PhysicalQty = d.PhysicalQty,
                    DiffQty = d.DiffQty,
                    UnitCost = d.UnitCost,
                    DiffAmount = d.DiffAmount,
                    Kind = d.Kind,
                    ShelfNo = d.ShelfNo,
                    SerialNo = d.SerialNo,
                    Notes = d.Notes,
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt
                }).ToList();
            }

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢盤點報表失敗", ex);
            throw;
        }
    }

    private string GetPlanStatusName(string status)
    {
        return status switch
        {
            "-1" => "申請中",
            "0" => "未審核",
            "1" => "已審核",
            "4" => "作廢",
            "5" => "結案",
            _ => status
        };
    }

    private string GetShopStatusName(string status)
    {
        return status switch
        {
            "0" => "計劃",
            "1" => "確認",
            "2" => "盤點中",
            "3" => "計算",
            "4" => "帳面庫存",
            "5" => "作廢",
            "6" => "結案",
            "7" => "認列完成",
            _ => status
        };
    }
}
