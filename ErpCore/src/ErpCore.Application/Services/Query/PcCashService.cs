using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金主檔服務實作 (SYSQ210)
/// </summary>
public class PcCashService : BaseService, IPcCashService
{
    private readonly IPcCashRepository _repository;

    public PcCashService(
        IPcCashRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PcCashDto>> QueryAsync(PcCashQueryDto query)
    {
        try
        {
            return await _repository.QueryAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCashDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCashDto> GetByCashIdAsync(string cashId)
    {
        try
        {
            var entity = await _repository.GetByCashIdAsync(cashId);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金不存在: {cashId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金失敗: {cashId}", ex);
            throw;
        }
    }

    public async Task<PcCashDto> CreateAsync(CreatePcCashDto dto)
    {
        try
        {
            var cashId = await _repository.GenerateCashIdAsync(dto.SiteId);

            var entity = new PcCash
            {
                CashId = cashId,
                SiteId = dto.SiteId,
                AppleDate = dto.AppleDate,
                AppleName = dto.AppleName,
                OrgId = dto.OrgId,
                KeepEmpId = dto.KeepEmpId,
                CashAmount = dto.CashAmount,
                CashStatus = "DRAFT",
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
            _logger.LogError("新增零用金失敗", ex);
            throw;
        }
    }

    public async Task<PcCashDto> UpdateAsync(long tKey, UpdatePcCashDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金不存在: {tKey}");
            }

            entity.AppleDate = dto.AppleDate;
            entity.AppleName = dto.AppleName;
            entity.OrgId = dto.OrgId;
            entity.KeepEmpId = dto.KeepEmpId;
            entity.CashAmount = dto.CashAmount;
            entity.CashStatus = dto.CashStatus ?? entity.CashStatus;
            entity.Notes = dto.Notes;
            entity.CUser = GetCurrentUserId();
            entity.CTime = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金失敗: {tKey}", ex);
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
                throw new InvalidOperationException($"零用金不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<List<PcCashDto>> BatchCreateAsync(BatchCreatePcCashDto dto)
    {
        try
        {
            var results = new List<PcCashDto>();
            foreach (var item in dto.Items)
            {
                var result = await CreateAsync(item);
                results.Add(result);
            }
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError("批量新增零用金失敗", ex);
            throw;
        }
    }

    private PcCashDto MapToDto(PcCash entity)
    {
        return new PcCashDto
        {
            TKey = entity.TKey,
            CashId = entity.CashId,
            SiteId = entity.SiteId,
            AppleDate = entity.AppleDate,
            AppleName = entity.AppleName,
            OrgId = entity.OrgId,
            KeepEmpId = entity.KeepEmpId,
            CashAmount = entity.CashAmount,
            CashStatus = entity.CashStatus,
            Notes = entity.Notes,
            BUser = entity.BUser,
            BTime = entity.BTime,
            CUser = entity.CUser,
            CTime = entity.CTime,
            CPriority = entity.CPriority,
            CGroup = entity.CGroup
        };
    }
}

