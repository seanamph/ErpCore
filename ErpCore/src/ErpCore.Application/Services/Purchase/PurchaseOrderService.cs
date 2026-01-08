using System.Data;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Purchase;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購單服務實作
/// </summary>
public class PurchaseOrderService : BaseService, IPurchaseOrderService
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public PurchaseOrderService(
        IPurchaseOrderRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PurchaseOrderDto>> GetPurchaseOrdersAsync(PurchaseOrderQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseOrderQuery
            {
                OrderId = query.OrderId,
                OrderType = query.OrderType,
                ShopId = query.ShopId,
                SupplierId = query.SupplierId,
                Status = query.Status,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                SourceProgram = query.SourceProgram,
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
            _logger.LogError("查詢採購單列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseOrderFullDto> GetPurchaseOrderByIdAsync(string orderId)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
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
            _logger.LogError($"查詢採購單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
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
                SourceProgram = dto.SourceProgram ?? "SYSW315",
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
            _logger.LogInfo($"建立採購單成功: {orderId}");
            return orderId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立採購單失敗", ex);
            throw;
        }
    }

    public async Task UpdatePurchaseOrderAsync(string orderId, UpdatePurchaseOrderDto dto)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }

            if (order.Status != "D")
            {
                throw new InvalidOperationException("只能修改草稿狀態的採購單");
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
            _logger.LogInfo($"更新採購單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新採購單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task DeletePurchaseOrderAsync(string orderId)
    {
        try
        {
            await _repository.DeleteAsync(orderId);
            _logger.LogInfo($"刪除採購單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task SubmitPurchaseOrderAsync(string orderId)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }

            if (order.Status != "D")
            {
                throw new InvalidOperationException("只能送出草稿狀態的採購單");
            }

            await _repository.UpdateStatusAsync(orderId, "S", GetCurrentUserId());
            _logger.LogInfo($"送出採購單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"送出採購單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task ApprovePurchaseOrderAsync(string orderId)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }

            if (order.Status != "S")
            {
                throw new InvalidOperationException("只能審核已送出狀態的採購單");
            }

            await _repository.UpdateStatusAsync(orderId, "A", GetCurrentUserId());
            _logger.LogInfo($"審核採購單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核採購單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task CancelPurchaseOrderAsync(string orderId)
    {
        try
        {
            var order = await _repository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"採購單不存在: {orderId}");
            }

            if (order.Status == "X")
            {
                throw new InvalidOperationException("採購單已取消");
            }

            await _repository.UpdateStatusAsync(orderId, "X");
            _logger.LogInfo($"取消採購單成功: {orderId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消採購單失敗: {orderId}", ex);
            throw;
        }
    }
}

