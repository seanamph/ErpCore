using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃會計分類服務實作 (SYSE110-SYSE140)
/// </summary>
public class LeaseAccountingCategoryService : BaseService, ILeaseAccountingCategoryService
{
    private readonly ILeaseAccountingCategoryRepository _repository;

    public LeaseAccountingCategoryService(
        ILeaseAccountingCategoryRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseAccountingCategoryDto>> GetLeaseAccountingCategoriesAsync(LeaseAccountingCategoryQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseAccountingCategoryQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                LeaseId = query.LeaseId,
                Version = query.Version,
                CategoryId = query.CategoryId
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseAccountingCategoryDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃會計分類列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseAccountingCategoryDto> GetLeaseAccountingCategoryByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃會計分類不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃會計分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseAccountingCategoryDto>> GetLeaseAccountingCategoriesByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            var items = await _repository.GetByLeaseIdAndVersionAsync(leaseId, version);
            return items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃會計分類失敗: {leaseId}/{version}", ex);
            throw;
        }
    }

    public async Task<LeaseAccountingCategoryDto> CreateLeaseAccountingCategoryAsync(CreateLeaseAccountingCategoryDto dto)
    {
        try
        {
            var entity = new LeaseAccountingCategory
            {
                LeaseId = dto.LeaseId,
                Version = dto.Version,
                CategoryId = dto.CategoryId,
                CategoryName = dto.CategoryName,
                Amount = dto.Amount,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增租賃會計分類成功: {dto.LeaseId}/{dto.Version}");

            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃會計分類失敗: {dto.LeaseId}/{dto.Version}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseAccountingCategoryAsync(long tKey, UpdateLeaseAccountingCategoryDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃會計分類不存在: {tKey}");
            }

            entity.CategoryId = dto.CategoryId;
            entity.CategoryName = dto.CategoryName;
            entity.Amount = dto.Amount;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改租賃會計分類成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃會計分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseAccountingCategoryAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃會計分類不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除租賃會計分類成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃會計分類失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseAccountingCategoriesByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            await _repository.DeleteByLeaseIdAndVersionAsync(leaseId, version);
            _logger.LogInfo($"刪除租賃會計分類成功: {leaseId}/{version}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃會計分類失敗: {leaseId}/{version}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(long tKey)
    {
        try
        {
            return await _repository.ExistsAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃會計分類是否存在失敗: {tKey}", ex);
            throw;
        }
    }

    private LeaseAccountingCategoryDto MapToDto(LeaseAccountingCategory entity)
    {
        return new LeaseAccountingCategoryDto
        {
            TKey = entity.TKey,
            LeaseId = entity.LeaseId,
            Version = entity.Version,
            CategoryId = entity.CategoryId,
            CategoryName = entity.CategoryName,
            Amount = entity.Amount,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

