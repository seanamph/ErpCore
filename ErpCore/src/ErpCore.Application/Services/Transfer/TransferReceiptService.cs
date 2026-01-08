using System.Data;
using ErpCore.Application.DTOs.Transfer;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Transfer;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Inventory;
using ErpCore.Infrastructure.Repositories.Transfer;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Transfer;

/// <summary>
/// 調撥驗收單服務實作
/// </summary>
public class TransferReceiptService : BaseService, ITransferReceiptService
{
    private readonly ITransferReceiptRepository _repository;
    private readonly IStockRepository _stockRepository;
    private readonly ITransferOrderRepository _transferOrderRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferReceiptService(
        ITransferReceiptRepository repository,
        IStockRepository stockRepository,
        ITransferOrderRepository transferOrderRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger) : base(logger)
    {
        _repository = repository;
        _stockRepository = stockRepository;
        _transferOrderRepository = transferOrderRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PendingTransferOrderDto>> GetPendingOrdersAsync(PendingTransferOrderQueryDto query)
    {
        try
        {
            var repositoryQuery = new PendingTransferOrderQuery
            {
                TransferId = query.TransferId,
                FromShopId = query.FromShopId,
                ToShopId = query.ToShopId,
                TransferDateFrom = query.TransferDateFrom,
                TransferDateTo = query.TransferDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.GetPendingOrdersAsync(repositoryQuery);
            var totalCount = await _repository.GetPendingOrdersCountAsync(repositoryQuery);

            var dtos = items.Select(x => new PendingTransferOrderDto
            {
                TransferId = x.TransferId,
                TransferDate = x.TransferDate,
                FromShopId = x.FromShopId,
                ToShopId = x.ToShopId,
                Status = x.Status,
                TotalQty = x.TotalQty,
                ReceiptQty = x.ReceiptQty,
                PendingReceiptQty = x.PendingReceiptQty
            }).ToList();

            return new PagedResult<PendingTransferOrderDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢待驗收調撥單失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<TransferReceiptDto>> GetTransferReceiptsAsync(TransferReceiptQueryDto query)
    {
        try
        {
            var repositoryQuery = new TransferReceiptQuery
            {
                ReceiptId = query.ReceiptId,
                TransferId = query.TransferId,
                FromShopId = query.FromShopId,
                ToShopId = query.ToShopId,
                Status = query.Status,
                ReceiptDateFrom = query.ReceiptDateFrom,
                ReceiptDateTo = query.ReceiptDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = new List<TransferReceiptDto>();
            foreach (var item in items)
            {
                var details = await _repository.GetDetailsByReceiptIdAsync(item.ReceiptId);
                dtos.Add(MapToDto(item, details));
            }

            return new PagedResult<TransferReceiptDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢調撥驗收單失敗", ex);
            throw;
        }
    }

    public async Task<TransferReceiptDto> GetTransferReceiptByIdAsync(string receiptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(receiptId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗收單不存在: {receiptId}");
            }

            var details = await _repository.GetDetailsByReceiptIdAsync(receiptId);
            return MapToDto(entity, details);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢調撥驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task<TransferReceiptDto> CreateReceiptFromOrderAsync(string transferId)
    {
        try
        {
            // 取得調撥單資料
            var transferOrder = await _transferOrderRepository.GetTransferOrderAsync(transferId);
            if (transferOrder == null)
            {
                throw new KeyNotFoundException($"調撥單不存在: {transferId}");
            }

            // 取得調撥單明細
            var transferDetails = await _transferOrderRepository.GetTransferOrderDetailsAsync(transferId);
            var detailsList = transferDetails.ToList();
            if (!detailsList.Any())
            {
                throw new InvalidOperationException($"調撥單無明細資料: {transferId}");
            }

            // 建立驗收單主檔
            var receiptId = await _repository.GenerateReceiptIdAsync();
            var entity = new TransferReceipt
            {
                ReceiptId = receiptId,
                TransferId = transferId,
                ReceiptDate = DateTime.Now,
                FromShopId = transferOrder.FromShopId,
                ToShopId = transferOrder.ToShopId,
                Status = "P",
                IsSettled = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // 建立驗收單明細（預設帶入可驗收數量）
            var receiptDetails = detailsList.Select((x, index) => new TransferReceiptDetail
            {
                DetailId = Guid.NewGuid(),
                ReceiptId = receiptId,
                TransferDetailId = x.DetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                TransferQty = x.TransferQty,
                ReceiptQty = x.TransferQty - (x.ReceiptQty ?? 0), // 可驗收數量 = 調撥數量 - 已驗收數量
                CreatedAt = DateTime.Now
            }).ToList();

            entity.TotalQty = receiptDetails.Sum(x => x.ReceiptQty);

            // 先建立驗收單（不保存，僅返回 DTO）
            // 實際保存需要呼叫 CreateTransferReceiptAsync
            return MapToDto(entity, receiptDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError($"依調撥單建立驗收單失敗: {transferId}", ex);
            throw;
        }
    }

    public async Task<string> CreateTransferReceiptAsync(CreateTransferReceiptDto dto)
    {
        try
        {
            var entity = new TransferReceipt
            {
                TransferId = dto.TransferId,
                ReceiptDate = dto.ReceiptDate,
                ReceiptUserId = dto.ReceiptUserId,
                Memo = dto.Memo,
                Status = "P",
                IsSettled = false,
                CreatedBy = dto.ReceiptUserId
            };

            var details = dto.Details.Select((x, index) => new TransferReceiptDetail
            {
                TransferDetailId = x.TransferDetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                ReceiptQty = x.ReceiptQty,
                UnitPrice = x.UnitPrice,
                Memo = x.Memo,
                CreatedBy = dto.ReceiptUserId
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => (x.UnitPrice ?? 0) * x.ReceiptQty);

            var receiptId = await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立調撥驗收單成功: {receiptId}");
            return receiptId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立調撥驗收單失敗", ex);
            throw;
        }
    }

    public async Task UpdateTransferReceiptAsync(string receiptId, UpdateTransferReceiptDto dto)
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
                return new TransferReceiptDetail
                {
                    DetailId = x.DetailId ?? Guid.NewGuid(),
                    ReceiptId = receiptId,
                    TransferDetailId = existing?.TransferDetailId,
                    LineNum = index + 1,
                    GoodsId = existing?.GoodsId ?? string.Empty,
                    BarcodeId = existing?.BarcodeId,
                    TransferQty = existing?.TransferQty ?? 0,
                    ReceiptQty = x.ReceiptQty,
                    UnitPrice = x.UnitPrice,
                    Memo = x.Memo,
                    CreatedBy = existing?.CreatedBy ?? dto.ReceiptUserId,
                    CreatedAt = existing?.CreatedAt ?? DateTime.Now
                };
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReceiptQty);
            entity.TotalAmount = details.Sum(x => (x.UnitPrice ?? 0) * x.ReceiptQty);

            await _repository.UpdateAsync(entity, details);
            _logger.LogInfo($"更新調撥驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新調撥驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task DeleteTransferReceiptAsync(string receiptId)
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
            _logger.LogInfo($"刪除調撥驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除調撥驗收單失敗: {receiptId}", ex);
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

            // 1. 更新庫存邏輯（調出庫減少、調入庫增加）
            foreach (var detail in detailsList)
            {
                if (detail.ReceiptQty > 0)
                {
                    // 調出庫減少（商品調出）
                    await _stockRepository.UpdateStockQtyAsync(
                        entity.FromShopId, 
                        detail.GoodsId, 
                        -detail.ReceiptQty, 
                        transaction);

                    // 調入庫增加（商品調入）
                    await _stockRepository.UpdateStockQtyAsync(
                        entity.ToShopId, 
                        detail.GoodsId, 
                        detail.ReceiptQty, 
                        transaction);

                    _logger.LogInfo($"更新庫存: FromShopId={entity.FromShopId}, ToShopId={entity.ToShopId}, GoodsId={detail.GoodsId}, ReceiptQty={detail.ReceiptQty}");
                }
            }

            // 2. 更新調撥單已收數量
            foreach (var detail in detailsList)
            {
                if (detail.TransferDetailId.HasValue && detail.ReceiptQty > 0)
                {
                    await _transferOrderRepository.UpdateReceiptQtyAsync(
                        detail.TransferDetailId.Value, 
                        detail.ReceiptQty, 
                        transaction);

                    _logger.LogInfo($"更新調撥單已收數量: TransferDetailId={detail.TransferDetailId}, ReceiptQty={detail.ReceiptQty}");
                }
            }

            // 3. 更新驗收單狀態
            await _repository.UpdateStatusAsync(receiptId, "C", transaction);

            transaction.Commit();
            _logger.LogInfo($"確認驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"確認驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    public async Task CancelTransferReceiptAsync(string receiptId)
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

            // 如果已確認，需要回退庫存和調撥單已收數量
            if (entity.Status == "C")
            {
                // 取得驗收單明細
                var details = await _repository.GetDetailsByReceiptIdAsync(receiptId);
                var detailsList = details.ToList();

                // 1. 回退庫存邏輯（調出庫增加、調入庫減少）
                foreach (var detail in detailsList)
                {
                    if (detail.ReceiptQty > 0)
                    {
                        // 調出庫增加（回退驗收操作）
                        await _stockRepository.UpdateStockQtyAsync(
                            entity.FromShopId, 
                            detail.GoodsId, 
                            detail.ReceiptQty, 
                            transaction);

                        // 調入庫減少（回退驗收操作）
                        await _stockRepository.UpdateStockQtyAsync(
                            entity.ToShopId, 
                            detail.GoodsId, 
                            -detail.ReceiptQty, 
                            transaction);

                        _logger.LogInfo($"回退庫存: FromShopId={entity.FromShopId}, ToShopId={entity.ToShopId}, GoodsId={detail.GoodsId}, ReceiptQty={detail.ReceiptQty}");
                    }
                }

                // 2. 回退調撥單已收數量
                foreach (var detail in detailsList)
                {
                    if (detail.TransferDetailId.HasValue && detail.ReceiptQty > 0)
                    {
                        await _transferOrderRepository.UpdateReceiptQtyAsync(
                            detail.TransferDetailId.Value, 
                            -detail.ReceiptQty, 
                            transaction);

                        _logger.LogInfo($"回退調撥單已收數量: TransferDetailId={detail.TransferDetailId}, ReceiptQty={detail.ReceiptQty}");
                    }
                }
            }

            // 3. 更新驗收單狀態
            await _repository.UpdateStatusAsync(receiptId, "X", transaction);

            transaction.Commit();
            _logger.LogInfo($"取消驗收單成功: {receiptId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"取消驗收單失敗: {receiptId}", ex);
            throw;
        }
    }

    private TransferReceiptDto MapToDto(TransferReceipt entity, IEnumerable<TransferReceiptDetail> details)
    {
        return new TransferReceiptDto
        {
            ReceiptId = entity.ReceiptId,
            TransferId = entity.TransferId,
            ReceiptDate = entity.ReceiptDate,
            FromShopId = entity.FromShopId,
            ToShopId = entity.ToShopId,
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
            Details = details.Select(x => new TransferReceiptDetailDto
            {
                DetailId = x.DetailId,
                ReceiptId = x.ReceiptId,
                TransferDetailId = x.TransferDetailId,
                LineNum = x.LineNum,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                TransferQty = x.TransferQty,
                ReceiptQty = x.ReceiptQty,
                UnitPrice = x.UnitPrice,
                Amount = x.Amount,
                Memo = x.Memo,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList()
        };
    }
}

