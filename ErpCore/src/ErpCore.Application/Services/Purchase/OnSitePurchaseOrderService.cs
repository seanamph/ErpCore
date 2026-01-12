using Dapper;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Purchase;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 現場打單作業服務實作 (SYSW322)
/// </summary>
public class OnSitePurchaseOrderService : BaseService, IOnSitePurchaseOrderService
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public OnSitePurchaseOrderService(
        IPurchaseOrderRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PurchaseOrderDto>> GetOnSitePurchaseOrdersAsync(PurchaseOrderQueryDto query)
    {
        try
        {
            // 現場打單作業預設篩選 SourceProgram = 'SYSW322'
            var repositoryQuery = new PurchaseOrderQuery
            {
                OrderId = query.OrderId,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                SupplierId = query.SupplierId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                SourceProgram = "SYSW322", // 固定為現場打單
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PurchaseOrderDto
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                OrderType = x.OrderType,
                ShopId = x.ShopId,
                SupplierId = x.SupplierId,
                Status = x.Status,
                ApplyUserId = x.ApplyUserId,
                ApplyDate = x.ApplyDate,
                ApproveUserId = x.ApproveUserId,
                ApproveDate = x.ApproveDate,
                TotalAmount = x.TotalAmount,
                TotalQty = x.TotalQty,
                Memo = x.Memo,
                ExpectedDate = x.ExpectedDate,
                SiteId = x.SiteId,
                SourceProgram = x.SourceProgram,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<PurchaseOrderDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢現場打單申請單列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseOrderFullDto> GetOnSitePurchaseOrderByIdAsync(string orderId)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"現場打單申請單不存在: {orderId}");
            }

            // 檢查是否為現場打單申請單
            if (order.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"此申請單不是現場打單申請單: {orderId}");
            }

            var details = await _repository.GetDetailsByOrderIdAsync(orderId);

            var dto = new PurchaseOrderFullDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                OrderType = order.OrderType,
                ShopId = order.ShopId,
                SupplierId = order.SupplierId,
                Status = order.Status,
                ApplyUserId = order.ApplyUserId,
                ApplyDate = order.ApplyDate,
                ApproveUserId = order.ApproveUserId,
                ApproveDate = order.ApproveDate,
                TotalAmount = order.TotalAmount,
                TotalQty = order.TotalQty,
                Memo = order.Memo,
                ExpectedDate = order.ExpectedDate,
                SiteId = order.SiteId,
                SourceProgram = order.SourceProgram,
                CreatedBy = order.CreatedBy,
                CreatedAt = order.CreatedAt,
                UpdatedBy = order.UpdatedBy,
                UpdatedAt = order.UpdatedAt,
                Details = details.Select(d => new PurchaseOrderDetailDto
                {
                    DetailId = d.DetailId,
                    OrderId = d.OrderId,
                    LineNum = d.LineNum,
                    GoodsId = d.GoodsId,
                    BarcodeId = d.BarcodeId,
                    OrderQty = d.OrderQty,
                    UnitPrice = d.UnitPrice,
                    Amount = d.Amount,
                    ReceivedQty = d.ReceivedQty,
                    Memo = d.Memo,
                    CreatedBy = d.CreatedBy,
                    CreatedAt = d.CreatedAt
                }).ToList()
            };

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<string> CreateOnSitePurchaseOrderAsync(CreatePurchaseOrderDto dto)
    {
        try
        {
            var entity = new PurchaseOrder
            {
                OrderDate = dto.OrderDate,
                OrderType = dto.OrderType,
                ShopId = dto.ShopId,
                SupplierId = dto.SupplierId,
                Status = "D", // 草稿
                Memo = dto.Memo,
                ExpectedDate = dto.ExpectedDate,
                SiteId = dto.SiteId,
                SourceProgram = "SYSW322", // 固定為現場打單
                CreatedBy = GetCurrentUserId(),
                UpdatedBy = GetCurrentUserId()
            };

            var details = dto.Details.Select((d, index) => new PurchaseOrderDetail
            {
                LineNum = d.LineNum > 0 ? d.LineNum : index + 1,
                GoodsId = d.GoodsId,
                BarcodeId = d.BarcodeId,
                OrderQty = d.OrderQty,
                UnitPrice = d.UnitPrice,
                ReceivedQty = 0,
                Memo = d.Memo,
                CreatedBy = GetCurrentUserId()
            }).ToList();

            var orderId = await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立現場打單申請單成功: {orderId}");
            return orderId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立現場打單申請單失敗", ex);
            throw;
        }
    }

    public async Task UpdateOnSitePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"現場打單申請單不存在: {orderId}");
            }

            // 檢查是否為現場打單申請單
            if (order.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"此申請單不是現場打單申請單: {orderId}");
            }

            if (order.Status != "D")
            {
                throw new InvalidOperationException("只能修改草稿狀態的現場打單申請單");
            }

            order.OrderDate = dto.OrderDate;
            order.OrderType = dto.OrderType;
            order.ShopId = dto.ShopId;
            order.SupplierId = dto.SupplierId;
            order.Memo = dto.Memo;
            order.ExpectedDate = dto.ExpectedDate;
            order.SiteId = dto.SiteId;
            order.UpdatedBy = GetCurrentUserId();

            var details = dto.Details.Select((d, index) => new PurchaseOrderDetail
            {
                DetailId = d.DetailId ?? Guid.Empty,
                LineNum = d.LineNum > 0 ? d.LineNum : index + 1,
                GoodsId = d.GoodsId,
                BarcodeId = d.BarcodeId,
                OrderQty = d.OrderQty,
                UnitPrice = d.UnitPrice,
                ReceivedQty = 0, // 修改時不變更已收數量
                Memo = d.Memo,
                CreatedBy = GetCurrentUserId()
            }).ToList();

            await _repository.UpdateAsync(order, details);
            _logger.LogInfo($"更新現場打單申請單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task DeleteOnSitePurchaseOrderAsync(string orderId)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"現場打單申請單不存在: {orderId}");
            }

            // 檢查是否為現場打單申請單
            if (order.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"此申請單不是現場打單申請單: {orderId}");
            }

            await _repository.DeleteAsync(orderId);
            _logger.LogInfo($"刪除現場打單申請單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task SubmitOnSitePurchaseOrderAsync(string orderId)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"現場打單申請單不存在: {orderId}");
            }

            // 檢查是否為現場打單申請單
            if (order.SourceProgram != "SYSW322")
            {
                throw new InvalidOperationException($"此申請單不是現場打單申請單: {orderId}");
            }

            if (order.Status != "D")
            {
                throw new InvalidOperationException("只能送出草稿狀態的現場打單申請單");
            }

            await _repository.UpdateStatusAsync(orderId, "S", GetCurrentUserId());
            _logger.LogInfo($"送出現場打單申請單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"送出現場打單申請單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<GoodsByBarcodeDto> GetGoodsByBarcodeAsync(string barcode)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(barcode))
            {
                throw new ArgumentException("條碼不能為空", nameof(barcode));
            }

            using var connection = _connectionFactory.CreateConnection();

            // 根據條碼查詢商品資訊
            const string sql = @"
                SELECT TOP 1 
                    GoodsId, 
                    GoodsName, 
                    BarcodeId, 
                    Mprc AS UnitPrice,
                    Unit
                FROM Products 
                WHERE BarcodeId = @BarcodeId 
                  AND Status = '1'";

            var goods = await connection.QueryFirstOrDefaultAsync<GoodsByBarcodeDto>(sql, new { BarcodeId = barcode });

            if (goods == null)
            {
                throw new InvalidOperationException($"找不到條碼為 {barcode} 的商品");
            }

            _logger.LogInfo($"根據條碼查詢商品成功: {barcode}, 商品編號: {goods.GoodsId}");
            return goods;
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據條碼查詢商品失敗: {barcode}", ex);
            throw;
        }
    }
}

