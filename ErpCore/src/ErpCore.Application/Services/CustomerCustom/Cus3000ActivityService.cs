using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Repositories.CustomerCustom;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerCustom;

/// <summary>
/// CUS3000 活動服務實作 (SYS3510-SYS3580 - 活動管理)
/// </summary>
public class Cus3000ActivityService : BaseService, ICus3000ActivityService
{
    private readonly ICus3000ActivityRepository _repository;

    public Cus3000ActivityService(
        ICus3000ActivityRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Cus3000ActivityDto>> GetCus3000ActivityListAsync(Cus3000ActivityQueryDto query)
    {
        try
        {
            var repositoryQuery = new Cus3000ActivityQuery
            {
                ActivityId = query.ActivityId,
                ActivityName = query.ActivityName,
                ActivityDateFrom = query.ActivityDateFrom,
                ActivityDateTo = query.ActivityDateTo,
                Status = query.Status,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<Cus3000ActivityDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000活動列表失敗", ex);
            throw;
        }
    }

    public async Task<Cus3000ActivityDto?> GetCus3000ActivityByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000活動失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Cus3000ActivityDto?> GetCus3000ActivityByActivityIdAsync(string activityId)
    {
        try
        {
            var entity = await _repository.GetByActivityIdAsync(activityId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000活動失敗: {activityId}", ex);
            throw;
        }
    }

    public async Task<long> CreateCus3000ActivityAsync(CreateCus3000ActivityDto dto)
    {
        try
        {
            // 檢查活動編號是否已存在
            var existing = await _repository.GetByActivityIdAsync(dto.ActivityId);
            if (existing != null)
            {
                throw new InvalidOperationException($"活動編號已存在: {dto.ActivityId}");
            }

            var entity = new Cus3000Activity
            {
                ActivityId = dto.ActivityId,
                ActivityName = dto.ActivityName,
                ActivityDate = dto.ActivityDate,
                Status = dto.Status,
                CreatedBy = _userContext.GetUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.GetUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增CUS3000活動成功: {dto.ActivityId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增CUS3000活動失敗", ex);
            throw;
        }
    }

    public async Task UpdateCus3000ActivityAsync(long tKey, UpdateCus3000ActivityDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            entity.ActivityName = dto.ActivityName;
            entity.ActivityDate = dto.ActivityDate;
            entity.Status = dto.Status;
            entity.UpdatedBy = _userContext.GetUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改CUS3000活動成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改CUS3000活動失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteCus3000ActivityAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除CUS3000活動成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CUS3000活動失敗: {tKey}", ex);
            throw;
        }
    }

    private Cus3000ActivityDto MapToDto(Cus3000Activity entity)
    {
        return new Cus3000ActivityDto
        {
            TKey = entity.TKey,
            ActivityId = entity.ActivityId,
            ActivityName = entity.ActivityName,
            ActivityDate = entity.ActivityDate,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

