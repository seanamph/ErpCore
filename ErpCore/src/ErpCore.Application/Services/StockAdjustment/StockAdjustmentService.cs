using System.Data;
using ErpCore.Application.DTOs.StockAdjustment;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StockAdjustment;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Infrastructure.Repositories.StockAdjustment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using ErpCore.Application.Common;

namespace ErpCore.Application.Services.StockAdjustment;

/// <summary>
/// 庫存調整單服務實作
/// </summary>
public class StockAdjustmentService : BaseService, IStockAdjustmentService
{
    private readonly IStockAdjustmentRepository _repository;
    private readonly IStockRepository _stockRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public StockAdjustmentService(
        IStockAdjustmentRepository repository,
        IStockRepository stockRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _stockRepository = stockRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<InventoryAdjustmentDto>> GetInventoryAdjustmentsAsync(InventoryAdjustmentQueryDto query)
    {
        try
        {
            var repositoryQuery = new InventoryAdjustmentQuery
            {
                AdjustmentId = query.AdjustmentId,
                ShopId = query.ShopId,
                Status = query.Status,
                AdjustmentDateFrom = query.AdjustmentDateFrom,
                AdjustmentDateTo = query.AdjustmentDateTo,
                AdjustmentUser = query.AdjustmentUser,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = new List<InventoryAdjustmentDto>();
            foreach (var item in items)
            {
                var details = await _repository.GetDetailsByAdjustmentIdAsync(item.AdjustmentId);
                dtos.Add(MapToDto(item, details));
            }

            return new PagedResult<InventoryAdjustmentDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢庫存調整單失敗", ex);
            throw;
        }
    }

    public async Task<InventoryAdjustmentDto> GetInventoryAdjustmentByIdAsync(string adjustmentId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(adjustmentId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"調整單不存在: {adjustmentId}");
            }

            var details = await _repository.GetDetailsByAdjustmentIdAsync(adjustmentId);
            return MapToDto(entity, details);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢庫存調整單失敗: {adjustmentId}", ex);
            throw;
        }
    }

    public async Task<string> CreateInventoryAdjustmentAsync(CreateInventoryAdjustmentDto dto)
    {
        try
        {
            var entity = new InventoryAdjustment
            {
                AdjustmentDate = dto.AdjustmentDate,
                ShopId = dto.ShopId,
                Status = "D",
                AdjustmentType = dto.AdjustmentType,
                AdjustmentUser = dto.AdjustmentUser,
                Memo = dto.Memo,
                Memo2 = dto.Memo2,
                SourceNo = dto.SourceNo,
                SourceNum = dto.SourceNum,
                SourceCheckDate = dto.SourceCheckDate,
                SourceSuppId = dto.SourceSuppId,
                SiteId = dto.SiteId,
                CreatedBy = dto.AdjustmentUser
            };

            var details = dto.Details.Select((x, index) => new InventoryAdjustmentDetail
            {
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                AdjustmentQty = x.AdjustmentQty,
                UnitCost = x.UnitCost,
                AdjustmentCost = (x.UnitCost ?? 0) * x.AdjustmentQty,
                AdjustmentAmount = (x.UnitCost ?? 0) * x.AdjustmentQty,
                Reason = x.Reason,
                Memo = x.Memo,
                CreatedBy = dto.AdjustmentUser
            }).ToList();

            entity.TotalQty = details.Sum(x => x.AdjustmentQty);
            entity.TotalCost = details.Sum(x => x.AdjustmentCost ?? 0);
            entity.TotalAmount = details.Sum(x => x.AdjustmentAmount ?? 0);

            var adjustmentId = await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立庫存調整單成功: {adjustmentId}");
            return adjustmentId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立庫存調整單失敗", ex);
            throw;
        }
    }

    public async Task UpdateInventoryAdjustmentAsync(string adjustmentId, UpdateInventoryAdjustmentDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(adjustmentId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"調整單不存在: {adjustmentId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅草稿狀態的調整單可修改");
            }

