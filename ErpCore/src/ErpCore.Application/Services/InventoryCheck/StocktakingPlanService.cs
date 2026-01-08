using System.Data;
using ErpCore.Application.DTOs.InventoryCheck;
using ErpCore.Application.DTOs.StockAdjustment;
using ErpCore.Application.Services.Base;
using ErpCore.Application.Services.StockAdjustment;
using ErpCore.Domain.Entities.InventoryCheck;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Infrastructure.Repositories.InventoryCheck;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Microsoft.AspNetCore.Http;

namespace ErpCore.Application.Services.InventoryCheck;

/// <summary>
/// 盤點計劃服務實作
/// </summary>
public class StocktakingPlanService : BaseService, IStocktakingPlanService
{
    private readonly IStocktakingPlanRepository _repository;
    private readonly IStockRepository _stockRepository;
    private readonly IStockAdjustmentService _stockAdjustmentService;
    private readonly IDbConnectionFactory _connectionFactory;

    public StocktakingPlanService(
        IStocktakingPlanRepository repository,
        IStockRepository stockRepository,
        IStockAdjustmentService stockAdjustmentService,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _stockRepository = stockRepository;
        _stockAdjustmentService = stockAdjustmentService;
        _connectionFactory = connectionFactory;
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
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = new List<StocktakingPlanDto>();
            foreach (var item in items)
            {
                var shops = await _repository.GetShopsByPlanIdAsync(item.PlanId);
                var details = await _repository.GetDetailsByPlanIdAsync(item.PlanId);
                dtos.Add(MapToDto(item, shops, details));
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
            return MapToDto(entity, shops, details);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢盤點計劃失敗: {planId}", ex);
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
                PlanStatus = "0", // 未審核
                SiteId = dto.SiteId,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var shops = dto.ShopIds.Select(shopId => new StocktakingPlanShop
            {
                PlanId = planId,
                ShopId = shopId,
                Status = "0", // 計劃
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            }).ToList();

            await _repository.CreateAsync(entity, shops);
            _logger.LogInfo($"建立盤點計劃成功: {planId}");
            return planId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立盤點計劃失敗", ex);
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
                throw new InvalidOperationException("僅未審核狀態的盤點計劃可修改");
            }

            entity.PlanDate = dto.PlanDate;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.SakeType = dto.SakeType;
            entity.SakeDept = dto.SakeDept;
            entity.SiteId = dto.SiteId;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var shops = dto.ShopIds.Select(shopId => new StocktakingPlanShop
            {
                PlanId = planId,
                ShopId = shopId,
                Status = "0",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            }).ToList();

            await _repository.UpdateAsync(entity, shops);
            _logger.LogInfo($"更新盤點計劃成功: {planId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新盤點計劃失敗: {planId}", ex);
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
                throw new InvalidOperationException("僅未審核狀態的盤點計劃可刪除");
            }

