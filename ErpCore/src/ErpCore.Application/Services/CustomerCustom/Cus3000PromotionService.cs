using ErpCore.Application.DTOs.CustomerCustom;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerCustom;
using ErpCore.Infrastructure.Repositories.CustomerCustom;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerCustom;

/// <summary>
/// CUS3000 促銷活動服務實作 (SYS3310-SYS3399 - 促銷活動管理)
/// </summary>
public class Cus3000PromotionService : BaseService, ICus3000PromotionService
{
    private readonly ICus3000PromotionRepository _repository;

    public Cus3000PromotionService(
        ICus3000PromotionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Cus3000PromotionDto>> GetCus3000PromotionListAsync(Cus3000PromotionQueryDto query)
    {
        try
        {
            var repositoryQuery = new Cus3000PromotionQuery
            {
                PromotionId = query.PromotionId,
                PromotionName = query.PromotionName,
                StartDateFrom = query.StartDateFrom,
                StartDateTo = query.StartDateTo,
                EndDateFrom = query.EndDateFrom,
                EndDateTo = query.EndDateTo,
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

            return new PagedResult<Cus3000PromotionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢CUS3000促銷活動列表失敗", ex);
            throw;
        }
    }

    public async Task<Cus3000PromotionDto?> GetCus3000PromotionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000促銷活動失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Cus3000PromotionDto?> GetCus3000PromotionByPromotionIdAsync(string promotionId)
    {
        try
        {
            var entity = await _repository.GetByPromotionIdAsync(promotionId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢CUS3000促銷活動失敗: {promotionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateCus3000PromotionAsync(CreateCus3000PromotionDto dto)
    {
        try
        {
            // 檢查促銷活動編號是否已存在
            var existing = await _repository.GetByPromotionIdAsync(dto.PromotionId);
            if (existing != null)
            {
                throw new InvalidOperationException($"促銷活動編號已存在: {dto.PromotionId}");
            }

            // 驗證日期
            if (dto.EndDate < dto.StartDate)
            {
                throw new InvalidOperationException("結束日期不能早於開始日期");
            }

            var entity = new Cus3000Promotion
            {
                PromotionId = dto.PromotionId,
                PromotionName = dto.PromotionName,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                CreatedBy = _userContext.GetUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.GetUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增CUS3000促銷活動成功: {dto.PromotionId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增CUS3000促銷活動失敗", ex);
            throw;
        }
    }

    public async Task UpdateCus3000PromotionAsync(long tKey, UpdateCus3000PromotionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            // 驗證日期
            if (dto.EndDate < dto.StartDate)
            {
                throw new InvalidOperationException("結束日期不能早於開始日期");
            }

            entity.PromotionName = dto.PromotionName;
            entity.StartDate = dto.StartDate;
            entity.EndDate = dto.EndDate;
            entity.Status = dto.Status;
            entity.UpdatedBy = _userContext.GetUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改CUS3000促銷活動成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改CUS3000促銷活動失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteCus3000PromotionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除CUS3000促銷活動成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除CUS3000促銷活動失敗: {tKey}", ex);
            throw;
        }
    }

    private Cus3000PromotionDto MapToDto(Cus3000Promotion entity)
    {
        return new Cus3000PromotionDto
        {
            TKey = entity.TKey,
            PromotionId = entity.PromotionId,
            PromotionName = entity.PromotionName,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

