using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 項目對應服務實作 (SYS0360)
/// </summary>
public class ItemCorrespondService : BaseService, IItemCorrespondService
{
    private readonly IItemCorrespondRepository _repository;

    public ItemCorrespondService(
        IItemCorrespondRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ItemCorrespondDto>> GetItemCorrespondsAsync(ItemCorrespondQueryDto query)
    {
        try
        {
            var repositoryQuery = new ItemCorrespondQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ItemId = query.ItemId,
                ItemName = query.ItemName,
                ItemType = query.ItemType,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ItemCorrespondDto
            {
                ItemId = x.ItemId,
                ItemName = x.ItemName,
                ItemType = x.ItemType,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ItemCorrespondDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢項目對應列表失敗", ex);
            throw;
        }
    }

    public async Task<ItemCorrespondDto?> GetItemCorrespondByIdAsync(string itemId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(itemId);
            if (entity == null)
            {
                return null;
            }

            return new ItemCorrespondDto
            {
                ItemId = entity.ItemId,
                ItemName = entity.ItemName,
                ItemType = entity.ItemType,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢項目對應失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task<ItemCorrespondDto> CreateItemCorrespondAsync(CreateItemCorrespondDto dto)
    {
        try
        {
            // 檢查項目代碼是否已存在
            var exists = await _repository.ExistsAsync(dto.ItemId);
            if (exists)
            {
                throw new InvalidOperationException($"項目代碼已存在: {dto.ItemId}");
            }

            var entity = new ItemCorrespond
            {
                ItemId = dto.ItemId,
                ItemName = dto.ItemName,
                ItemType = dto.ItemType,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);

            return new ItemCorrespondDto
            {
                ItemId = result.ItemId,
                ItemName = result.ItemName,
                ItemType = result.ItemType,
                Status = result.Status,
                Notes = result.Notes,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                UpdatedBy = result.UpdatedBy,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增項目對應失敗: {dto.ItemId}", ex);
            throw;
        }
    }

    public async Task<ItemCorrespondDto> UpdateItemCorrespondAsync(string itemId, UpdateItemCorrespondDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(itemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"項目對應不存在: {itemId}");
            }

            entity.ItemName = dto.ItemName;
            entity.ItemType = dto.ItemType;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);

            return new ItemCorrespondDto
            {
                ItemId = result.ItemId,
                ItemName = result.ItemName,
                ItemType = result.ItemType,
                Status = result.Status,
                Notes = result.Notes,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                UpdatedBy = result.UpdatedBy,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改項目對應失敗: {itemId}", ex);
            throw;
        }
    }

    public async Task DeleteItemCorrespondAsync(string itemId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(itemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"項目對應不存在: {itemId}");
            }

            await _repository.DeleteAsync(itemId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除項目對應失敗: {itemId}", ex);
            throw;
        }
    }
}