            await _repository.DeleteAsync(planId);
            _logger.LogInfo($"刪除盤點計劃成功: {planId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除盤點計劃失敗: {planId}", ex);
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
                throw new InvalidOperationException("僅未審核狀態的盤點計劃可審核");
            }

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                await _repository.UpdateStatusAsync(planId, "1", transaction); // 已審核

                // 更新店舖狀態為確認
                const string sql = @"
                    UPDATE StocktakingPlanShops 
                    SET Status = '1' 
                    WHERE PlanId = @PlanId";

                await transaction.Connection!.ExecuteAsync(sql, new { PlanId = planId }, transaction);

                transaction.Commit();
                _logger.LogInfo($"審核盤點計劃成功: {planId}");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核盤點計劃失敗: {planId}", ex);
            throw;
        }
    }

    public async Task UploadStocktakingDataAsync(string planId, IFormFile? file, List<StocktakingTempDto>? data)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            if (entity.PlanStatus != "1")
            {
                throw new InvalidOperationException("僅已審核狀態的盤點計劃可上傳資料");
            }

            var tempList = new List<StocktakingTemp>();

            if (file != null)
            {
                // 解析檔案內容（CSV、Excel等）
                using var stream = file.OpenReadStream();
                var fileData = await FileParser.ParseStocktakingFileAsync(stream, file.FileName);
                
                if (fileData.Any())
                {
                    tempList.AddRange(fileData.Select(x => new StocktakingTemp
                    {
                        PlanId = planId,
                        SPlanId = x.SPlanId,
                        ShopId = x.ShopId,
                        GoodsId = x.GoodsId,
                        Kind = x.Kind,
                        ShelfNo = x.ShelfNo,
                        SerialNo = x.SerialNo,
                        Qty = x.Qty,
                        IQty = x.IQty,
                        IsAdd = "N",
                        HtStatus = "0", // 未審核
                        CreatedBy = GetCurrentUserId(),
                        CreatedAt = DateTime.Now
                    }));
                }
            }

            if (data != null && data.Any())
            {
                tempList = data.Select(x => new StocktakingTemp
                {
                    PlanId = planId,
                    SPlanId = x.SPlanId,
                    ShopId = x.ShopId,
                    GoodsId = x.GoodsId,
                    Kind = x.Kind,
                    ShelfNo = x.ShelfNo,
                    SerialNo = x.SerialNo,
                    Qty = x.Qty,
                    IQty = x.IQty,
                    IsAdd = "N",
                    HtStatus = "0", // 未審核
                    CreatedBy = GetCurrentUserId(),
                    CreatedAt = DateTime.Now
                }).ToList();
            }

            if (tempList.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                using var transaction = connection.BeginTransaction();

                try
                {
                    await _repository.BulkInsertTempAsync(tempList, transaction);
                    transaction.Commit();
                    _logger.LogInfo($"上傳盤點資料成功: {planId}, 筆數: {tempList.Count}");
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"上傳盤點資料失敗: {planId}", ex);
            throw;
        }
    }

    public async Task CalculateStocktakingDiffAsync(string planId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            var shops = await _repository.GetShopsByPlanIdAsync(planId);
            if (!shops.Any())
            {
                throw new InvalidOperationException($"盤點計劃無店舖資料: {planId}");
            }

            using var connection = _connectionFactory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 刪除舊的明細資料
                const string sqlDelete = @"
                    DELETE FROM StocktakingDetails 
                    WHERE PlanId = @PlanId";

                await transaction.Connection!.ExecuteAsync(sqlDelete, new { PlanId = planId }, transaction);

                // 計算每個店舖的差異
                var allDetails = new List<StocktakingDetail>();
                foreach (var shop in shops)
                {
                    var details = await _repository.CalculateDiffAsync(planId, shop.ShopId);
                    allDetails.AddRange(details);
                }

                // 批次新增明細
                if (allDetails.Any())
                {
                    const string sqlInsert = @"
                        INSERT INTO StocktakingDetails (
                            DetailId, PlanId, ShopId, GoodsId,
                            BookQty, PhysicalQty, DiffQty, UnitCost, DiffAmount,
                            Kind, ShelfNo, SerialNo, Notes,
                            CreatedBy, CreatedAt
                        ) VALUES (
                            @DetailId, @PlanId, @ShopId, @GoodsId,
                            @BookQty, @PhysicalQty, @DiffQty, @UnitCost, @DiffAmount,
                            @Kind, @ShelfNo, @SerialNo, @Notes,
                            @CreatedBy, @CreatedAt
                        )";

                    await transaction.Connection!.ExecuteAsync(sqlInsert, allDetails, transaction);
                }

                // 更新店舖狀態為計算完成
                const string sqlUpdate = @"
                    UPDATE StocktakingPlanShops 
                    SET Status = '3' 
                    WHERE PlanId = @PlanId";

                await transaction.Connection!.ExecuteAsync(sqlUpdate, new { PlanId = planId }, transaction);

                transaction.Commit();
                _logger.LogInfo($"計算盤點差異成功: {planId}");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"計算盤點差異失敗: {planId}", ex);
            throw;
        }
    }

    public async Task<string> ConfirmStocktakingResultAsync(string planId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            var details = await _repository.GetDetailsByPlanIdAsync(planId);
            var detailsList = details.ToList();
            if (!detailsList.Any())
            {
                throw new InvalidOperationException($"盤點計劃無明細資料: {planId}");
            }

            // 產生庫存調整單
            string adjustmentId;
            try
            {
                var plan = await _repository.GetByIdAsync(planId);
                if (plan == null)
                {
                    throw new InvalidOperationException($"盤點計劃不存在: {planId}");
                }

                // 建立庫存調整單 DTO
                var adjustmentDto = new CreateInventoryAdjustmentDto
                {
                    AdjustmentDate = DateTime.Now,
                    ShopId = plan.ShopId,
                    AdjustmentType = "STOCKTAKING", // 盤點調整
                    AdjustmentUser = GetCurrentUserId(),
                    Memo = $"盤點計劃: {planId}",
                    SourceNo = planId,
                    SourceNum = plan.PlanId,
                    SourceCheckDate = plan.PlanDate,
                    Details = detailsList.Select(d => new CreateInventoryAdjustmentDetailDto
                    {
                        GoodsId = d.GoodsId,
                        BarcodeId = d.BarcodeId,
                        AdjustmentQty = d.DifferenceQty, // 差異數量
                        UnitCost = d.UnitCost,
                        Reason = "盤點差異",
                        Memo = $"盤點計劃: {planId}"
                    }).ToList()
                };

                adjustmentId = await _stockAdjustmentService.CreateInventoryAdjustmentAsync(adjustmentDto);
                _logger.LogInfo($"產生庫存調整單成功: PlanId={planId}, AdjustmentId={adjustmentId}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"產生庫存調整單失敗: PlanId={planId}", ex);
                throw new InvalidOperationException($"產生庫存調整單失敗: {ex.Message}", ex);
            }

            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // 更新計劃狀態為結案
                await _repository.UpdateStatusAsync(planId, "5", transaction); // 結案

                // 更新店舖狀態為結案
                const string sql = @"
                    UPDATE StocktakingPlanShops 
                    SET Status = '6' 
                    WHERE PlanId = @PlanId";

                await transaction.Connection!.ExecuteAsync(sql, new { PlanId = planId }, transaction);

                transaction.Commit();
                _logger.LogInfo($"確認盤點結果成功: PlanId={planId}, AdjustmentId={adjustmentId}");
                return adjustmentId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"確認盤點結果失敗: {planId}", ex);
            throw;
        }
    }

    public async Task<StocktakingReportDto> GetStocktakingReportAsync(string planId, StocktakingReportQueryDto query)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(planId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"盤點計劃不存在: {planId}");
            }

            var details = await _repository.GetDetailsByPlanIdAsync(planId);
            var shops = await _repository.GetShopsByPlanIdAsync(planId);

            // 篩選明細
            var filteredDetails = details.AsEnumerable();
            if (!string.IsNullOrEmpty(query.ShopId))
            {
                filteredDetails = filteredDetails.Where(x => x.ShopId == query.ShopId);
            }
            if (!string.IsNullOrEmpty(query.GoodsId))
            {
                filteredDetails = filteredDetails.Where(x => x.GoodsId == query.GoodsId);
            }
            if (!query.IncludeZero)
            {
                filteredDetails = filteredDetails.Where(x => x.DiffQty != 0);
            }

            var detailList = filteredDetails.ToList();

            var report = new StocktakingReportDto
            {
                PlanId = planId,
                PlanDate = entity.PlanDate,
                Details = detailList.Select(x => new StocktakingDetailDto
                {
                    DetailId = x.DetailId,
                    PlanId = x.PlanId,
                    ShopId = x.ShopId,
                    GoodsId = x.GoodsId,
                    BookQty = x.BookQty,
                    PhysicalQty = x.PhysicalQty,
                    DiffQty = x.DiffQty,
                    UnitCost = x.UnitCost,
                    DiffAmount = x.DiffAmount,
                    Kind = x.Kind,
                    ShelfNo = x.ShelfNo,
                    SerialNo = x.SerialNo,
                    Notes = x.Notes,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };

            if (query.ReportType == "SUMMARY")
            {
                // 彙總報表
                var summary = detailList
                    .GroupBy(x => x.ShopId)
                    .Select(g => new StocktakingReportSummaryDto
                    {
                        ShopId = g.Key,
                        GoodsCount = g.Count(),
                        TotalBookQty = g.Sum(x => x.BookQty),
                        TotalPhysicalQty = g.Sum(x => x.PhysicalQty),
                        TotalDiffQty = g.Sum(x => x.DiffQty),
                        TotalDiffAmount = g.Sum(x => x.DiffAmount ?? 0)
                    }).ToList();

                report.Summary = summary;
            }

            return report;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢盤點報表失敗: {planId}", ex);
            throw;
        }
    }

    private StocktakingPlanDto MapToDto(StocktakingPlan entity, IEnumerable<StocktakingPlanShop> shops, IEnumerable<StocktakingDetail> details)
    {
        var shopList = shops.ToList();
        var detailList = details.ToList();

        return new StocktakingPlanDto
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
            Shops = shopList.Select(x => new StocktakingPlanShopDto
            {
                TKey = x.TKey,
                PlanId = x.PlanId,
                ShopId = x.ShopId,
                Status = x.Status,
                StatusName = GetShopStatusName(x.Status),
                InvStatus = x.InvStatus,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList(),
            Details = detailList.Select(x => new StocktakingDetailDto
            {
                DetailId = x.DetailId,
                PlanId = x.PlanId,
                ShopId = x.ShopId,
                GoodsId = x.GoodsId,
                BookQty = x.BookQty,
                PhysicalQty = x.PhysicalQty,
                DiffQty = x.DiffQty,
                UnitCost = x.UnitCost,
                DiffAmount = x.DiffAmount,
                Kind = x.Kind,
                ShelfNo = x.ShelfNo,
                SerialNo = x.SerialNo,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList(),
            ShopCount = shopList.Count,
            TotalDiffQty = detailList.Sum(x => x.DiffQty),
            TotalDiffAmount = detailList.Sum(x => x.DiffAmount ?? 0)
        };
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

