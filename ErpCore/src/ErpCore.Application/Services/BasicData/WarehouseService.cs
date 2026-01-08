using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 庫別服務實作
/// </summary>
public class WarehouseService : BaseService, IWarehouseService
{
    private readonly IWarehouseRepository _repository;

    public WarehouseService(
        IWarehouseRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<WarehouseDto>> GetWarehousesAsync(WarehouseQueryDto query)
    {
        try
        {
            var repositoryQuery = new WarehouseQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                WarehouseId = query.WarehouseId,
                WarehouseName = query.WarehouseName,
                WarehouseType = query.WarehouseType,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new WarehouseDto
            {
                WarehouseId = x.WarehouseId,
                WarehouseName = x.WarehouseName,
                WarehouseType = x.WarehouseType,
                Location = x.Location,
                SeqNo = x.SeqNo,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<WarehouseDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢庫別列表失敗", ex);
            throw;
        }
    }

    public async Task<WarehouseDto> GetWarehouseByIdAsync(string warehouseId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(warehouseId);
            if (entity == null)
            {
                throw new InvalidOperationException($"庫別不存在: {warehouseId}");
            }

            return new WarehouseDto
            {
                WarehouseId = entity.WarehouseId,
                WarehouseName = entity.WarehouseName,
                WarehouseType = entity.WarehouseType,
                Location = entity.Location,
                SeqNo = entity.SeqNo,
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
            _logger.LogError($"查詢庫別失敗: {warehouseId}", ex);
            throw;
        }
    }

    public async Task<string> CreateWarehouseAsync(CreateWarehouseDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.WarehouseId);
            if (exists)
            {
                throw new InvalidOperationException($"庫別已存在: {dto.WarehouseId}");
            }

            var entity = new Warehouse
            {
                WarehouseId = dto.WarehouseId,
                WarehouseName = dto.WarehouseName,
                WarehouseType = dto.WarehouseType,
                Location = dto.Location,
                SeqNo = dto.SeqNo ?? 0,
                Status = dto.Status ?? "A",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            await _repository.CreateAsync(entity);

            return entity.WarehouseId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增庫別失敗: {dto.WarehouseId}", ex);
            throw;
        }
    }

    public async Task UpdateWarehouseAsync(string warehouseId, UpdateWarehouseDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(warehouseId);
            if (entity == null)
            {
                throw new InvalidOperationException($"庫別不存在: {warehouseId}");
            }

            entity.WarehouseName = dto.WarehouseName;
            entity.WarehouseType = dto.WarehouseType;
            entity.Location = dto.Location;
            entity.SeqNo = dto.SeqNo ?? 0;
            entity.Status = dto.Status ?? "A";
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改庫別失敗: {warehouseId}", ex);
            throw;
        }
    }

    public async Task DeleteWarehouseAsync(string warehouseId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(warehouseId);
            if (entity == null)
            {
                throw new InvalidOperationException($"庫別不存在: {warehouseId}");
            }

            await _repository.DeleteAsync(warehouseId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除庫別失敗: {warehouseId}", ex);
            throw;
        }
    }

    public async Task DeleteWarehousesBatchAsync(BatchDeleteWarehouseDto dto)
    {
        try
        {
            foreach (var warehouseId in dto.WarehouseIds)
            {
                await DeleteWarehouseAsync(warehouseId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除庫別失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string warehouseId, UpdateWarehouseStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(warehouseId);
            if (entity == null)
            {
                throw new InvalidOperationException($"庫別不存在: {warehouseId}");
            }

            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新庫別狀態失敗: {warehouseId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateWarehouseDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.WarehouseId))
        {
            throw new ArgumentException("庫別代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.WarehouseName))
        {
            throw new ArgumentException("庫別名稱不能為空");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A (啟用) 或 I (停用)");
        }
    }
}

