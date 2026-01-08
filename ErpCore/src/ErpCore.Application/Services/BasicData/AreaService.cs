using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 區域服務實作
/// </summary>
public class AreaService : BaseService, IAreaService
{
    private readonly IAreaRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public AreaService(
        IAreaRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<AreaDto>> GetAreasAsync(AreaQueryDto query)
    {
        try
        {
            var repositoryQuery = new AreaQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                AreaId = query.AreaId,
                AreaName = query.AreaName,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new AreaDto
            {
                AreaId = x.AreaId,
                AreaName = x.AreaName,
                SeqNo = x.SeqNo,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<AreaDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢區域列表失敗", ex);
            throw;
        }
    }

    public async Task<AreaDto> GetAreaByIdAsync(string areaId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(areaId);
            if (entity == null)
            {
                throw new InvalidOperationException($"區域不存在: {areaId}");
            }

            return new AreaDto
            {
                AreaId = entity.AreaId,
                AreaName = entity.AreaName,
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
            _logger.LogError($"查詢區域失敗: {areaId}", ex);
            throw;
        }
    }

    public async Task<string> CreateAreaAsync(CreateAreaDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.AreaId);
            if (exists)
            {
                throw new InvalidOperationException($"區域已存在: {dto.AreaId}");
            }

            var entity = new Area
            {
                AreaId = dto.AreaId,
                AreaName = dto.AreaName,
                SeqNo = dto.SeqNo,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.AreaId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增區域失敗: {dto.AreaId}", ex);
            throw;
        }
    }

    public async Task UpdateAreaAsync(string areaId, UpdateAreaDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(areaId);
            if (entity == null)
            {
                throw new InvalidOperationException($"區域不存在: {areaId}");
            }

            entity.AreaName = dto.AreaName;
            entity.SeqNo = dto.SeqNo;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改區域失敗: {areaId}", ex);
            throw;
        }
    }

    public async Task DeleteAreaAsync(string areaId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(areaId);
            if (entity == null)
            {
                throw new InvalidOperationException($"區域不存在: {areaId}");
            }

            await _repository.DeleteAsync(areaId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除區域失敗: {areaId}", ex);
            throw;
        }
    }

    public async Task DeleteAreasBatchAsync(BatchDeleteAreaDto dto)
    {
        try
        {
            foreach (var areaId in dto.AreaIds)
            {
                await DeleteAreaAsync(areaId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除區域失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string areaId, string status)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(areaId);
            if (entity == null)
            {
                throw new InvalidOperationException($"區域不存在: {areaId}");
            }

            if (status != "A" && status != "I")
            {
                throw new ArgumentException("狀態值必須為 A(啟用) 或 I(停用)");
            }

            entity.Status = status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新區域狀態失敗: {areaId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateAreaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.AreaId))
        {
            throw new ArgumentException("區域代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.AreaName))
        {
            throw new ArgumentException("區域名稱不能為空");
        }

        if (dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A(啟用) 或 I(停用)");
        }
    }
}

