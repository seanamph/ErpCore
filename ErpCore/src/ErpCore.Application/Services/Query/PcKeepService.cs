using ErpCore.Application.DTOs.Query;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Query;
using ErpCore.Infrastructure.Repositories.Query;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Query;

/// <summary>
/// 保管人及額度設定服務實作 (SYSQ120)
/// </summary>
public class PcKeepService : BaseService, IPcKeepService
{
    private readonly IPcKeepRepository _repository;

    public PcKeepService(
        IPcKeepRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<PcKeepDto>> QueryAsync(PcKeepQueryDto query)
    {
        try
        {
            var repositoryQuery = new PcKeepQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SiteId = query.SiteId,
                KeepEmpId = query.KeepEmpId
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<PcKeepDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢保管人及額度列表失敗", ex);
            throw;
        }
    }

    public async Task<PcKeepDto> GetByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"保管人及額度不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢保管人及額度失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<PcKeepDto> CreateAsync(CreatePcKeepDto dto)
    {
        try
        {
            // 檢查保管人代碼是否已存在（同一分店）
            if (await _repository.ExistsAsync(dto.KeepEmpId, dto.SiteId))
            {
                throw new InvalidOperationException($"保管人代碼已存在: {dto.KeepEmpId}, 分店: {dto.SiteId}");
            }

            var entity = new PcKeep
            {
                SiteId = dto.SiteId,
                KeepEmpId = dto.KeepEmpId,
                PcQuota = dto.PcQuota ?? 0,
                Notes = dto.Notes,
                BUser = GetCurrentUserId(),
                BTime = DateTime.Now,
                CUser = GetCurrentUserId(),
                CTime = DateTime.Now,
                CPriority = null,
                CGroup = GetCurrentOrgId()
            };

            var result = await _repository.CreateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError("新增保管人及額度失敗", ex);
            throw;
        }
    }

    public async Task<PcKeepDto> UpdateAsync(long tKey, UpdatePcKeepDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"保管人及額度不存在: {tKey}");
            }

            entity.PcQuota = dto.PcQuota ?? 0;
            entity.Notes = dto.Notes;
            entity.CUser = GetCurrentUserId();
            entity.CTime = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);
            return MapToDto(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改保管人及額度失敗: {tKey}", ex);
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
                throw new InvalidOperationException($"保管人及額度不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除保管人及額度失敗: {tKey}", ex);
            throw;
        }
    }

    private PcKeepDto MapToDto(PcKeep entity)
    {
        return new PcKeepDto
        {
            TKey = entity.TKey,
            SiteId = entity.SiteId,
            KeepEmpId = entity.KeepEmpId,
            PcQuota = entity.PcQuota,
            Notes = entity.Notes,
            BUser = entity.BUser,
            BTime = entity.BTime,
            CUser = entity.CUser,
            CTime = entity.CTime,
            CPriority = entity.CPriority,
            CGroup = entity.CGroup
        };
    }
}

