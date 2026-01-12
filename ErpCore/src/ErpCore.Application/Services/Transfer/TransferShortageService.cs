using System.Data;
using Dapper;
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
/// 調撥短溢單服務實作
/// </summary>
public class TransferShortageService : BaseService, ITransferShortageService
{
    private readonly ITransferShortageRepository _repository;
    private readonly IStockRepository _stockRepository;
    private readonly ITransferOrderRepository _transferOrderRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public TransferShortageService(
        ITransferShortageRepository repository,
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

    public async Task<PagedResult<TransferShortageDto>> GetTransferShortagesAsync(TransferShortageQueryDto query)
    {
        try
        {
            var repositoryQuery = new TransferShortageQuery
            {
                ShortageId = query.ShortageId,
                TransferId = query.TransferId,
                FromShopId = query.FromShopId,
                ToShopId = query.ToShopId,
                Status = query.Status,
                ProcessType = query.ProcessType,
                ShortageDateFrom = query.ShortageDateFrom,
                ShortageDateTo = query.ShortageDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = new List<TransferShortageDto>();
            foreach (var item in items)
            {
                var details = await _repository.GetDetailsByShortageIdAsync(item.ShortageId);
                dtos.Add(MapToDto(item, details));
            }

            return new PagedResult<TransferShortageDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢調撥短溢單失敗", ex);
            throw;
        }
    }

    public async Task<TransferShortageDto> GetTransferShortageByIdAsync(string shortageId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(shortageId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"短溢單不存在: {shortageId}");
            }

            var details = await _repository.GetDetailsByShortageIdAsync(shortageId);
            return MapToDto(entity, details);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢調撥短溢單失敗: {shortageId}", ex);
            throw;
        }
    }

    public async Task<TransferShortageDto> CreateShortageFromTransferAsync(string transferId)
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

            // 建立短溢單主檔
            var shortageId = await _repository.GenerateShortageIdAsync();
            var entity = new TransferShortage
            {
                ShortageId = shortageId,
                TransferId = transferId,
                ShortageDate = DateTime.Now,
                FromShopId = transferOrder.FromShopId,
                ToShopId = transferOrder.ToShopId,
                Status = "P",
                IsSettled = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // 建立短溢單明細（預設帶入調撥數量、驗收數量，短溢數量需手動輸入）
            var shortageDetails = detailsList.Select((x, index) => new TransferShortageDetail
            {
                DetailId = Guid.NewGuid(),
                ShortageId = shortageId,
                TransferDetailId = x.DetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                TransferQty = x.TransferQty,
                ReceiptQty = x.ReceiptQty ?? 0,
                ShortageQty = 0, // 預設為0，需手動輸入
                CreatedAt = DateTime.Now
            }).ToList();

            entity.TotalShortageQty = shortageDetails.Sum(x => x.ShortageQty);

            // 先建立短溢單（不保存，僅返回 DTO）
            // 實際保存需要呼叫 CreateTransferShortageAsync
            return MapToDto(entity, shortageDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError($"依調撥單建立短溢單失敗: {transferId}", ex);
            throw;
        }
    }

    public async Task<string> CreateTransferShortageAsync(CreateTransferShortageDto dto)
    {
        try
        {
            // 取得調撥單資料
            var transferOrder = await _transferOrderRepository.GetTransferOrderAsync(dto.TransferId);
            if (transferOrder == null)
            {
                throw new KeyNotFoundException($"調撥單不存在: {dto.TransferId}");
            }

            var entity = new TransferShortage
            {
                TransferId = dto.TransferId,
                ReceiptId = dto.ReceiptId,
                ShortageDate = dto.ShortageDate,
                FromShopId = transferOrder.FromShopId,
                ToShopId = transferOrder.ToShopId,
                ProcessType = dto.ProcessType,
                ShortageReason = dto.ShortageReason,
                Memo = dto.Memo,
                Status = "P",
                IsSettled = false,
                CreatedBy = dto.ShortageReason // 暫時使用，實際應從當前使用者取得
            };

            var details = dto.Details.Select((x, index) => new TransferShortageDetail
            {
                TransferDetailId = x.TransferDetailId,
                ReceiptDetailId = x.ReceiptDetailId,
                LineNum = index + 1,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                TransferQty = x.TransferQty,
                ReceiptQty = x.ReceiptQty,
                ShortageQty = x.ShortageQty,
                UnitPrice = x.UnitPrice,
                ShortageReason = x.ShortageReason,
                Memo = x.Memo,
                CreatedBy = entity.CreatedBy
            }).ToList();

            // 驗證短溢數量不可為0
            if (details.All(x => x.ShortageQty == 0))
            {
                throw new InvalidOperationException("短溢數量不可全部為0");
            }

            entity.TotalShortageQty = details.Sum(x => x.ShortageQty);
            entity.TotalAmount = details.Sum(x => (x.UnitPrice ?? 0) * Math.Abs(x.ShortageQty));

            var shortageId = await _repository.CreateAsync(entity, details);
            _logger.LogInfo($"建立調撥短溢單成功: {shortageId}");
            return shortageId;
        }
        catch (Exception ex)
        {
            _logger.LogError("建立調撥短溢單失敗", ex);
            throw;
        }
    }

