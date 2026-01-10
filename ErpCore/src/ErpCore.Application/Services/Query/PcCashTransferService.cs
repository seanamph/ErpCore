using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金拋轉檔服務實作 (SYSQ230)
/// </summary>
public class PcCashTransferService : BaseService, IPcCashTransferService
{
    private readonly IPcCashTransferRepository _repository;

    public PcCashTransferService(
        IPcCashTransferRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PcCashTransferDto>> QueryAsync(PcCashTransferQueryDto query)
    {
        try
        {
            return await _repository.QueryAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金拋轉檔列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCashTransferDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金拋轉檔不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金拋轉檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCashTransferDto> GetByTransferIdAsync(string transferId)
    {
        try
        {
            var entity = await _repository.GetByTransferIdAsync(transferId);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金拋轉檔不存在: {transferId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金拋轉檔失敗: {transferId}", ex);
            throw;
        }
    }

    public async Task<PcCashTransferDto> CreateAsync(CreatePcCashTransferDto dto)
    {
        try
        {
            var transferId = await _repository.GenerateTransferIdAsync(dto.SiteId);

            var entity = new PcCashTransfer
            {
                TransferId = transferId,
                SiteId = dto.SiteId,
                TransferDate = dto.TransferDate,
                VoucherId = dto.VoucherId,
                VoucherKind = dto.VoucherKind,
                VoucherDate = dto.VoucherDate,
                TransferAmount = dto.TransferAmount,
                TransferStatus = "DRAFT",
                Notes = dto.Notes,
                BUser = GetCurrentUserId(),
                BTime = DateTime.Now,
                CUser = GetCurrentUserId(),
                CTime = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增零用金拋轉檔失敗", ex);
            throw;
        }
    }

    public async Task<PcCashTransferDto> UpdateAsync(long tKey, UpdatePcCashTransferDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金拋轉檔不存在: {tKey}");
            }

            entity.TransferDate = dto.TransferDate;
            entity.VoucherId = dto.VoucherId;
            entity.VoucherKind = dto.VoucherKind;
            entity.VoucherDate = dto.VoucherDate;
            entity.TransferAmount = dto.TransferAmount;
            entity.TransferStatus = dto.TransferStatus ?? entity.TransferStatus;
            entity.Notes = dto.Notes;
            entity.CUser = GetCurrentUserId();
            entity.CTime = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金拋轉檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金拋轉檔不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金拋轉檔失敗: {tKey}", ex);
            throw;
        }
    }

    private PcCashTransferDto MapToDto(PcCashTransfer entity)
    {
        return new PcCashTransferDto
        {
            TKey = entity.TKey,
            TransferId = entity.TransferId,
            SiteId = entity.SiteId,
            TransferDate = entity.TransferDate,
            VoucherId = entity.VoucherId,
            VoucherKind = entity.VoucherKind,
            VoucherDate = entity.VoucherDate,
            TransferAmount = entity.TransferAmount,
            TransferStatus = entity.TransferStatus,
            Notes = entity.Notes,
            BUser = entity.BUser,
            BTime = entity.BTime,
            CUser = entity.CUser,
            CTime = entity.CTime
        };
    }
}

