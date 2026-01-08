using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金參數服務實作 (SYSQ110)
/// </summary>
public class CashParamsService : BaseService, ICashParamsService
{
    private readonly ICashParamsRepository _repository;

    public CashParamsService(
        ICashParamsRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CashParamsDto>> GetAllAsync()
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金參數列表失敗", ex);
            throw;
        }
    }

    public async Task<CashParamsDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金參數不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金參數失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<CashParamsDto> CreateAsync(CreateCashParamsDto dto)
    {
        try
        {
            var entity = new CashParams
            {
                UnitId = dto.UnitId,
                ApexpLid = dto.ApexpLid,
                PtaxLid = dto.PtaxLid,
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
            _logger.LogError("新增零用金參數失敗", ex);
            throw;
        }
    }

    public async Task<CashParamsDto> UpdateAsync(long tKey, UpdateCashParamsDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金參數不存在: {tKey}");
            }

            entity.UnitId = dto.UnitId;
            entity.ApexpLid = dto.ApexpLid;
            entity.PtaxLid = dto.PtaxLid;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金參數失敗: {tKey}", ex);
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
                throw new InvalidOperationException($"零用金參數不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金參數失敗: {tKey}", ex);
            throw;
        }
    }

    private CashParamsDto MapToDto(CashParams entity)
    {
        return new CashParamsDto
        {
            TKey = entity.TKey,
            UnitId = entity.UnitId,
            ApexpLid = entity.ApexpLid,
            PtaxLid = entity.PtaxLid,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