            entity.AdjustmentDate = dto.AdjustmentDate;
            entity.AdjustmentType = dto.AdjustmentType;
            entity.AdjustmentUser = dto.AdjustmentUser;
            entity.Memo = dto.Memo;
            entity.Memo2 = dto.Memo2;
            entity.SourceNo = dto.SourceNo;
            entity.SourceNum = dto.SourceNum;
            entity.SourceCheckDate = dto.SourceCheckDate;
            entity.SourceSuppId = dto.SourceSuppId;
            entity.SiteId = dto.SiteId;
            entity.UpdatedBy = dto.AdjustmentUser;

            var existingDetails = await _repository.GetDetailsByAdjustmentIdAsync(adjustmentId);
            var details = dto.Details.Select((x, index) =>
            {
                var existing = existingDetails.FirstOrDefault(d => d.DetailId == x.DetailId);
                return new InventoryAdjustmentDetail
                {
                    DetailId = x.DetailId ?? Guid.NewGuid(),
                    AdjustmentId = adjustmentId,
                    LineNum = index + 1,
                    GoodsId = x.GoodsId,
                    BarcodeId = x.BarcodeId,
                    AdjustmentQty = x.AdjustmentQty,
                    UnitCost = x.UnitCost,
                    AdjustmentCost = (x.UnitCost ?? 0) * x.AdjustmentQty,
                    AdjustmentAmount = (x.UnitCost ?? 0) * x.AdjustmentQty,
                    Reason = x.Reason,
                    Memo = x.Memo,
                    CreatedBy = existing?.CreatedBy ?? dto.AdjustmentUser,
                    CreatedAt = existing?.CreatedAt ?? DateTime.Now
                };
            }).ToList();

            entity.TotalQty = details.Sum(x => x.AdjustmentQty);
            entity.TotalCost = details.Sum(x => x.AdjustmentCost ?? 0);
            entity.TotalAmount = details.Sum(x => x.AdjustmentAmount ?? 0);

