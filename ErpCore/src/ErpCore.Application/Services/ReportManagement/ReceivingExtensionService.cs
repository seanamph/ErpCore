using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Repositories.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 收款擴展功能服務實作 (SYSR310-SYSR450)
/// </summary>
public class ReceivingExtensionService : BaseService, IReceivingExtensionService
{
    private readonly IReceiptVoucherTransferRepository _repository;

    public ReceivingExtensionService(
        IReceiptVoucherTransferRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<ReceiptVoucherTransferDto> GetReceiptVoucherTransferByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款沖帳傳票不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款沖帳傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PagedResult<ReceiptVoucherTransferDto>> QueryReceiptVoucherTransferAsync(ReceiptVoucherTransferQueryDto query)
    {
        try
        {
            var repositoryQuery = new ReceiptVoucherTransferQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortBy = query.SortBy,
                SortOrder = query.SortOrder,
                ReceiptNo = query.ReceiptNo,
                VoucherNo = query.VoucherNo,
                ReceiptDateFrom = query.ReceiptDateFrom,
                ReceiptDateTo = query.ReceiptDateTo,
                TransferStatus = query.TransferStatus,
                ObjectId = query.ObjectId,
                ShopId = query.ShopId,
                SiteId = query.SiteId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(MapToDto).ToList();

            return new PagedResult<ReceiptVoucherTransferDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢收款沖帳傳票列表失敗", ex);
            throw;
        }
    }

    public async Task<ReceiptVoucherTransferDto> CreateReceiptVoucherTransferAsync(CreateReceiptVoucherTransferDto dto)
    {
        try
        {
            var entity = new ReceiptVoucherTransfer
            {
                ReceiptNo = dto.ReceiptNo,
                ReceiptDate = dto.ReceiptDate,
                ObjectId = dto.ObjectId,
                AcctKey = dto.AcctKey,
                ReceiptAmount = dto.ReceiptAmount,
                AritemId = dto.AritemId,
                ShopId = dto.ShopId,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                Notes = dto.Notes,
                TransferStatus = "P", // 待拋轉
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增收款沖帳傳票失敗", ex);
            throw;
        }
    }

    public async Task<ReceiptVoucherTransferDto> UpdateReceiptVoucherTransferAsync(long tKey, UpdateReceiptVoucherTransferDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款沖帳傳票不存在: {tKey}");
            }

            // 只有待拋轉狀態才能修改
            if (entity.TransferStatus != "P")
            {
                throw new InvalidOperationException($"只有待拋轉狀態的收款沖帳傳票才能修改");
            }

            if (dto.ReceiptNo != null) entity.ReceiptNo = dto.ReceiptNo;
            if (dto.ReceiptDate.HasValue) entity.ReceiptDate = dto.ReceiptDate.Value;
            if (dto.ObjectId != null) entity.ObjectId = dto.ObjectId;
            if (dto.AcctKey != null) entity.AcctKey = dto.AcctKey;
            if (dto.ReceiptAmount.HasValue) entity.ReceiptAmount = dto.ReceiptAmount.Value;
            if (dto.AritemId != null) entity.AritemId = dto.AritemId;
            if (dto.VoucherNo != null) entity.VoucherNo = dto.VoucherNo;
            if (dto.VoucherM_TKey.HasValue) entity.VoucherM_TKey = dto.VoucherM_TKey;
            if (dto.TransferStatus != null) entity.TransferStatus = dto.TransferStatus;
            if (dto.TransferDate.HasValue) entity.TransferDate = dto.TransferDate;
            if (dto.TransferUser != null) entity.TransferUser = dto.TransferUser;
            if (dto.ErrorMessage != null) entity.ErrorMessage = dto.ErrorMessage;
            if (dto.Notes != null) entity.Notes = dto.Notes;

            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新收款沖帳傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteReceiptVoucherTransferAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款沖帳傳票不存在: {tKey}");
            }

            // 只有待拋轉狀態才能刪除
            if (entity.TransferStatus != "P")
            {
                throw new InvalidOperationException($"只有待拋轉狀態的收款沖帳傳票才能刪除");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除收款沖帳傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ReceiptVoucherTransferDto> TransferReceiptVoucherAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款沖帳傳票不存在: {tKey}");
            }

            if (entity.TransferStatus != "P")
            {
                throw new InvalidOperationException($"只有待拋轉狀態的收款沖帳傳票才能拋轉");
            }

            // TODO: 實作拋轉邏輯，產生傳票單號
            // 這裡簡化處理，實際應該呼叫傳票服務產生傳票
            var voucherNo = $"VCH{DateTime.Now:yyyyMMdd}{tKey:D6}";
            var voucherM_TKey = 0L; // 實際應該從傳票服務取得

            entity.VoucherNo = voucherNo;
            entity.VoucherM_TKey = voucherM_TKey;
            entity.TransferStatus = "S"; // 已拋轉
            entity.TransferDate = DateTime.Now;
            entity.TransferUser = GetCurrentUserId();
            entity.ErrorMessage = null;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"拋轉收款沖帳傳票失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<BatchTransferResultDto> BatchTransferReceiptVoucherAsync(BatchTransferReceiptVoucherDto dto)
    {
        try
        {
            var result = new BatchTransferResultDto
            {
                Total = dto.ReceiptNos.Count,
                Results = new List<TransferResultItemDto>()
            };

            foreach (var receiptNo in dto.ReceiptNos)
            {
                try
                {
                    var entities = await _repository.GetByReceiptNoAsync(receiptNo);
                    var entity = entities.FirstOrDefault(x => x.TransferStatus == "P");

                    if (entity == null)
                    {
                        result.Results.Add(new TransferResultItemDto
                        {
                            ReceiptNo = receiptNo,
                            Status = "failed",
                            ErrorMessage = "找不到待拋轉的收款沖帳傳票"
                        });
                        result.Failed++;
                        continue;
                    }

                    var transferResult = await TransferReceiptVoucherAsync(entity.TKey);
                    result.Results.Add(new TransferResultItemDto
                    {
                        ReceiptNo = receiptNo,
                        Status = "success",
                        VoucherNo = transferResult.VoucherNo
                    });
                    result.Success++;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"批次拋轉收款沖帳傳票失敗: ReceiptNo={receiptNo}", ex);
                    result.Results.Add(new TransferResultItemDto
                    {
                        ReceiptNo = receiptNo,
                        Status = "failed",
                        ErrorMessage = ex.Message
                    });
                    result.Failed++;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError("批次拋轉收款沖帳傳票失敗", ex);
            throw;
        }
    }

    private ReceiptVoucherTransferDto MapToDto(ReceiptVoucherTransfer entity)
    {
        return new ReceiptVoucherTransferDto
        {
            TKey = entity.TKey,
            ReceiptNo = entity.ReceiptNo,
            ReceiptDate = entity.ReceiptDate,
            ObjectId = entity.ObjectId,
            AcctKey = entity.AcctKey,
            ReceiptAmount = entity.ReceiptAmount,
            AritemId = entity.AritemId,
            VoucherNo = entity.VoucherNo,
            VoucherM_TKey = entity.VoucherM_TKey,
            TransferStatus = entity.TransferStatus,
            TransferStatusName = entity.TransferStatus switch
            {
                "P" => "待拋轉",
                "S" => "已拋轉",
                "F" => "失敗",
                _ => entity.TransferStatus
            },
            TransferDate = entity.TransferDate,
            TransferUser = entity.TransferUser,
            ErrorMessage = entity.ErrorMessage,
            ShopId = entity.ShopId,
            SiteId = entity.SiteId,
            OrgId = entity.OrgId,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

