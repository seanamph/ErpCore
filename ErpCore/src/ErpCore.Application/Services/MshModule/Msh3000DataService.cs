using ErpCore.Application.DTOs.MshModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.MshModule;
using ErpCore.Infrastructure.Repositories.MshModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.MshModule;

/// <summary>
/// MSH3000 資料服務實作
/// </summary>
public class Msh3000DataService : BaseService, IMsh3000DataService
{
    private readonly IMsh3000DataRepository _repository;

    public Msh3000DataService(
        IMsh3000DataRepository repository,
        ILoggerService logger) : base(logger)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Msh3000DataDto>> GetDataListAsync(Msh3000DataQueryDto query)
    {
        try
        {
            var repositoryQuery = new Msh3000DataQuery
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

            var dtos = items.Select(x => new Msh3000DataDto
            {
                TKey = x.TKey,
                DataId = x.DataId,
                DataName = x.DataName,
                DataValue = x.DataValue,
                DataType = x.DataType,
                ImagePath = x.ImagePath,
                Status = x.Status,
                SortOrder = x.SortOrder,
                Memo = x.Memo,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<Msh3000DataDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢MSH3000資料列表失敗", ex);
            throw;
        }
    }

    public async Task<Msh3000DataDto> GetDataByIdAsync(string dataId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dataId);
            if (entity == null)
            {
                throw new Exception($"MSH3000資料不存在: {dataId}");
            }

            return new Msh3000DataDto
            {
                TKey = entity.TKey,
                DataId = entity.DataId,
                DataName = entity.DataName,
                DataValue = entity.DataValue,
                DataType = entity.DataType,
                ImagePath = entity.ImagePath,
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
            _logger.LogError($"查詢MSH3000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<string> CreateDataAsync(CreateMsh3000DataDto dto)
    {
        try
        {
            var entity = new Msh3000Data
            {
                DataId = dto.DataId,
                DataName = dto.DataName,
                DataValue = dto.DataValue,
                DataType = dto.DataType,
                ImagePath = dto.ImagePath,
                Status = dto.Status,
                SortOrder = dto.SortOrder,
                Memo = dto.Memo,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);
            return entity.DataId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增MSH3000資料失敗: {dto.DataId}", ex);
            throw;
        }
    }

    public async Task UpdateDataAsync(string dataId, UpdateMsh3000DataDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(dataId);
            if (entity == null)
            {
                throw new Exception($"MSH3000資料不存在: {dataId}");
            }

            entity.DataName = dto.DataName;
            entity.DataValue = dto.DataValue;
            entity.DataType = dto.DataType;
            entity.ImagePath = dto.ImagePath;
            entity.Status = dto.Status;
            entity.SortOrder = dto.SortOrder;
            entity.Memo = dto.Memo;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改MSH3000資料失敗: {dataId}", ex);
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
                throw new Exception($"MSH3000資料不存在: {dataId}");
            }

            await _repository.DeleteAsync(dataId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除MSH3000資料失敗: {dataId}", ex);
            throw;
        }
    }
}

