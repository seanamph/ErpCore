using ErpCore.Application.DTOs.MirModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Repositories.MirModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRV000 資料服務實作
/// </summary>
public class MirV000DataService : BaseService, IMirV000DataService
{
    private readonly IMirV000DataRepository _repository;

    public MirV000DataService(
        IMirV000DataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MirV000DataDto>> GetDataListAsync(MirV000DataQueryDto query)
    {
        try
        {
            var repositoryQuery = new MirV000DataQuery
            {
                DataId = query.DataId,
                DataName = query.DataName,
                DataType = query.DataType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new MirV000DataDto
            {
                TKey = x.TKey,
                DataId = x.DataId,
                DataName = x.DataName,
                DataValue = x.DataValue,
                DataType = x.DataType,
                Status = x.Status,
                SortOrder = x.SortOrder,
                Memo = x.Memo,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<MirV000DataDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢MIRV000資料列表失敗", ex);
            throw;
        }
    }

    public async Task<MirV000DataDto> GetDataByIdAsync(string dataId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dataId);
            if (entity == null)
            {
                throw new Exception($"MIRV000資料不存在: {dataId}");
            }

            return new MirV000DataDto
            {
                TKey = entity.TKey,
                DataId = entity.DataId,
                DataName = entity.DataName,
                DataValue = entity.DataValue,
                DataType = entity.DataType,
                Status = entity.Status,
                SortOrder = entity.SortOrder,
                Memo = entity.Memo,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢MIRV000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<string> CreateDataAsync(CreateMirV000DataDto dto)
    {
        try
        {
            var entity = new MirV000Data
            {
                DataId = dto.DataId,
                DataName = dto.DataName,
                DataValue = dto.DataValue,
                DataType = dto.DataType,
                Status = dto.Status,
                SortOrder = dto.SortOrder,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            return entity.DataId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增MIRV000資料失敗: {dto.DataId}", ex);
            throw;
        }
    }

    public async Task UpdateDataAsync(string dataId, UpdateMirV000DataDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dataId);
            if (entity == null)
            {
                throw new Exception($"MIRV000資料不存在: {dataId}");
            }

            entity.DataName = dto.DataName;
            entity.DataValue = dto.DataValue;
            entity.DataType = dto.DataType;
            entity.Status = dto.Status;
            entity.SortOrder = dto.SortOrder;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改MIRV000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task DeleteDataAsync(string dataId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dataId);
            if (entity == null)
            {
                throw new Exception($"MIRV000資料不存在: {dataId}");
            }

            await _repository.DeleteAsync(dataId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除MIRV000資料失敗: {dataId}", ex);
            throw;
        }
    }
}