    public async Task UpdateTransferShortageAsync(string shortageId, UpdateTransferShortageDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(shortageId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"短溢單不存在: {shortageId}");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的短溢單不可修改");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的短溢單不可修改");
            }

            if (entity.Status == "A" || entity.Status == "C")
            {
                throw new InvalidOperationException("已審核或已處理的短溢單不可修改");
            }

            entity.ShortageDate = dto.ShortageDate;
            entity.ProcessType = dto.ProcessType;
            entity.ShortageReason = dto.ShortageReason;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = entity.CreatedBy; // 暫時使用，實際應從當前使用者取得

            var existingDetails = await _repository.GetDetailsByShortageIdAsync(shortageId);
            var details = dto.Details.Select((x, index) =>
            {
                var existing = existingDetails.FirstOrDefault(d => d.DetailId == x.DetailId);
                return new TransferShortageDetail
                {
                    DetailId = x.DetailId ?? Guid.NewGuid(),
                    ShortageId = shortageId,
                    TransferDetailId = existing?.TransferDetailId,
                    ReceiptDetailId = existing?.ReceiptDetailId,
                    LineNum = index + 1,
                    GoodsId = existing?.GoodsId ?? string.Empty,
                    BarcodeId = existing?.BarcodeId,
                    TransferQty = x.TransferQty,
                    ReceiptQty = x.ReceiptQty,
                    ShortageQty = x.ShortageQty,
                    UnitPrice = x.UnitPrice,
                    ShortageReason = x.ShortageReason,
                    Memo = x.Memo,
                    CreatedBy = existing?.CreatedBy ?? entity.CreatedBy,
                    CreatedAt = existing?.CreatedAt ?? DateTime.Now
                };
            }).ToList();

            // 驗證短溢數量不可為0
            if (details.All(x => x.ShortageQty == 0))
            {
                throw new InvalidOperationException("短溢數量不可全部為0");
            }

            entity.TotalShortageQty = details.Sum(x => x.ShortageQty);
            entity.TotalAmount = details.Sum(x => (x.UnitPrice ?? 0) * Math.Abs(x.ShortageQty));

            await _repository.UpdateAsync(entity, details);
            _logger.LogInfo($"更新調撥短溢單成功: {shortageId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新調撥短溢單失敗: {shortageId}", ex);
            throw;
        }
    }

    public async Task DeleteTransferShortageAsync(string shortageId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(shortageId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"短溢單不存在: {shortageId}");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的短溢單不可刪除");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的短溢單不可刪除");
            }

            if (entity.Status == "A" || entity.Status == "C")
            {
                throw new InvalidOperationException("已審核或已處理的短溢單不可刪除");
            }

            await _repository.DeleteAsync(shortageId);
            _logger.LogInfo($"刪除調撥短溢單成功: {shortageId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除調撥短溢單失敗: {shortageId}", ex);
            throw;
        }
    }

    public async Task ApproveTransferShortageAsync(string shortageId, ApproveTransferShortageDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(shortageId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"短溢單不存在: {shortageId}");
            }

            if (entity.Status == "A")
            {
                throw new InvalidOperationException("短溢單已審核");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的短溢單不可審核");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的短溢單不可審核");
            }

            entity.Status = "A";
            entity.ApproveUserId = dto.ApproveUserId;
            entity.ApproveDate = dto.ApproveDate;
            entity.UpdatedBy = dto.ApproveUserId;
            entity.UpdatedAt = DateTime.Now;

            // 更新主檔
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                const string updateSql = @"
                    UPDATE TransferShortages SET
                        Status = @Status,
                        ApproveUserId = @ApproveUserId,
                        ApproveDate = @ApproveDate,
                        UpdatedBy = @UpdatedBy,
                        UpdatedAt = @UpdatedAt
                    WHERE ShortageId = @ShortageId";

                await connection.ExecuteAsync(updateSql, entity, transaction);
                transaction.Commit();
                _logger.LogInfo($"審核短溢單成功: {shortageId}");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError($"審核短溢單失敗: {shortageId}", ex);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核短溢單失敗: {shortageId}", ex);
            throw;
        }
    }

    public async Task ProcessTransferShortageAsync(string shortageId, ProcessTransferShortageDto dto)
    {
        using var connection = _connectionFactory.CreateConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            var entity = await _repository.GetByIdAsync(shortageId);
            if (entity == null)
            {
                throw new KeyNotFoundException($"短溢單不存在: {shortageId}");
            }

            if (entity.Status == "C")
            {
                throw new InvalidOperationException("短溢單已處理");
            }

            if (entity.IsSettled)
            {
                throw new InvalidOperationException("已日結的短溢單不可處理");
            }

            if (entity.Status == "X")
            {
                throw new InvalidOperationException("已取消的短溢單不可處理");
            }

            // 取得短溢單明細
            var details = await _repository.GetDetailsByShortageIdAsync(shortageId);
            var detailsList = details.ToList();

            if (!detailsList.Any())
            {
                throw new InvalidOperationException($"短溢單無明細資料: {shortageId}");
            }

            // 如果處理方式為調整庫存，則更新庫存
            if (dto.ProcessType == "ADJUST")
            {
                foreach (var detail in detailsList)
                {
                    if (detail.ShortageQty != 0)
                    {
                        // 短少時（ShortageQty < 0）：調入庫減少，調出庫增加
                        // 溢收時（ShortageQty > 0）：調入庫增加，調出庫減少
                        await _stockRepository.UpdateStockQtyAsync(
                            entity.ToShopId,
                            detail.GoodsId,
                            detail.ShortageQty, // 調入庫：短少為負數（減少），溢收為正數（增加）
                            transaction);

                        await _stockRepository.UpdateStockQtyAsync(
                            entity.FromShopId,
                            detail.GoodsId,
                            -detail.ShortageQty, // 調出庫：短少為正數（增加），溢收為負數（減少）
                            transaction);

                        _logger.LogInfo($"更新庫存: ToShopId={entity.ToShopId}, FromShopId={entity.FromShopId}, GoodsId={detail.GoodsId}, ShortageQty={detail.ShortageQty}");
                    }
                }
            }

            // 更新短溢單狀態和處理資訊
            entity.Status = "C";
            entity.ProcessType = dto.ProcessType;
            entity.ProcessUserId = dto.ProcessUserId;
            entity.ProcessDate = dto.ProcessDate;
            entity.UpdatedBy = dto.ProcessUserId;
            entity.UpdatedAt = DateTime.Now;

            const string updateSql = @"
                UPDATE TransferShortages SET
                    Status = @Status,
                    ProcessType = @ProcessType,
                    ProcessUserId = @ProcessUserId,
                    ProcessDate = @ProcessDate,
                    UpdatedBy = @UpdatedBy,
                    UpdatedAt = @UpdatedAt
                WHERE ShortageId = @ShortageId";

            await connection.ExecuteAsync(updateSql, entity, transaction);

            transaction.Commit();
            _logger.LogInfo($"處理短溢單成功: {shortageId}");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            _logger.LogError($"處理短溢單失敗: {shortageId}", ex);
            throw;
        }
    }

    private TransferShortageDto MapToDto(TransferShortage entity, IEnumerable<TransferShortageDetail> details)
    {
        return new TransferShortageDto
        {
            ShortageId = entity.ShortageId,
            TransferId = entity.TransferId,
            ReceiptId = entity.ReceiptId,
            ShortageDate = entity.ShortageDate,
            FromShopId = entity.FromShopId,
            ToShopId = entity.ToShopId,
            Status = entity.Status,
            ProcessType = entity.ProcessType,
            ProcessUserId = entity.ProcessUserId,
            ProcessDate = entity.ProcessDate,
            ApproveUserId = entity.ApproveUserId,
            ApproveDate = entity.ApproveDate,
            TotalShortageQty = entity.TotalShortageQty,
            TotalAmount = entity.TotalAmount,
            ShortageReason = entity.ShortageReason,
            Memo = entity.Memo,
            IsSettled = entity.IsSettled,
            SettledDate = entity.SettledDate,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt,
            Details = details.Select(x => new TransferShortageDetailDto
            {
                DetailId = x.DetailId,
                ShortageId = x.ShortageId,
                TransferDetailId = x.TransferDetailId,
                ReceiptDetailId = x.ReceiptDetailId,
                LineNum = x.LineNum,
                GoodsId = x.GoodsId,
                BarcodeId = x.BarcodeId,
                TransferQty = x.TransferQty,
                ReceiptQty = x.ReceiptQty,
                ShortageQty = x.ShortageQty,
                UnitPrice = x.UnitPrice,
                Amount = x.Amount,
                ShortageReason = x.ShortageReason,
                Memo = x.Memo,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt
            }).ToList()
        };
    }
}
