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
/// 調撥驗退單服務實作 (SYSW362)
/// </summary>
public class TransferReturnService : BaseService, ITransferReturnService
{
    private readonly ITransferReturnRepository _repository;
    private readonly IStockRepository _stockRepository;
    private readonly ITransferOrderRepository _transferOrderRepository;
    private readonly ITransferReceiptRepository _transferReceiptRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferReturnService(
        ITransferReturnRepository repository,
        IStockRepository stockRepository,
        ITransferOrderRepository transferOrderRepository,
        ITransferReceiptRepository transferReceiptRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger) : base(logger)
    {
        _repository = repository;
        _stockRepository = stockRepository;
        _transferOrderRepository = transferOrderRepository;
        _transferReceiptRepository = transferReceiptRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<PendingTransferOrderForReturnDto>> GetPendingTransfersAsync(PendingTransferOrderForReturnQueryDto query)
    {
        try
        {
            var repositoryQuery = new PendingTransferOrderForReturnQuery
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

            var dtos = items.Select(x => new PendingTransferOrderForReturnDto
            {
                TransferId = x.TransferId,
                TransferDate = x.TransferDate,
                FromShopId = x.FromShopId,
                ToShopId = x.ToShopId,
                Status = x.Status,
                TotalQty = x.TotalQty,
                ReceiptQty = x.ReceiptQty,
                ReturnQty = x.ReturnQty,
                PendingReturnQty = x.PendingReturnQty
            }).ToList();

            return new PagedResult<PendingTransferOrderForReturnDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢待驗退調撥單失敗", ex);
            throw;
        }
    }

    public async Task<PagedResult<TransferReturnDto>> GetTransferReturnsAsync(TransferReturnQueryDto query)
    {
        try
        {
            var repositoryQuery = new TransferReturnQuery
            {
                ReturnId = query.ReturnId,
                TransferId = query.TransferId,
                ReceiptId = query.ReceiptId,
                FromShopId = query.FromShopId,
                ToShopId = query.ToShopId,
                Status = query.Status,
                ReturnDateFrom = query.ReturnDateFrom,
                ReturnDateTo = query.ReturnDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = new List<TransferReturnDto>();
            foreach (var item in items)
            {
                var details = await _repository.GetDetailsByReturnIdAsync(item.ReturnId);
                dtos.Add(MapToDto(item, details));
            }

            return new PagedResult<TransferReturnDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢調撥驗退單失敗", ex);
            throw;
        }
    }

    public async Task<TransferReturnDto> GetTransferReturnByIdAsync(string returnId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(returnId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗退單不存在: {returnId}");
            }

            var details = await _repository.GetDetailsByReturnIdAsync(returnId);
            return MapToDto(entity, details);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢調撥驗退單失敗: {returnId}", ex);
            throw;
        }
    }

    public async Task<TransferReturnDto> CreateReturnFromTransferAsync(string transferId)
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

            // 取得驗收單明細（用於計算可驗退數量）
            var receiptDetails = new List<TransferReceiptDetail>();
            foreach (var transferDetail in detailsList)
            {
                var receipts = await _transferReceiptRepository.GetDetailsByReceiptIdAsync(transferId);
                // 這裡需要根據實際情況調整，可能需要從 TransferReceiptDetails 中查找對應的驗收明細
                // 暫時假設可以從驗收單明細中取得
            }

            // 建立驗退單主檔
            var returnId = await _repository.GenerateReturnIdAsync();
            var entity = new TransferReturn
            {
                ReturnId = returnId,
                TransferId = transferId,
                ReturnDate = DateTime.Now,
                FromShopId = transferOrder.FromShopId,
                ToShopId = transferOrder.ToShopId,
                Status = "P",
                IsSettled = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // 建立驗退單明細（預設帶入可驗退數量）
            var returnDetails = detailsList.Select((x, index) => new TransferReturnDetail
            {
                DetailId = Guid.NewGuid(),
                ReturnId = returnId,
                TransferDetailId = x.DetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                TransferQty = x.TransferQty,
                ReceiptQty = x.ReceiptQty ?? 0, // 已驗收數量
                ReturnQty = 0, // 預設為0，由使用者輸入
                CreatedAt = DateTime.Now
            }).ToList();

            entity.TotalQty = returnDetails.Sum(x => x.ReturnQty);

            // 先建立驗退單（不保存，僅返回 DTO）
            // 實際保存需要呼叫 CreateTransferReturnAsync
            return MapToDto(entity, returnDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError($"依調撥單建立驗退單失敗: {transferId}", ex);
            throw;
        }
    }

    public async Task<string> CreateTransferReturnAsync(CreateTransferReturnDto dto)
    {
        try
        {
            // 取得調撥單資料
            var transferOrder = await _transferOrderRepository.GetTransferOrderAsync(dto.TransferId);
            if (transferOrder == null)
            {
                throw new KeyNotFoundException($"調撥單不存在: {dto.TransferId}");
            }

            var entity = new TransferReturn
            {
                TransferId = dto.TransferId,
                ReceiptId = dto.ReceiptId,
                ReturnDate = dto.ReturnDate,
                FromShopId = transferOrder.FromShopId,
                ToShopId = transferOrder.ToShopId,
                ReturnUserId = dto.ReturnUserId,
                ReturnReason = dto.ReturnReason,
                Memo = dto.Memo,
                Status = "P",
                IsSettled = false,
                CreatedBy = dto.ReturnUserId
            };

            var details = dto.Details.Select((x, index) => new TransferReturnDetail
            {
                TransferDetailId = x.TransferDetailId,
                ReceiptDetailId = x.ReceiptDetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                ReturnQty = x.ReturnQty,
                UnitPrice = x.UnitPrice,
                ReturnReason = x.ReturnReason,
                Memo = x.Memo,
                CreatedBy = dto.ReturnUserId
            }).ToList();

            // 計算總數量和總金額
            entity.TotalQty = details.Sum(x => x.ReturnQty);
            entity.TotalAmount = details.Sum(x => (x.UnitPrice ?? 0) * x.ReturnQty);

            // 驗證驗退數量不超過可驗退數量
            foreach (var detail in details)
            {
                if (detail.ReceiptDetailId.HasValue)
                {
                    var receiptDetail = await _transferReceiptRepository.GetDetailByIdAsync(detail.ReceiptDetailId.Value);
                    if (receiptDetail != null)
                    {
                        // 計算已驗退數量
                        var existingReturns = await _repository.GetDetailsByReturnIdAsync(entity.ReturnId);
                        var returnedQty = existingReturns
                            .Where(r => r.ReceiptDetailId == detail.ReceiptDetailId)
                            .Sum(r => r.ReturnQty);

                        var availableQty = receiptDetail.ReceiptQty - returnedQty;
                        if (detail.ReturnQty > availableQty)
                        {
                            throw new InvalidOperationException($"驗退數量超過可驗退數量: GoodsId={detail.GoodsId}, 可驗退數量={availableQty}, 驗退數量={detail.ReturnQty}");
                        }
                    }
                }
            }

            var returnId = await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立調撥驗退單成功: {returnId}");
            return returnId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立調撥驗退單失敗", ex);
            throw;
        }
    }

    public async Task UpdateTransferReturnAsync(string returnId, UpdateTransferReturnDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(returnId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗退單不存在: {returnId}");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗退單不可修改");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的驗退單不可修改");
            }

            if (entity.Status == "C")
            {
                throw new InvalidOperationException("已確認的驗退單不可修改");
            }

            entity.ReturnDate = dto.ReturnDate;
            entity.ReturnUserId = dto.ReturnUserId;
            entity.ReturnReason = dto.ReturnReason;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = dto.ReturnUserId;

            var existingDetails = await _repository.GetDetailsByReturnIdAsync(returnId);
            var details = dto.Details.Select((x, index) =>
            {
                var existing = existingDetails.FirstOrDefault(d => d.DetailId == x.DetailId);
                return new TransferReturnDetail
                {
                    DetailId = x.DetailId ?? Guid.NewGuid(),
                    ReturnId = returnId,
                    TransferDetailId = existing?.TransferDetailId,
                    ReceiptDetailId = existing?.ReceiptDetailId,
                    LineNum = index + 1,
                    GoodsId = existing?.GoodsId ?? string.Empty,
                    BarcodeId = existing?.BarcodeId,
                    TransferQty = existing?.TransferQty ?? 0,
                    ReceiptQty = existing?.ReceiptQty ?? 0,
                    ReturnQty = x.ReturnQty,
                    UnitPrice = x.UnitPrice,
                    ReturnReason = x.ReturnReason,
                    Memo = x.Memo,
                    CreatedBy = existing?.CreatedBy ?? dto.ReturnUserId,
                    CreatedAt = existing?.CreatedAt ?? DateTime.Now
                };
            }).ToList();

            entity.TotalQty = details.Sum(x => x.ReturnQty);
            entity.TotalAmount = details.Sum(x => (x.UnitPrice ?? 0) * x.ReturnQty);

            await _repository.UpdateAsync(entity, details);
            _logger.LogInfo($"更新調撥驗退單成功: {returnId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新調撥驗退單失敗: {returnId}", ex);
            throw;
        }
    }

    public async Task DeleteTransferReturnAsync(string returnId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(returnId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗退單不存在: {returnId}");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗退單不可刪除");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的驗退單不可刪除");
            }

            if (entity.Status == "C")
            {
                throw new InvalidOperationException("已確認的驗退單不可刪除");
            }

            await _repository.DeleteAsync(returnId);
            _logger.LogInfo($"刪除調撥驗退單成功: {returnId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除調撥驗退單失敗: {returnId}", ex);
            throw;
        }
    }

    public async Task ConfirmReturnAsync(string returnId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var entity = await _repository.GetByIdAsync(returnId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗退單不存在: {returnId}");
            }

            if (entity.Status == "C")
            {
                throw new InvalidOperationException("驗退單已確認");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗退單不可確認");
            }

            // 取得驗退單明細
            var details = await _repository.GetDetailsByReturnIdAsync(returnId);
            var detailsList = details.ToList();

            if (!detailsList.Any())
            {
                throw new InvalidOperationException($"驗退單無明細資料: {returnId}");
            }

            // 1. 更新庫存邏輯（調入庫減少、調出庫增加）
            foreach (var detail in detailsList)
            {
                if (detail.ReturnQty > 0)
                {
                    // 調入庫減少（退回商品）
                    await _stockRepository.UpdateStockQtyAsync(
                        entity.ToShopId, 
                        detail.GoodsId, 
                        -detail.ReturnQty, 
                        transaction);

                    // 調出庫增加（商品退回）
                    await _stockRepository.UpdateStockQtyAsync(
                        entity.FromShopId, 
                        detail.GoodsId, 
                        detail.ReturnQty, 
                        transaction);

                    _logger.LogInfo($"更新庫存: FromShopId={entity.FromShopId}, ToShopId={entity.ToShopId}, GoodsId={detail.GoodsId}, ReturnQty={detail.ReturnQty}");
                }
            }

            // 2. 更新調撥單已退數量
            foreach (var detail in detailsList)
            {
                if (detail.TransferDetailId.HasValue && detail.ReturnQty > 0)
                {
                    await _transferOrderRepository.UpdateReturnQtyAsync(
                        detail.TransferDetailId.Value, 
                        detail.ReturnQty, 
                        transaction);

                    _logger.LogInfo($"更新調撥單已退數量: TransferDetailId={detail.TransferDetailId}, ReturnQty={detail.ReturnQty}");
                }
            }

            // 3. 更新驗退單狀態
            await _repository.UpdateStatusAsync(returnId, "C", transaction);

            transaction.Commit();
            _logger.LogInfo($"確認驗退單成功: {returnId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"確認驗退單失敗: {returnId}", ex);
            throw;
        }
    }

    public async Task CancelTransferReturnAsync(string returnId)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var entity = await _repository.GetByIdAsync(returnId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"驗退單不存在: {returnId}");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("驗退單已取消");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的驗退單不可取消");
            }

            // 如果已確認，需要回退庫存和調撥單已退數量
            if (entity.Status == "C")
            {
                // 取得驗退單明細
                var details = await _repository.GetDetailsByReturnIdAsync(returnId);
                var detailsList = details.ToList();

                // 1. 回退庫存邏輯（調入庫增加、調出庫減少）
                foreach (var detail in detailsList)
                {
                    if (detail.ReturnQty > 0)
                    {
                        // 調入庫增加（回退驗退操作）
                        await _stockRepository.UpdateStockQtyAsync(
                            entity.ToShopId, 
                            detail.GoodsId, 
                            detail.ReturnQty, 
                            transaction);

                        // 調出庫減少（回退驗退操作）
                        await _stockRepository.UpdateStockQtyAsync(
                            entity.FromShopId, 
                            detail.GoodsId, 
                            -detail.ReturnQty, 
                            transaction);

                        _logger.LogInfo($"回退庫存: FromShopId={entity.FromShopId}, ToShopId={entity.ToShopId}, GoodsId={detail.GoodsId}, ReturnQty={detail.ReturnQty}");
                    }
                }

                // 2. 回退調撥單已退數量
                foreach (var detail in detailsList)
                {
                    if (detail.TransferDetailId.HasValue && detail.ReturnQty > 0)
                    {
                        await _transferOrderRepository.UpdateReturnQtyAsync(
                            detail.TransferDetailId.Value, 
                            -detail.ReturnQty, 
                            transaction);

                        _logger.LogInfo($"回退調撥單已退數量: TransferDetailId={detail.TransferDetailId}, ReturnQty={detail.ReturnQty}");
                    }
                }
            }

            // 3. 更新驗退單狀態
            await _repository.UpdateStatusAsync(returnId, "X", transaction);

            transaction.Commit();
            _logger.LogInfo($"取消驗退單成功: {returnId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"取消驗退單失敗: {returnId}", ex);
            throw;
        }
    }

    private TransferReturnDto MapToDto(TransferReturn entity, IEnumerable<TransferReturnDetail> details)
    {
        return new TransferReturnDto
        {
            ReturnId = entity.ReturnId,
            TransferId = entity.TransferId,
            ReceiptId = entity.ReceiptId,
            ReturnDate = entity.ReturnDate,
            FromShopId = entity.FromShopId,
            ToShopId = entity.ToShopId,
            Status = entity.Status,
            ReturnUserId = entity.ReturnUserId,
            TotalAmount = entity.TotalAmount,
            TotalQty = entity.TotalQty,
            ReturnReason = entity.ReturnReason,
            Memo = entity.Memo,
            IsSettled = entity.IsSettled,
            SettledDate = entity.SettledDate,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,
            Details = details.Select(x => new TransferReturnDetailDto
            {
                DetailId = x.DetailId,
                ReturnId = x.ReturnId,
                TransferDetailId = x.TransferDetailId,
                ReceiptDetailId = x.ReceiptDetailId,
                LineNum = x.LineNum,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                TransferQty = x.TransferQty,
                ReceiptQty = x.ReceiptQty,
                ReturnQty = x.ReturnQty,
                UnitPrice = x.UnitPrice,
                Amount = x.Amount,
                ReturnReason = x.ReturnReason,
                Memo = x.Memo,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList()
        };
    }
}
