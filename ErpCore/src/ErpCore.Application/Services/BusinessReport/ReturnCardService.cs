using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 銷退卡服務實作 (SYSL310)
/// </summary>
public class ReturnCardService : BaseService, IReturnCardService
{
    private readonly IReturnCardRepository _repository;

    public ReturnCardService(
        IReturnCardRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ReturnCardDto>> GetReturnCardsAsync(ReturnCardQueryDto query)
    {
        try
        {
            var repositoryQuery = new ReturnCardQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SiteId = query.SiteId,
                OrgId = query.OrgId,
                CardYear = query.CardYear,
                CardMonth = query.CardMonth
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ReturnCardDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢銷退卡列表失敗", ex);
            throw;
        }
    }

    public async Task<ReturnCardDto?> GetReturnCardByUuidAsync(Guid uuid)
    {
        try
        {
            var entity = await _repository.GetByUuidAsync(uuid);
            if (entity == null)
            {
                return null;
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢銷退卡失敗: {uuid}", ex);
            throw;
        }
    }

    public async Task<long> CreateReturnCardAsync(CreateReturnCardDto dto)
    {
        try
        {
            var entity = new ReturnCard
            {
                Uuid = Guid.NewGuid(),
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                CardYear = dto.CardYear,
                CardMonth = dto.CardMonth,
                CardType = dto.CardType,
                Status = "1",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增銷退卡失敗", ex);
            throw;
        }
    }

    public async Task UpdateReturnCardAsync(Guid uuid, UpdateReturnCardDto dto)
    {
        try
        {
            var entity = await _repository.GetByUuidAsync(uuid);
            if (entity == null)
            {
                throw new Exception($"找不到銷退卡: {uuid}");
            }

            if (dto.OrgId != null) entity.OrgId = dto.OrgId;
            if (dto.CardYear.HasValue) entity.CardYear = dto.CardYear.Value;
            if (dto.CardMonth.HasValue) entity.CardMonth = dto.CardMonth.Value;
            if (dto.CardType != null) entity.CardType = dto.CardType;
            if (dto.Status != null) entity.Status = dto.Status;
            if (dto.Notes != null) entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改銷退卡失敗: {uuid}", ex);
            throw;
        }
    }

    public async Task DeleteReturnCardAsync(Guid uuid)
    {
        try
        {
            var entity = await _repository.GetByUuidAsync(uuid);
            if (entity == null)
            {
                throw new Exception($"找不到銷退卡: {uuid}");
            }

            await _repository.DeleteAsync(entity.TKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除銷退卡失敗: {uuid}", ex);
            throw;
        }
    }

    private ReturnCardDto MapToDto(ReturnCard entity)
    {
        return new ReturnCardDto
        {
            TKey = entity.TKey,
            Uuid = entity.Uuid,
            SiteId = entity.SiteId,
            SiteName = entity.SiteName,
            OrgId = entity.OrgId,
            OrgName = entity.OrgName,
            CardYear = entity.CardYear,
            CardMonth = entity.CardMonth,
            CardType = entity.CardType,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

