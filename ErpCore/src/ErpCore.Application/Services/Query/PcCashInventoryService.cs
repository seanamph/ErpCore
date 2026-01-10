using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 零用金盤點檔服務實作 (SYSQ241, SYSQ242)
/// </summary>
public class PcCashInventoryService : BaseService, IPcCashInventoryService
{
    private readonly IPcCashInventoryRepository _repository;

    public PcCashInventoryService(
        IPcCashInventoryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PcCashInventoryDto>> QueryAsync(PcCashInventoryQueryDto query)
    {
        try
        {
            return await _repository.QueryAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢零用金盤點檔列表失敗", ex);
            throw;
        }
    }

    public async Task<PcCashInventoryDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金盤點檔不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金盤點檔失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcCashInventoryDto> GetByInventoryIdAsync(string inventoryId)
    {
        try
        {
            var entity = await _repository.GetByInventoryIdAsync(inventoryId);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金盤點檔不存在: {inventoryId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢零用金盤點檔失敗: {inventoryId}", ex);
            throw;
        }
    }

    public async Task<PcCashInventoryDto> CreateAsync(CreatePcCashInventoryDto dto)
    {
        try
        {
            var inventoryId = await _repository.GenerateInventoryIdAsync(dto.SiteId);

            var entity = new PcCashInventory
            {
                InventoryId = inventoryId,
                SiteId = dto.SiteId,
                InventoryDate = dto.InventoryDate,
                KeepEmpId = dto.KeepEmpId,
                InventoryAmount = dto.InventoryAmount,
                ActualAmount = dto.ActualAmount,
                InventoryStatus = "DRAFT",
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
            _logger.LogError("新增零用金盤點檔失敗", ex);
            throw;
        }
    }

    public async Task<PcCashInventoryDto> UpdateAsync(long tKey, UpdatePcCashInventoryDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"零用金盤點檔不存在: {tKey}");
            }

            entity.ActualAmount = dto.ActualAmount;
            entity.InventoryStatus = dto.InventoryStatus ?? entity.InventoryStatus;
            entity.Notes = dto.Notes;
            entity.CUser = GetCurrentUserId();
            entity.CTime = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改零用金盤點檔失敗: {tKey}", ex);
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
                throw new InvalidOperationException($"零用金盤點檔不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除零用金盤點檔失敗: {tKey}", ex);
            throw;
        }
    }

    private PcCashInventoryDto MapToDto(PcCashInventory entity)
    {
        return new PcCashInventoryDto
        {
            TKey = entity.TKey,
            InventoryId = entity.InventoryId,
            SiteId = entity.SiteId,
            InventoryDate = entity.InventoryDate,
            KeepEmpId = entity.KeepEmpId,
            InventoryAmount = entity.InventoryAmount,
            ActualAmount = entity.ActualAmount,
            DifferenceAmount = entity.DifferenceAmount,
            InventoryStatus = entity.InventoryStatus,
            Notes = entity.Notes,
            BUser = entity.BUser,
            BTime = entity.BTime,
            CUser = entity.CUser,
            CTime = entity.CTime
        };
    }
}

