using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 費用項目主檔服務實作 (SYSE310-SYSE430)
/// </summary>
public class LeaseFeeItemService : BaseService, ILeaseFeeItemService
{
    private readonly ILeaseFeeItemRepository _repository;

    public LeaseFeeItemService(
        ILeaseFeeItemRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseFeeItemDto>> GetLeaseFeeItemsAsync(LeaseFeeItemQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseFeeItemQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                FeeItemId = query.FeeItemId,
                FeeItemName = query.FeeItemName,
                FeeType = query.FeeType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseFeeItemDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢費用項目列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseFeeItemDto> GetLeaseFeeItemByIdAsync(string feeItemId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeItemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用項目不存在: {feeItemId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢費用項目失敗: {feeItemId}", ex);
            throw;
        }
    }

    public async Task<LeaseFeeItemDto> CreateLeaseFeeItemAsync(CreateLeaseFeeItemDto dto)
    {
        try
        {
            // 檢查費用項目編號是否已存在
            if (await _repository.ExistsAsync(dto.FeeItemId))
            {
                throw new InvalidOperationException($"費用項目編號已存在: {dto.FeeItemId}");
            }

            var entity = new LeaseFeeItem
            {
                FeeItemId = dto.FeeItemId,
                FeeItemName = dto.FeeItemName,
                FeeType = dto.FeeType,
                DefaultAmount = dto.DefaultAmount,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增費用項目成功: {dto.FeeItemId}");

            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增費用項目失敗: {dto.FeeItemId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseFeeItemAsync(string feeItemId, UpdateLeaseFeeItemDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeItemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用項目不存在: {feeItemId}");
            }

            entity.FeeItemName = dto.FeeItemName;
            entity.FeeType = dto.FeeType;
            entity.DefaultAmount = dto.DefaultAmount;
            entity.Status = dto.Status;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改費用項目成功: {feeItemId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改費用項目失敗: {feeItemId}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseFeeItemAsync(string feeItemId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeItemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用項目不存在: {feeItemId}");
            }

            await _repository.DeleteAsync(feeItemId);
            _logger.LogInfo($"刪除費用項目成功: {feeItemId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除費用項目失敗: {feeItemId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseFeeItemStatusAsync(string feeItemId, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(feeItemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"費用項目不存在: {feeItemId}");
            }

            await _repository.UpdateStatusAsync(feeItemId, status);
            _logger.LogInfo($"更新費用項目狀態成功: {feeItemId} -> {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新費用項目狀態失敗: {feeItemId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string feeItemId)
    {
        try
        {
            return await _repository.ExistsAsync(feeItemId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查費用項目是否存在失敗: {feeItemId}", ex);
            throw;
        }
    }

    private LeaseFeeItemDto MapToDto(LeaseFeeItem entity)
    {
        return new LeaseFeeItemDto
        {
            TKey = entity.TKey,
            FeeItemId = entity.FeeItemId,
            FeeItemName = entity.FeeItemName,
            FeeType = entity.FeeType,
            DefaultAmount = entity.DefaultAmount,
            Status = entity.Status,
            StatusName = GetStatusName(entity.Status),
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private string? GetStatusName(string status)
    {
        return status switch
        {
            "A" => "啟用",
            "I" => "停用",
            _ => status
        };
    }
}

