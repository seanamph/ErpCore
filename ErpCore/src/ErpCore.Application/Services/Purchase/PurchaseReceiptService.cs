using System.Data;
using ErpCore.Application.DTOs.Purchase;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Purchase;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Infrastructure.Repositories.Purchase;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;
using Dapper;

namespace ErpCore.Application.Services.Purchase;

/// <summary>
/// 採購驗收單服務實作
/// </summary>
public class PurchaseReceiptService : BaseService, IPurchaseReceiptService
{
    private readonly IPurchaseReceiptRepository _repository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IStockRepository _stockRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public PurchaseReceiptService(
        IPurchaseReceiptRepository repository,
        IPurchaseOrderRepository purchaseOrderRepository,
        IStockRepository stockRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _stockRepository = stockRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PendingPurchaseOrderDto>> GetPendingOrdersAsync(PendingPurchaseOrderQueryDto query)
    {
        try
        {
            var repositoryQuery = new PendingPurchaseOrderQuery
            {
                OrderId = query.OrderId,
                ShopId = query.ShopId,
                SupplierId = query.SupplierId,
                OrderDateFrom = query.OrderDateFrom,
                OrderDateTo = query.OrderDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.GetPendingOrdersAsync(repositoryQuery);
            var totalCount = await _repository.GetPendingOrdersCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PendingPurchaseOrderDto
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                ShopId = x.ShopId,
                SupplierId = x.SupplierId,
                Status = x.Status,
                TotalQty = x.TotalQty,
                ReceiptQty = x.ReceiptQty,
                PendingReceiptQty = x.PendingReceiptQty
            }).ToList();

            return new PagedResult<PendingPurchaseOrderDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢待驗收採購單失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<PurchaseReceiptDto>> GetPurchaseReceiptsAsync(PurchaseReceiptQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseReceiptQuery
            {
                ReceiptId = query.ReceiptId,
                OrderId = query.OrderId,
                ShopId = query.ShopId,
                SupplierId = query.SupplierId,
                Status = query.Status,
                ReceiptDateFrom = query.ReceiptDateFrom,
                ReceiptDateTo = query.ReceiptDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PurchaseReceiptDto
            {
                ReceiptId = x.ReceiptId,
                OrderId = x.OrderId,
                ReceiptDate = x.ReceiptDate,
                ShopId = x.ShopId,
                SupplierId = x.SupplierId,
                Status = x.Status,
                ReceiptUserId = x.ReceiptUserId,
                TotalAmount = x.TotalAmount,
                TotalQty = x.TotalQty,
                Memo = x.Memo,
                IsSettled = x.IsSettled,
                SettledDate = x.SettledDate,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<PurchaseReceiptDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購驗收單列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseReceiptFullDto> GetPurchaseReceiptByIdAsync(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗收單不存在: {receiptId}");
            }

            var details = await _repository.GetDetailsByReceiptIdAsync(receiptId);

            var receiptDto = new PurchaseReceiptDto
            {
                ReceiptId = entity.ReceiptId,
                OrderId = entity.OrderId,
                ReceiptDate = entity.ReceiptDate,
                ShopId = entity.ShopId,
                SupplierId = entity.SupplierId,
                Status = entity.Status,
                ReceiptUserId = entity.ReceiptUserId,
                TotalAmount = entity.TotalAmount,
                TotalQty = entity.TotalQty,
                Memo = entity.Memo,
                IsSettled = entity.IsSettled,
                SettledDate = entity.SettledDate,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                Details = details.Select(x => new PurchaseReceiptDetailDto
                {
                    DetailId = x.DetailId,
                    ReceiptId = x.ReceiptId,
                    OrderDetailId = x.OrderDetailId,
                    LineNum = x.LineNum,
                    GoodsId = x.GoodsId,
                    BarcodeId = x.BarcodeId,
                    OrderQty = x.OrderQty,
                    ReceiptQty = x.ReceiptQty,
                    UnitPrice = x.UnitPrice,
                    Amount = x.Amount,
                    Memo = x.Memo,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };

            return new PurchaseReceiptFullDto { Receipt = receiptDto };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task<PurchaseReceiptFullDto> CreateReceiptFromOrderAsync(string orderId)
    {
        try
        {
            // 查詢採購單主檔
            var order = await _purchaseOrderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"採購單不存在: {orderId}");
            }

            // 查詢採購單明細
            var orderDetails = await _purchaseOrderRepository.GetDetailsByOrderIdAsync(orderId);
            var orderDetailsList = orderDetails.ToList();

            if (!orderDetailsList.Any())
            {
                throw new InvalidOperationException($"採購單無明細資料: {orderId}");
            }

            // 產生驗收單號
            var receiptId = await _repository.GenerateReceiptIdAsync();

            // 建立驗收單明細（從採購單明細帶入，計算可驗收數量）
            var receiptDetails = orderDetailsList.Select((od, index) => new PurchaseReceiptDetailDto
            {
                OrderDetailId = od.DetailId,
                LineNum = index + 1,
                GoodsId = od.GoodsId,
                BarcodeId = od.BarcodeId,
                OrderQty = od.OrderQty,
                ReceiptQty = 0, // 預設為0，由使用者輸入
                UnitPrice = od.UnitPrice,
                Amount = 0,
                Memo = od.Memo
            }).ToList();

            return new PurchaseReceiptFullDto
            {
                Receipt = new PurchaseReceiptDto
                {
                    ReceiptId = receiptId,
                    OrderId = orderId,
                    ReceiptDate = DateTime.Now,
                    ShopId = order.ShopId,
                    SupplierId = order.SupplierId,
                    Status = "P",
                    Details = receiptDetails
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"依採購單建立驗收單失敗: {orderId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePurchaseReceiptAsync(CreatePurchaseReceiptDto dto)
    {
        try
        {
            // 查詢採購單主檔以取得 ShopId 和 SupplierId
            var order = await _purchaseOrderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                throw new KeyNotFoundException($"採購單不存在: {dto.OrderId}");
            }

            // 驗證採購單狀態（必須是已審核狀態才能驗收）
            if (order.Status != "A")
            {
                throw new InvalidOperationException($"僅已審核的採購單可進行驗收，目前狀態: {order.Status}");
            }

            var entity = new PurchaseReceipt
            {
                OrderId = dto.OrderId,
                ReceiptDate = dto.ReceiptDate,
                ReceiptUserId = dto.ReceiptUserId,
                Memo = dto.Memo,
                Status = "P",
                IsSettled = false,
                ShopId = order.ShopId,
                SupplierId = order.SupplierId,
                CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
            };

            // 建立驗收單明細，從採購單明細取得訂購數量
            var details = new List<PurchaseReceiptDetail>();
            foreach (var detailDto in dto.Details)
            {
                // 驗證驗收數量
                if (detailDto.ReceiptQty <= 0)
                {
                    throw new InvalidOperationException($"驗收數量必須大於0: {detailDto.GoodsId}");
                }

                // 取得採購單明細
                PurchaseOrderDetail? orderDetail = null;
                if (detailDto.OrderDetailId.HasValue)
                {
                    orderDetail = await _purchaseOrderRepository.GetDetailByIdAsync(detailDto.OrderDetailId.Value);
                }

                if (orderDetail == null)
                {
                    throw new KeyNotFoundException($"採購單明細不存在: {detailDto.OrderDetailId}");
                }

                // 驗證驗收數量不超過可驗收數量
                var availableQty = orderDetail.OrderQty - orderDetail.ReceivedQty;
                if (detailDto.ReceiptQty > availableQty)
                {
                    throw new InvalidOperationException($"驗收數量 {detailDto.ReceiptQty} 超過可驗收數量 {availableQty}: {detailDto.GoodsId}");
                }

                var detail = new PurchaseReceiptDetail
                {
                    OrderDetailId = detailDto.OrderDetailId,
                    LineNum = detailDto.LineNum,
                    GoodsId = detailDto.GoodsId,
                    BarcodeId = detailDto.BarcodeId,
                    OrderQty = orderDetail.OrderQty,
                    ReceiptQty = detailDto.ReceiptQty,
                    UnitPrice = detailDto.UnitPrice ?? orderDetail.UnitPrice,
                    Amount = (detailDto.UnitPrice ?? orderDetail.UnitPrice ?? 0) * detailDto.ReceiptQty,
                    Memo = detailDto.Memo,
                    CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
                };

                details.Add(detail);
            }

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => x.Amount ?? 0);

            var receiptId = await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立採購驗收單成功: {receiptId}");
            return receiptId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立採購驗收單失敗", ex);
            throw;
        }
    }

    public async Task UpdatePurchaseReceiptAsync(string receiptId, UpdatePurchaseReceiptDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗收單不存在: {receiptId}");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗收單不可修改");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的驗收單不可修改");
            }

            entity.ReceiptDate = dto.ReceiptDate;
            entity.ReceiptUserId = dto.ReceiptUserId;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = dto.ReceiptUserId;

            var existingDetails = await _repository.GetDetailsByReceiptIdAsync(receiptId);
            var details = dto.Details.Select((x, index) =>
            {
                var existing = existingDetails.FirstOrDefault(d => d.DetailId == x.DetailId);
                return new PurchaseReceiptDetail
                {
                    DetailId = x.DetailId ?? Guid.NewGuid(),
                    ReceiptId = receiptId,
                    OrderDetailId = existing?.OrderDetailId,
                    LineNum = index + 1,
                    GoodsId = existing?.GoodsId ?? string.Empty,
                    BarcodeId = existing?.BarcodeId,
                    OrderQty = existing?.OrderQty ?? 0,
                    ReceiptQty = x.ReceiptQty,
                    UnitPrice = x.UnitPrice,
                    Amount = x.UnitPrice.HasValue ? x.UnitPrice.Value * x.ReceiptQty : null,
                    Memo = x.Memo,
                    CreatedBy = existing?.CreatedBy ?? dto.ReceiptUserId,
                    CreatedAt = existing?.CreatedAt ?? DateTime.Now
                };
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => x.Amount ?? 0);

            await _repository.UpdateAsync(entity, details);
            _logger.LogInfo($"更新採購驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新採購驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task DeletePurchaseReceiptAsync(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗收單不存在: {receiptId}");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗收單不可刪除");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的驗收單不可刪除");
            }

            await _repository.DeleteAsync(receiptId);
            _logger.LogInfo($"刪除採購驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task ConfirmReceiptAsync(string receiptId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗收單不存在: {receiptId}");
            }

            if (entity.Status == "C")
            {
                throw new InvalidOperationException("驗收單已確認");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗收單不可確認");
            }

            // 取得驗收單明細
            var details = await _repository.GetDetailsByReceiptIdAsync(receiptId);
            var detailsList = details.ToList();

            if (!detailsList.Any())
            {
                throw new InvalidOperationException($"驗收單無明細資料: {receiptId}");
            }

            // 1. 更新庫存（驗收後庫存增加）
            foreach (var detail in detailsList)
            {
                if (detail.ReceiptQty > 0)
                {
                    await _stockRepository.UpdateStockQtyAsync(
                        entity.ShopId,
                        detail.GoodsId,
                        detail.ReceiptQty,
                        transaction);

                    _logger.LogInfo($"更新庫存: ShopId={entity.ShopId}, GoodsId={detail.GoodsId}, ReceiptQty={detail.ReceiptQty}");
                }
            }

            // 2. 更新採購單已收數量
            foreach (var detail in detailsList)
            {
                if (detail.OrderDetailId.HasValue && detail.ReceiptQty > 0)
                {
                    await _purchaseOrderRepository.UpdateReceiptQtyAsync(
                        detail.OrderDetailId.Value,
                        detail.ReceiptQty,
                        transaction);
                    _logger.LogInfo($"更新採購單已收數量: OrderDetailId={detail.OrderDetailId}, ReceiptQty={detail.ReceiptQty}");
                }
            }

            // 3. 更新驗收單狀態為已確認
            await _repository.UpdateStatusAsync(receiptId, "C", transaction);

            transaction.Commit();
            _logger.LogInfo($"確認採購驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"確認採購驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task CancelPurchaseReceiptAsync(string receiptId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗收單不存在: {receiptId}");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("驗收單已取消");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗收單不可取消");
            }

            // 如果已確認，需要回退庫存和採購單已收數量
            if (entity.Status == "C")
            {
                var details = await _repository.GetDetailsByReceiptIdAsync(receiptId);
                var detailsList = details.ToList();

                // 回退庫存
                foreach (var detail in detailsList)
                {
                    if (detail.ReceiptQty > 0)
                    {
                        await _stockRepository.UpdateStockQtyAsync(
                            entity.ShopId,
                            detail.GoodsId,
                            -detail.ReceiptQty,
                            transaction);

                        _logger.LogInfo($"回退庫存: ShopId={entity.ShopId}, GoodsId={detail.GoodsId}, ReceiptQty={-detail.ReceiptQty}");
                    }
                }

                // 回退採購單已收數量
                foreach (var detail in detailsList)
                {
                    if (detail.OrderDetailId.HasValue && detail.ReceiptQty > 0)
                    {
                        await _purchaseOrderRepository.UpdateReceiptQtyAsync(
                            detail.OrderDetailId.Value,
                            -detail.ReceiptQty,
                            transaction);
                        _logger.LogInfo($"回退採購單已收數量: OrderDetailId={detail.OrderDetailId}, ReceiptQty={-detail.ReceiptQty}");
                    }
                }
            }

            // 更新驗收單狀態為已取消
            await _repository.UpdateStatusAsync(receiptId, "X", transaction);

            transaction.Commit();
            _logger.LogInfo($"取消採購驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"取消採購驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    // ========== SYSW333 - 已日結採購單驗收調整作業 ==========

    public async Task<PagedResult<PurchaseReceiptDto>> GetSettledAdjustmentsAsync(SettledPurchaseReceiptAdjustmentQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseReceiptQuery
            {
                ReceiptId = query.ReceiptId,
                OrderId = query.OrderId,
                ShopId = query.ShopId,
                SupplierId = query.SupplierId,
                Status = query.Status,
                ReceiptDateFrom = query.ReceiptDateFrom,
                ReceiptDateTo = query.ReceiptDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QuerySettledAdjustmentsAsync(repositoryQuery);
            var totalCount = await _repository.GetSettledAdjustmentsCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PurchaseReceiptDto
            {
                ReceiptId = x.ReceiptId,
                OrderId = x.OrderId,
                ReceiptDate = x.ReceiptDate,
                ShopId = x.ShopId,
                SupplierId = x.SupplierId,
                Status = x.Status,
                ReceiptUserId = x.ReceiptUserId,
                TotalAmount = x.TotalAmount,
                TotalQty = x.TotalQty,
                Memo = x.Memo,
                IsSettled = x.IsSettled,
                SettledDate = x.SettledDate,
                PurchaseOrderType = x.PurchaseOrderType,
                IsSettledAdjustment = x.IsSettledAdjustment,
                OriginalReceiptId = x.OriginalReceiptId,
                AdjustmentReason = x.AdjustmentReason,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<PurchaseReceiptDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢已日結採購單驗收調整列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseReceiptFullDto> GetSettledAdjustmentByIdAsync(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment)
            {
                throw new KeyNotFoundException($"已日結採購單驗收調整單不存在: {receiptId}");
            }

            var details = await _repository.GetDetailsByReceiptIdAsync(receiptId);

            var receiptDto = new PurchaseReceiptDto
            {
                ReceiptId = entity.ReceiptId,
                OrderId = entity.OrderId,
                ReceiptDate = entity.ReceiptDate,
                ShopId = entity.ShopId,
                SupplierId = entity.SupplierId,
                Status = entity.Status,
                ReceiptUserId = entity.ReceiptUserId,
                TotalAmount = entity.TotalAmount,
                TotalQty = entity.TotalQty,
                Memo = entity.Memo,
                IsSettled = entity.IsSettled,
                SettledDate = entity.SettledDate,
                PurchaseOrderType = entity.PurchaseOrderType,
                IsSettledAdjustment = entity.IsSettledAdjustment,
                OriginalReceiptId = entity.OriginalReceiptId,
                AdjustmentReason = entity.AdjustmentReason,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                Details = details.Select(x => new PurchaseReceiptDetailDto
                {
                    DetailId = x.DetailId,
                    ReceiptId = x.ReceiptId,
                    OrderDetailId = x.OrderDetailId,
                    LineNum = x.LineNum,
                    GoodsId = x.GoodsId,
                    BarcodeId = x.BarcodeId,
                    OrderQty = x.OrderQty,
                    ReceiptQty = x.ReceiptQty,
                    UnitPrice = x.UnitPrice,
                    Amount = x.Amount,
                    OriginalReceiptQty = x.OriginalReceiptQty,
                    AdjustmentQty = x.AdjustmentQty,
                    OriginalUnitPrice = x.OriginalUnitPrice,
                    AdjustmentPrice = x.AdjustmentPrice,
                    Memo = x.Memo,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };

            return new PurchaseReceiptFullDto { Receipt = receiptDto };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢已日結採購單驗收調整失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task<List<PurchaseOrderDto>> GetSettledOrdersAsync(SettledOrderQueryDto query)
    {
        try
        {
            // 查詢已日結的採購單（Status = 'A' 且 IsSettled = true）
            var orders = await _repository.GetSettledOrdersAsync(query);
            return orders.Select(x => new PurchaseOrderDto
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                OrderType = x.OrderType,
                ShopId = x.ShopId,
                SupplierId = x.SupplierId,
                Status = x.Status,
                TotalAmount = x.TotalAmount,
                TotalQty = x.TotalQty
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢已日結採購單失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateSettledAdjustmentAsync(CreatePurchaseReceiptDto dto)
    {
        try
        {
            // 驗證採購單是否已日結（Status = 'A' 表示已審核，視為已日結）
            var order = await _repository.GetPurchaseOrderByIdAsync(dto.OrderId);
            if (order == null || order.Status != "A")
            {
                throw new InvalidOperationException("僅已日結的採購單可進行驗收調整");
            }

            var receiptId = await _repository.GenerateReceiptIdAsync();
            var entity = new PurchaseReceipt
            {
                ReceiptId = receiptId,
                OrderId = dto.OrderId,
                ReceiptDate = dto.ReceiptDate,
                ReceiptUserId = dto.ReceiptUserId,
                Memo = dto.Memo,
                Status = "D", // 草稿狀態
                IsSettled = false,
                IsSettledAdjustment = true,
                PurchaseOrderType = "1", // 採購單
                AdjustmentReason = dto.Memo, // 使用備註作為調整原因
                ShopId = order.ShopId,
                SupplierId = order.SupplierId,
                CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
            };

            var details = dto.Details.Select((x, index) => new PurchaseReceiptDetail
            {
                DetailId = Guid.NewGuid(),
                ReceiptId = receiptId,
                OrderDetailId = x.OrderDetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                OrderQty = 0, // 需要從採購單明細取得
                ReceiptQty = x.ReceiptQty,
                UnitPrice = x.UnitPrice,
                Amount = x.UnitPrice.HasValue ? x.UnitPrice.Value * x.ReceiptQty : null,
                OriginalReceiptQty = x.ReceiptQty, // 記錄原始驗收數量
                AdjustmentQty = 0, // 調整數量待計算
                OriginalUnitPrice = x.UnitPrice,
                AdjustmentPrice = 0, // 調整單價待計算
                Memo = x.Memo,
                CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => x.Amount ?? 0);

            await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立已日結採購單驗收調整成功: {receiptId}");
            return receiptId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立已日結採購單驗收調整失敗", ex);
            throw;
        }
    }

    public async Task UpdateSettledAdjustmentAsync(string receiptId, UpdatePurchaseReceiptDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment)
            {
                throw new KeyNotFoundException($"已日結採購單驗收調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可修改");
            }

            entity.ReceiptDate = dto.ReceiptDate;
            entity.ReceiptUserId = dto.ReceiptUserId;
            entity.Memo = dto.Memo;
            entity.AdjustmentReason = dto.Memo;
            entity.UpdatedBy = dto.ReceiptUserId ?? GetCurrentUserId();

            var existingDetails = await _repository.GetDetailsByReceiptIdAsync(receiptId);
            var details = dto.Details.Select((x, index) =>
            {
                var existing = existingDetails.FirstOrDefault(d => d.DetailId == x.DetailId);
                return new PurchaseReceiptDetail
                {
                    DetailId = x.DetailId ?? Guid.NewGuid(),
                    ReceiptId = receiptId,
                    OrderDetailId = existing?.OrderDetailId,
                    LineNum = index + 1,
                    GoodsId = existing?.GoodsId ?? string.Empty,
                    BarcodeId = existing?.BarcodeId,
                    OrderQty = existing?.OrderQty ?? 0,
                    ReceiptQty = x.ReceiptQty,
                    UnitPrice = x.UnitPrice,
                    Amount = x.UnitPrice.HasValue ? x.UnitPrice.Value * x.ReceiptQty : null,
                    OriginalReceiptQty = existing?.OriginalReceiptQty ?? x.ReceiptQty,
                    AdjustmentQty = x.ReceiptQty - (existing?.OriginalReceiptQty ?? x.ReceiptQty),
                    OriginalUnitPrice = existing?.OriginalUnitPrice ?? x.UnitPrice,
                    AdjustmentPrice = x.UnitPrice.HasValue && existing?.OriginalUnitPrice.HasValue == true
                        ? x.UnitPrice.Value - existing.OriginalUnitPrice.Value
                        : 0,
                    Memo = x.Memo,
                    CreatedBy = existing?.CreatedBy ?? "SYSTEM"
                };
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => x.Amount ?? 0);

            await _repository.UpdateAsync(entity, details);
            _logger.LogInfo($"修改已日結採購單驗收調整成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改已日結採購單驗收調整失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task DeleteSettledAdjustmentAsync(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment)
            {
                throw new KeyNotFoundException($"已日結採購單驗收調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可刪除");
            }

            await _repository.DeleteAsync(receiptId);
            _logger.LogInfo($"刪除已日結採購單驗收調整成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除已日結採購單驗收調整失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task ApproveSettledAdjustmentAsync(string receiptId, ApproveSettledAdjustmentDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment)
            {
                throw new KeyNotFoundException($"已日結採購單驗收調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可審核");
            }

            await _repository.UpdateStatusAsync(receiptId, "A", null); // A: 已審核
            _logger.LogInfo($"審核已日結採購單驗收調整成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核已日結採購單驗收調整失敗: {receiptId}", ex);
            throw;
        }
    }

    // ========== SYSW530 - 已日結退貨單驗退調整作業 ==========

    public async Task<PagedResult<PurchaseReceiptDto>> GetClosedReturnAdjustmentsAsync(ClosedReturnAdjustmentQueryDto query)
    {
        try
        {
            var repositoryQuery = new PurchaseReceiptQuery
            {
                ReceiptId = query.ReceiptId,
                OrderId = query.PurchaseOrderId,
                ShopId = query.ShopId,
                SupplierId = query.SupplierId,
                Status = query.Status,
                ReceiptDateFrom = query.CheckDateFrom,
                ReceiptDateTo = query.CheckDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryClosedReturnAdjustmentsAsync(repositoryQuery);
            var totalCount = await _repository.GetClosedReturnAdjustmentsCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PurchaseReceiptDto
            {
                ReceiptId = x.ReceiptId,
                OrderId = x.OrderId,
                ReceiptDate = x.ReceiptDate,
                ShopId = x.ShopId,
                SupplierId = x.SupplierId,
                Status = x.Status,
                ReceiptUserId = x.ReceiptUserId,
                TotalAmount = x.TotalAmount,
                TotalQty = x.TotalQty,
                Memo = x.Memo,
                IsSettled = x.IsSettled,
                SettledDate = x.SettledDate,
                PurchaseOrderType = x.PurchaseOrderType,
                IsSettledAdjustment = x.IsSettledAdjustment,
                OriginalReceiptId = x.OriginalReceiptId,
                AdjustmentReason = x.AdjustmentReason,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<PurchaseReceiptDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢已日結退貨單驗退調整列表失敗", ex);
            throw;
        }
    }

    public async Task<PurchaseReceiptFullDto> GetClosedReturnAdjustmentByIdAsync(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment || entity.PurchaseOrderType != "2")
            {
                throw new KeyNotFoundException($"已日結退貨單驗退調整單不存在: {receiptId}");
            }

            var details = await _repository.GetDetailsByReceiptIdAsync(receiptId);

            var receiptDto = new PurchaseReceiptDto
            {
                ReceiptId = entity.ReceiptId,
                OrderId = entity.OrderId,
                ReceiptDate = entity.ReceiptDate,
                ShopId = entity.ShopId,
                SupplierId = entity.SupplierId,
                Status = entity.Status,
                ReceiptUserId = entity.ReceiptUserId,
                TotalAmount = entity.TotalAmount,
                TotalQty = entity.TotalQty,
                Memo = entity.Memo,
                IsSettled = entity.IsSettled,
                SettledDate = entity.SettledDate,
                PurchaseOrderType = entity.PurchaseOrderType,
                IsSettledAdjustment = entity.IsSettledAdjustment,
                OriginalReceiptId = entity.OriginalReceiptId,
                AdjustmentReason = entity.AdjustmentReason,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                Details = details.Select(x => new PurchaseReceiptDetailDto
                {
                    DetailId = x.DetailId,
                    ReceiptId = x.ReceiptId,
                    OrderDetailId = x.OrderDetailId,
                    LineNum = x.LineNum,
                    GoodsId = x.GoodsId,
                    BarcodeId = x.BarcodeId,
                    OrderQty = x.OrderQty,
                    ReceiptQty = x.ReceiptQty,
                    UnitPrice = x.UnitPrice,
                    Amount = x.Amount,
                    OriginalReceiptQty = x.OriginalReceiptQty,
                    AdjustmentQty = x.AdjustmentQty,
                    OriginalUnitPrice = x.OriginalUnitPrice,
                    AdjustmentPrice = x.AdjustmentPrice,
                    Memo = x.Memo,
                    CreatedBy = x.CreatedBy,
                    CreatedAt = x.CreatedAt
                }).ToList()
            };

            return new PurchaseReceiptFullDto { Receipt = receiptDto };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢已日結退貨單驗退調整失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task<List<PurchaseOrderDto>> GetClosedReturnOrdersAsync(ClosedReturnOrderQueryDto query)
    {
        try
        {
            // 查詢已日結的退貨單（PurchaseOrderType = '2' 且 Status = 'A' 且 IsSettled = true）
            var orders = await _repository.GetClosedReturnOrdersAsync(query);
            return orders.Select(x => new PurchaseOrderDto
            {
                OrderId = x.OrderId,
                OrderDate = x.OrderDate,
                OrderType = x.OrderType,
                ShopId = x.ShopId,
                SupplierId = x.SupplierId,
                Status = x.Status,
                TotalAmount = x.TotalAmount,
                TotalQty = x.TotalQty
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢已日結退貨單失敗", ex);
            throw;
        }
    }

    public async Task<string> CreateClosedReturnAdjustmentAsync(CreatePurchaseReceiptDto dto)
    {
        try
        {
            // 驗證退貨單是否已日結（Status = 'A' 表示已審核，視為已日結）
            var order = await _repository.GetPurchaseOrderByIdAsync(dto.OrderId);
            if (order == null || order.Status != "A" || order.OrderType != "RT")
            {
                throw new InvalidOperationException("僅已日結的退貨單可進行驗退調整");
            }

            var receiptId = await _repository.GenerateReceiptIdAsync();
            var entity = new PurchaseReceipt
            {
                ReceiptId = receiptId,
                OrderId = dto.OrderId,
                ReceiptDate = dto.ReceiptDate,
                ReceiptUserId = dto.ReceiptUserId,
                Memo = dto.Memo,
                Status = "D", // 草稿狀態
                IsSettled = false,
                IsSettledAdjustment = true,
                PurchaseOrderType = "2", // 退貨單
                AdjustmentReason = dto.Memo,
                ShopId = order.ShopId,
                SupplierId = order.SupplierId,
                CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
            };

            var details = dto.Details.Select((x, index) => new PurchaseReceiptDetail
            {
                DetailId = Guid.NewGuid(),
                ReceiptId = receiptId,
                OrderDetailId = x.OrderDetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                OrderQty = 0,
                ReceiptQty = x.ReceiptQty,
                UnitPrice = x.UnitPrice,
                Amount = x.UnitPrice.HasValue ? x.UnitPrice.Value * x.ReceiptQty : null,
                OriginalReceiptQty = x.ReceiptQty,
                AdjustmentQty = 0,
                OriginalUnitPrice = x.UnitPrice,
                AdjustmentPrice = 0,
                Memo = x.Memo,
                CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => x.Amount ?? 0);

            await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立已日結退貨單驗退調整成功: {receiptId}");
            return receiptId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立已日結退貨單驗退調整失敗", ex);
            throw;
        }
    }

    public async Task UpdateClosedReturnAdjustmentAsync(string receiptId, UpdatePurchaseReceiptDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment || entity.PurchaseOrderType != "2")
            {
                throw new KeyNotFoundException($"已日結退貨單驗退調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可修改");
            }

            // 與 UpdateSettledAdjustmentAsync 相同的邏輯
            await UpdateSettledAdjustmentAsync(receiptId, dto);
            _logger.LogInfo($"修改已日結退貨單驗退調整成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改已日結退貨單驗退調整失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task DeleteClosedReturnAdjustmentAsync(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment || entity.PurchaseOrderType != "2")
            {
                throw new KeyNotFoundException($"已日結退貨單驗退調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可刪除");
            }

            await _repository.DeleteAsync(receiptId);
            _logger.LogInfo($"刪除已日結退貨單驗退調整成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除已日結退貨單驗退調整失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task ApproveClosedReturnAdjustmentAsync(string receiptId, ApproveSettledAdjustmentDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment || entity.PurchaseOrderType != "2")
            {
                throw new KeyNotFoundException($"已日結退貨單驗退調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可審核");
            }

            await _repository.UpdateStatusAsync(receiptId, "A", null); // A: 已審核
            _logger.LogInfo($"審核已日結退貨單驗退調整成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核已日結退貨單驗退調整失敗: {receiptId}", ex);
            throw;
        }
    }

    // ========== SYSW337 - 已日結退貨單驗退調整作業 ==========

    /// <summary>
    /// 查詢已日結退貨單驗退調整列表 (SYSW337)
    /// 與 SYSW530 共用相同邏輯，但可通過 SourceProgram 區分來源
    /// </summary>
    public async Task<PagedResult<PurchaseReceiptDto>> GetClosedReturnAdjustmentsV2Async(ClosedReturnAdjustmentQueryDto query)
    {
        try
        {
            // SYSW337 與 SYSW530 共用相同邏輯
            return await GetClosedReturnAdjustmentsAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢已日結退貨單驗退調整列表失敗 (SYSW337)", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢單筆已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    public async Task<PurchaseReceiptFullDto> GetClosedReturnAdjustmentV2ByIdAsync(string receiptId)
    {
        try
        {
            // SYSW337 與 SYSW530 共用相同邏輯
            return await GetClosedReturnAdjustmentByIdAsync(receiptId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢已日結退貨單驗退調整失敗 (SYSW337): {receiptId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢可用的已日結退貨單 (SYSW337)
    /// </summary>
    public async Task<List<PurchaseOrderDto>> GetClosedReturnOrdersV2Async(ClosedReturnOrderQueryDto query)
    {
        try
        {
            // SYSW337 與 SYSW530 共用相同邏輯
            return await GetClosedReturnOrdersAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢已日結退貨單失敗 (SYSW337)", ex);
            throw;
        }
    }

    /// <summary>
    /// 新增已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    public async Task<string> CreateClosedReturnAdjustmentV2Async(CreatePurchaseReceiptDto dto)
    {
        try
        {
            // 驗證退貨單是否已日結（Status = 'A' 表示已審核，視為已日結）
            var order = await _repository.GetPurchaseOrderByIdAsync(dto.OrderId);
            if (order == null || order.Status != "A" || order.OrderType != "RT")
            {
                throw new InvalidOperationException("僅已日結的退貨單可進行驗退調整");
            }

            var receiptId = await _repository.GenerateReceiptIdAsync();
            var entity = new PurchaseReceipt
            {
                ReceiptId = receiptId,
                OrderId = dto.OrderId,
                ReceiptDate = dto.ReceiptDate,
                ReceiptUserId = dto.ReceiptUserId,
                Memo = dto.Memo,
                Status = "D", // 草稿狀態
                IsSettled = false,
                IsSettledAdjustment = true,
                PurchaseOrderType = "2", // 退貨單
                AdjustmentReason = dto.Memo,
                ShopId = order.ShopId,
                SupplierId = order.SupplierId,
                CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
            };

            var details = dto.Details.Select((x, index) => new PurchaseReceiptDetail
            {
                DetailId = Guid.NewGuid(),
                ReceiptId = receiptId,
                OrderDetailId = x.OrderDetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                OrderQty = 0,
                ReceiptQty = x.ReceiptQty,
                UnitPrice = x.UnitPrice,
                Amount = x.UnitPrice.HasValue ? x.UnitPrice.Value * x.ReceiptQty : null,
                OriginalReceiptQty = x.ReceiptQty,
                AdjustmentQty = 0,
                OriginalUnitPrice = x.UnitPrice,
                AdjustmentPrice = 0,
                Memo = x.Memo,
                CreatedBy = dto.ReceiptUserId ?? GetCurrentUserId()
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => x.Amount ?? 0);

            await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立已日結退貨單驗退調整成功 (SYSW337): {receiptId}");
            return receiptId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立已日結退貨單驗退調整失敗 (SYSW337)", ex);
            throw;
        }
    }

    /// <summary>
    /// 修改已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    public async Task UpdateClosedReturnAdjustmentV2Async(string receiptId, UpdatePurchaseReceiptDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment || entity.PurchaseOrderType != "2")
            {
                throw new KeyNotFoundException($"已日結退貨單驗退調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可修改");
            }

            // 與 UpdateClosedReturnAdjustmentAsync 相同的邏輯
            await UpdateClosedReturnAdjustmentAsync(receiptId, dto);
            _logger.LogInfo($"修改已日結退貨單驗退調整成功 (SYSW337): {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改已日結退貨單驗退調整失敗 (SYSW337): {receiptId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 刪除已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    public async Task DeleteClosedReturnAdjustmentV2Async(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment || entity.PurchaseOrderType != "2")
            {
                throw new KeyNotFoundException($"已日結退貨單驗退調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可刪除");
            }

            await _repository.DeleteAsync(receiptId);
            _logger.LogInfo($"刪除已日結退貨單驗退調整成功 (SYSW337): {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除已日結退貨單驗退調整失敗 (SYSW337): {receiptId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 審核已日結退貨單驗退調整 (SYSW337)
    /// </summary>
    public async Task ApproveClosedReturnAdjustmentV2Async(string receiptId, ApproveSettledAdjustmentDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null || !entity.IsSettledAdjustment || entity.PurchaseOrderType != "2")
            {
                throw new KeyNotFoundException($"已日結退貨單驗退調整單不存在: {receiptId}");
            }

            if (entity.Status != "D")
            {
                throw new InvalidOperationException("僅未審核狀態的調整單可審核");
            }

            await _repository.UpdateStatusAsync(receiptId, "A", null); // A: 已審核
            _logger.LogInfo($"審核已日結退貨單驗退調整成功 (SYSW337): {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核已日結退貨單驗退調整失敗 (SYSW337): {receiptId}", ex);
            throw;
        }
    }
}

