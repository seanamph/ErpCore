using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 傳票審核傳送檔服務實作 (SYSQ250)
/// </summary>
public class VoucherAuditService : BaseService, IVoucherAuditService
{
    private readonly IVoucherAuditRepository _repository;

    public VoucherAuditService(
        IVoucherAuditRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<VoucherAuditDto>> QueryAsync(VoucherAuditQueryDto query)
    {
        try
        {
            return await _repository.QueryAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢傳票審核傳送檔列表失敗", ex);
            throw;
        }
    }

    public async Task<VoucherAuditDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"傳票審核傳送檔不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票審核傳送檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<VoucherAuditDto> GetByVoucherIdAsync(string voucherId)
    {
        try
        {
            var entity = await _repository.GetByVoucherIdAsync(voucherId);
            if (entity == null)
            {
                throw new InvalidOperationException($"傳票審核傳送檔不存在: {voucherId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢傳票審核傳送檔失敗: {voucherId}", ex);
            throw;
        }
    }

    public async Task<VoucherAuditDto> CreateAsync(VoucherAuditDto dto)
    {
        try
        {
            var entity = new VoucherAudit
            {
                VoucherId = dto.VoucherId,
                VoucherKind = dto.VoucherKind,
                VoucherDate = dto.VoucherDate,
                AuditStatus = dto.AuditStatus ?? "PENDING",
                AuditUser = dto.AuditUser,
                AuditTime = dto.AuditTime,
                AuditNotes = dto.AuditNotes,
                SendStatus = dto.SendStatus ?? "PENDING",
                SendTime = dto.SendTime,
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
            _logger.LogError("新增傳票審核傳送檔失敗", ex);
            throw;
        }
    }

    public async Task<VoucherAuditDto> UpdateAsync(long tKey, UpdateVoucherAuditDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"傳票審核傳送檔不存在: {tKey}");
            }

            if (!string.IsNullOrEmpty(dto.AuditStatus))
            {
                entity.AuditStatus = dto.AuditStatus;
                if (dto.AuditStatus == "APPROVED" || dto.AuditStatus == "REJECTED")
                {
                    entity.AuditUser = GetCurrentUserId();
                    entity.AuditTime = DateTime.Now;
                }
            }

            if (dto.AuditNotes != null)
            {
                entity.AuditNotes = dto.AuditNotes;
            }

            if (!string.IsNullOrEmpty(dto.SendStatus))
            {
                entity.SendStatus = dto.SendStatus;
                if (dto.SendStatus == "SENT")
                {
                    entity.SendTime = DateTime.Now;
                }
            }

            if (dto.Notes != null)
            {
                entity.Notes = dto.Notes;
            }

            entity.CUser = GetCurrentUserId();
            entity.CTime = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改傳票審核傳送檔失敗: {tKey}", ex);
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
                throw new InvalidOperationException($"傳票審核傳送檔不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除傳票審核傳送檔失敗: {tKey}", ex);
            throw;
        }
    }

    private VoucherAuditDto MapToDto(VoucherAudit entity)
    {
        return new VoucherAuditDto
        {
            TKey = entity.TKey,
            VoucherId = entity.VoucherId,
            VoucherKind = entity.VoucherKind,
            VoucherDate = entity.VoucherDate,
            AuditStatus = entity.AuditStatus,
            AuditUser = entity.AuditUser,
            AuditTime = entity.AuditTime,
            AuditNotes = entity.AuditNotes,
            SendStatus = entity.SendStatus,
            SendTime = entity.SendTime,
            Notes = entity.Notes,
            BUser = entity.BUser,
            BTime = entity.BTime,
            CUser = entity.CUser,
            CTime = entity.CTime
        };
    }
}

