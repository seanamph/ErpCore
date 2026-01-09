using ErpCore.Application.DTOs.MirModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Repositories.MirModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRH000 人事服務實作
/// </summary>
public class MirH000PersonnelService : BaseService, IMirH000PersonnelService
{
    private readonly IMirH000PersonnelRepository _repository;

    public MirH000PersonnelService(
        IMirH000PersonnelRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MirH000PersonnelDto>> GetPersonnelListAsync(MirH000PersonnelQueryDto query)
    {
        try
        {
            var repositoryQuery = new MirH000PersonnelQuery
            {
                PersonnelId = query.PersonnelId,
                PersonnelName = query.PersonnelName,
                DepartmentId = query.DepartmentId,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new MirH000PersonnelDto
            {
                TKey = x.TKey,
                PersonnelId = x.PersonnelId,
                PersonnelName = x.PersonnelName,
                DepartmentId = x.DepartmentId,
                PositionId = x.PositionId,
                HireDate = x.HireDate,
                ResignDate = x.ResignDate,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<MirH000PersonnelDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢人事列表失敗", ex);
            throw;
        }
    }

    public async Task<MirH000PersonnelDto> GetPersonnelByIdAsync(string personnelId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(personnelId);
            if (entity == null)
            {
                throw new Exception($"人事資料不存在: {personnelId}");
            }

            return new MirH000PersonnelDto
            {
                TKey = entity.TKey,
                PersonnelId = entity.PersonnelId,
                PersonnelName = entity.PersonnelName,
                DepartmentId = entity.DepartmentId,
                PositionId = entity.PositionId,
                HireDate = entity.HireDate,
                ResignDate = entity.ResignDate,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢人事資料失敗: {personnelId}", ex);
            throw;
        }
    }

    public async Task<string> CreatePersonnelAsync(CreateMirH000PersonnelDto dto)
    {
        try
        {
            var entity = new MirH000Personnel
            {
                PersonnelId = dto.PersonnelId,
                PersonnelName = dto.PersonnelName,
                DepartmentId = dto.DepartmentId,
                PositionId = dto.PositionId,
                HireDate = dto.HireDate,
                ResignDate = dto.ResignDate,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            return entity.PersonnelId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增人事資料失敗: {dto.PersonnelId}", ex);
            throw;
        }
    }

    public async Task UpdatePersonnelAsync(string personnelId, UpdateMirH000PersonnelDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(personnelId);
            if (entity == null)
            {
                throw new Exception($"人事資料不存在: {personnelId}");
            }

            entity.PersonnelName = dto.PersonnelName;
            entity.DepartmentId = dto.DepartmentId;
            entity.PositionId = dto.PositionId;
            entity.HireDate = dto.HireDate;
            entity.ResignDate = dto.ResignDate;
            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改人事資料失敗: {personnelId}", ex);
            throw;
        }
    }

    public async Task DeletePersonnelAsync(string personnelId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(personnelId);
            if (entity == null)
            {
                throw new Exception($"人事資料不存在: {personnelId}");
            }

            await _repository.DeleteAsync(personnelId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除人事資料失敗: {personnelId}", ex);
            throw;
        }
    }
}

