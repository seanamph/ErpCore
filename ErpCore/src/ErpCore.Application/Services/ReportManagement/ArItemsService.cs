using ErpCore.Application.DTOs.ReportManagement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.ReportManagement;
using ErpCore.Infrastructure.Repositories.ReportManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.ReportManagement;

/// <summary>
/// 收款項目服務實作 (SYSR110-SYSR120)
/// </summary>
public class ArItemsService : BaseService, IArItemsService
{
    private readonly IArItemsRepository _repository;

    public ArItemsService(
        IArItemsRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ArItemsDto>> GetAllAsync()
    {
        try
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢收款項目列表失敗", ex);
            throw;
        }
    }

    public async Task<ArItemsDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款項目不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款項目失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ArItemsDto> GetBySiteIdAndAritemIdAsync(string siteId, string aritemId)
    {
        try
        {
            var entity = await _repository.GetBySiteIdAndAritemIdAsync(siteId, aritemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款項目不存在: SiteId={siteId}, AritemId={aritemId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款項目失敗: SiteId={siteId}, AritemId={aritemId}", ex);
            throw;
        }
    }

    public async Task<IEnumerable<ArItemsDto>> GetBySiteIdAsync(string siteId)
    {
        try
        {
            var entities = await _repository.GetBySiteIdAsync(siteId);
            return entities.Select(MapToDto);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢收款項目列表失敗: SiteId={siteId}", ex);
            throw;
        }
    }

    public async Task<ArItemsDto> CreateAsync(CreateArItemsDto dto)
    {
        try
        {
            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.SiteId, dto.AritemId);
            if (exists)
            {
                throw new InvalidOperationException($"收款項目已存在: SiteId={dto.SiteId}, AritemId={dto.AritemId}");
            }

            var entity = new ArItems
            {
                SiteId = dto.SiteId,
                AritemId = dto.AritemId,
                AritemName = dto.AritemName,
                StypeId = dto.StypeId,
                Notes = dto.Notes,
                Buser = GetCurrentUserId(),
                Btime = DateTime.Now,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增收款項目失敗", ex);
            throw;
        }
    }

    public async Task<ArItemsDto> UpdateAsync(long tKey, UpdateArItemsDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款項目不存在: {tKey}");
            }

            entity.AritemName = dto.AritemName;
            entity.StypeId = dto.StypeId;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新收款項目失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"收款項目不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除收款項目失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string siteId, string aritemId)
    {
        try
        {
            return await _repository.ExistsAsync(siteId, aritemId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查收款項目是否存在失敗: SiteId={siteId}, AritemId={aritemId}", ex);
            throw;
        }
    }

    private ArItemsDto MapToDto(ArItems entity)
    {
        return new ArItemsDto
        {
            TKey = entity.TKey,
            SiteId = entity.SiteId,
            AritemId = entity.AritemId,
            AritemName = entity.AritemName,
            StypeId = entity.StypeId,
            Notes = entity.Notes,
            Buser = entity.Buser,
            Btime = entity.Btime,
            Cpriority = entity.Cpriority,
            Cgroup = entity.Cgroup,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

