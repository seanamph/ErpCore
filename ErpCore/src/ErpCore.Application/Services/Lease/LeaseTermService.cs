using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃條件服務實作 (SYSE110-SYSE140)
/// </summary>
public class LeaseTermService : BaseService, ILeaseTermService
{
    private readonly ILeaseTermRepository _repository;

    public LeaseTermService(
        ILeaseTermRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseTermDto>> GetLeaseTermsAsync(LeaseTermQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseTermQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                LeaseId = query.LeaseId,
                Version = query.Version,
                TermType = query.TermType
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseTermDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃條件列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseTermDto> GetLeaseTermByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃條件不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃條件失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<LeaseTermDto>> GetLeaseTermsByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            var items = await _repository.GetByLeaseIdAndVersionAsync(leaseId, version);
            return items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃條件失敗: {leaseId}/{version}", ex);
            throw;
        }
    }

    public async Task<LeaseTermDto> CreateLeaseTermAsync(CreateLeaseTermDto dto)
    {
        try
        {
            var entity = new LeaseTerm
            {
                LeaseId = dto.LeaseId,
                Version = dto.Version,
                TermType = dto.TermType,
                TermName = dto.TermName,
                TermValue = dto.TermValue,
                TermAmount = dto.TermAmount,
                TermDate = dto.TermDate,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增租賃條件成功: {dto.LeaseId}/{dto.Version}");

            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃條件失敗: {dto.LeaseId}/{dto.Version}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseTermAsync(long tKey, UpdateLeaseTermDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃條件不存在: {tKey}");
            }

            entity.TermType = dto.TermType;
            entity.TermName = dto.TermName;
            entity.TermValue = dto.TermValue;
            entity.TermAmount = dto.TermAmount;
            entity.TermDate = dto.TermDate;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改租賃條件成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃條件失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseTermAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"租賃條件不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除租賃條件成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃條件失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseTermsByLeaseIdAndVersionAsync(string leaseId, string version)
    {
        try
        {
            await _repository.DeleteByLeaseIdAndVersionAsync(leaseId, version);
            _logger.LogInfo($"刪除租賃條件成功: {leaseId}/{version}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃條件失敗: {leaseId}/{version}", ex);
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
            _logger.LogError($"檢查租賃條件是否存在失敗: {tKey}", ex);
            throw;
        }
    }

    private LeaseTermDto MapToDto(LeaseTerm entity)
    {
        return new LeaseTermDto
        {
            TKey = entity.TKey,
            LeaseId = entity.LeaseId,
            Version = entity.Version,
            TermType = entity.TermType,
            TermName = entity.TermName,
            TermValue = entity.TermValue,
            TermAmount = entity.TermAmount,
            TermDate = entity.TermDate,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