            await _repository.UpdateAsync(entity, details);
            _logger.LogInfo($"更新庫存調整單成功: {adjustmentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新庫存調整單失敗: {adjustmentId}", ex);
            throw;
        }
    }

    public async Task DeleteInventoryAdjustmentAsync(string adjustmentId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(adjustmentId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"調整單不存在: {adjustmentId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅草稿狀態的調整單可刪除");
            }

            await _repository.DeleteAsync(adjustmentId);
            _logger.LogInfo($"刪除庫存調整單成功: {adjustmentId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除庫存調整單失敗: {adjustmentId}", ex);
            throw;
        }
    }

    public async Task ConfirmAdjustmentAsync(string adjustmentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var entity = await _repository.GetByIdAsync(adjustmentId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"調整單不存在: {adjustmentId}");
            }

            if (entity.Status == "C")
            {
                throw new InvalidOperationException("調整單已確認");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("調整單已取消");
            }

            // 取得調整單明細
            var details = await _repository.GetDetailsByAdjustmentIdAsync(adjustmentId);
            var detailsList = details.ToList();

            if (!detailsList.Any())
            {
                throw new InvalidOperationException($"調整單無明細資料: {adjustmentId}");
            }

            // 更新庫存邏輯
            foreach (var detail in detailsList)
            {
                if (detail.AdjustmentQty != 0)
                {
                    // 取得調整前庫存數量
                    var currentStock = await _stockRepository.GetStockQtyAsync(entity.ShopId, detail.GoodsId, transaction);
                    detail.BeforeQty = currentStock;
                    detail.AfterQty = currentStock + detail.AdjustmentQty;

                    // 更新庫存數量
                    await _stockRepository.UpdateStockQtyAsync(
                        entity.ShopId,
                        detail.GoodsId,
                        detail.AdjustmentQty,
                        transaction);

                    _logger.LogInfo($"更新庫存: ShopId={entity.ShopId}, GoodsId={detail.GoodsId}, AdjustmentQty={detail.AdjustmentQty}");
                }
            }

            // 更新調整單狀態
            await _repository.UpdateStatusAsync(adjustmentId, "C", transaction);

            transaction.Commit();
            _logger.LogInfo($"確認調整單成功: {adjustmentId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"確認調整單失敗: {adjustmentId}", ex);
            throw;
        }
    }

    public async Task CancelAdjustmentAsync(string adjustmentId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var entity = await _repository.GetByIdAsync(adjustmentId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"調整單不存在: {adjustmentId}");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("調整單已取消");
            }

            // 如果已確認，需要回退庫存
            if (entity.Status == "C")
            {
                // 取得調整單明細
                var details = await _repository.GetDetailsByAdjustmentIdAsync(adjustmentId);
                var detailsList = details.ToList();

                // 回退庫存邏輯
                foreach (var detail in detailsList)
                {
                    if (detail.AdjustmentQty != 0)
                    {
                        // 回退庫存數量（反向調整）
                        await _stockRepository.UpdateStockQtyAsync(
                            entity.ShopId,
                            detail.GoodsId,
                            -detail.AdjustmentQty,
                            transaction);

                        _logger.LogInfo($"回退庫存: ShopId={entity.ShopId}, GoodsId={detail.GoodsId}, AdjustmentQty={-detail.AdjustmentQty}");
                    }
                }
            }

            // 更新調整單狀態
            await _repository.UpdateStatusAsync(adjustmentId, "X", transaction);

            transaction.Commit();
            _logger.LogInfo($"取消調整單成功: {adjustmentId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"取消調整單失敗: {adjustmentId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<AdjustmentReasonDto>> GetAdjustmentReasonsAsync()
    {
        try
        {
            var reasons = await _repository.GetAdjustmentReasonsAsync();
            return reasons.Select(x => new AdjustmentReasonDto
            {
                ReasonId = x.ReasonId,
                ReasonName = x.ReasonName,
                ReasonType = x.ReasonType,
                Status = x.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢調整原因失敗", ex);
            throw;
        }
    }

    private InventoryAdjustmentDto MapToDto(InventoryAdjustment entity, IEnumerable<InventoryAdjustmentDetail> details)
    {
        return new InventoryAdjustmentDto
        {
            AdjustmentId = entity.AdjustmentId,
            AdjustmentDate = entity.AdjustmentDate,
            ShopId = entity.ShopId,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            AdjustmentType = entity.AdjustmentType,
            AdjustmentUser = entity.AdjustmentUser,
            Memo = entity.Memo,
            Memo2 = entity.Memo2,
            SourceNo = entity.SourceNo,
            SourceNum = entity.SourceNum,
            SourceCheckDate = entity.SourceCheckDate,
            SourceSuppId = entity.SourceSuppId,
            SiteId = entity.SiteId,
            TotalQty = entity.TotalQty,
            TotalCost = entity.TotalCost,
            TotalAmount = entity.TotalAmount,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,
            Details = details.Select(x => new InventoryAdjustmentDetailDto
            {
                DetailId = x.DetailId,
                AdjustmentId = x.AdjustmentId,
                LineNum = x.LineNum,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                AdjustmentQty = x.AdjustmentQty,
                BeforeQty = x.BeforeQty,
                AfterQty = x.AfterQty,
                UnitCost = x.UnitCost,
                AdjustmentCost = x.AdjustmentCost,
                AdjustmentAmount = x.AdjustmentAmount,
                Reason = x.Reason,
                Memo = x.Memo,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList()
        };
    }

    private string GetStatusName(string status)
    {
        return status switch
        {
            "D" => "草稿",
            "C" => "已確認",
            "X" => "已取消",
            _ => status
        };
    }
}

