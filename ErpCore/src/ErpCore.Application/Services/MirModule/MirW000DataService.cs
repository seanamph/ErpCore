using ErpCore.Application.DTOs.MirModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.MirModule;
using ErpCore.Infrastructure.Repositories.MirModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.MirModule;

/// <summary>
/// MIRW000 資料服務實作
/// </summary>
public class MirW000DataService : BaseService, IMirW000DataService
{
    private readonly IMirW000DataRepository _repository;

    public MirW000DataService(
        IMirW000DataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<MirW000DataDto>> GetDataListAsync(MirW000DataQueryDto query)
    {
        try
        {
            var repositoryQuery = new MirW000DataQuery
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

            var dtos = items.Select(x => new MirW000DataDto
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

            return new PagedResult<MirW000DataDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢MIRW000資料列表失敗", ex);
            throw;
        }
    }

    public async Task<MirW000DataDto> GetDataByIdAsync(string dataId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dataId);
            if (entity == null)
            {
                throw new Exception($"MIRW000資料不存在: {dataId}");
            }

            return new MirW000DataDto
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
            _logger.LogError($"查詢MIRW000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<string> CreateDataAsync(CreateMirW000DataDto dto)
    {
        try
        {
            var entity = new MirW000Data
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
            _logger.LogError($"新增MIRW000資料失敗: {dto.DataId}", ex);
            throw;
        }
    }

    public async Task UpdateDataAsync(string dataId, UpdateMirW000DataDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dataId);
            if (entity == null)
            {
                throw new Exception($"MIRW000資料不存在: {dataId}");
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
            _logger.LogError($"修改MIRW000資料失敗: {dataId}", ex);
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
                throw new Exception($"MIRW000資料不存在: {dataId}");
            }

            await _repository.DeleteAsync(dataId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除MIRW000資料失敗: {dataId}", ex);
            throw;
        }
    }
}

