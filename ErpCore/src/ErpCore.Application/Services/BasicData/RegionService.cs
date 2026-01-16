using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 地區服務實作
/// </summary>
public class RegionService : BaseService, IRegionService
{
    private readonly IRegionRepository _repository;

    public RegionService(
        IRegionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<RegionDto>> GetRegionsAsync(RegionQueryDto query)
    {
        try
        {
            var repositoryQuery = new RegionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                RegionId = query.RegionId,
                RegionName = query.RegionName
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new RegionDto
            {
                RegionId = x.RegionId,
                RegionName = x.RegionName,
                Memo = x.Memo,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<RegionDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢地區列表失敗", ex);
            throw;
        }
    }

    public async Task<RegionDto> GetRegionAsync(string regionId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(regionId);
            if (entity == null)
            {
                throw new InvalidOperationException($"地區不存在: {regionId}");
            }

            return new RegionDto
            {
                RegionId = entity.RegionId,
                RegionName = entity.RegionName,
                Memo = entity.Memo,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢地區失敗: {regionId}", ex);
            throw;
        }
    }

    public async Task<string> CreateRegionAsync(CreateRegionDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.RegionId);
            if (exists)
            {
                throw new InvalidOperationException($"地區已存在: {dto.RegionId}");
            }

            var entity = new Region
            {
                RegionId = dto.RegionId,
                RegionName = dto.RegionName,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            await _repository.CreateAsync(entity);

            return entity.RegionId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增地區失敗: {dto.RegionId}", ex);
            throw;
        }
    }

    public async Task UpdateRegionAsync(string regionId, UpdateRegionDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(regionId);
            if (entity == null)
            {
                throw new InvalidOperationException($"地區不存在: {regionId}");
            }

            entity.RegionName = dto.RegionName;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改地區失敗: {regionId}", ex);
            throw;
        }
    }

    public async Task DeleteRegionAsync(string regionId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(regionId);
            if (entity == null)
            {
                throw new InvalidOperationException($"地區不存在: {regionId}");
            }

            await _repository.DeleteAsync(regionId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除地區失敗: {regionId}", ex);
            throw;
        }
    }

    public async Task DeleteRegionsBatchAsync(BatchDeleteRegionDto dto)
    {
        try
        {
            foreach (var regionId in dto.RegionIds)
            {
                await DeleteRegionAsync(regionId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除地區失敗", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateRegionDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RegionId))
        {
            throw new ArgumentException("地區編號不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.RegionName))
        {
            throw new ArgumentException("地區名稱不能為空");
        }
    }
}
