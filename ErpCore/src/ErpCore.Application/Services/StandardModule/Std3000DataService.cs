using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Repositories.StandardModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD3000 資料服務實作 (SYS3620 - 標準資料維護)
/// </summary>
public class Std3000DataService : BaseService, IStd3000DataService
{
    private readonly IStd3000DataRepository _repository;

    public Std3000DataService(
        IStd3000DataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Std3000DataDto>> GetStd3000DataListAsync(Std3000DataQueryDto query)
    {
        try
        {
            var repositoryQuery = new Std3000DataQuery
            {
                DataId = query.DataId,
                DataName = query.DataName,
                DataType = query.DataType,
                Status = query.Status,
                Keyword = query.Keyword,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<Std3000DataDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD3000資料列表失敗", ex);
            throw;
        }
    }

    public async Task<Std3000DataDto?> GetStd3000DataByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD3000資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Std3000DataDto?> GetStd3000DataByDataIdAsync(string dataId)
    {
        try
        {
            var entity = await _repository.GetByDataIdAsync(dataId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD3000資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<long> CreateStd3000DataAsync(CreateStd3000DataDto dto)
    {
        try
        {
            // 檢查資料代碼是否已存在
            var existing = await _repository.GetByDataIdAsync(dto.DataId);
            if (existing != null)
            {
                throw new InvalidOperationException($"資料代碼已存在: {dto.DataId}");
            }

            var entity = new Std3000Data
            {
                DataId = dto.DataId,
                DataName = dto.DataName,
                DataValue = dto.DataValue,
                DataType = dto.DataType,
                Status = dto.Status,
                SortOrder = dto.SortOrder,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增STD3000資料成功: {dto.DataId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD3000資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateStd3000DataAsync(long tKey, UpdateStd3000DataDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            entity.DataName = dto.DataName;
            entity.DataValue = dto.DataValue;
            entity.DataType = dto.DataType;
            entity.Status = dto.Status;
            entity.SortOrder = dto.SortOrder;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改STD3000資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD3000資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteStd3000DataAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除STD3000資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD3000資料失敗: {tKey}", ex);
            throw;
        }
    }

    private Std3000DataDto MapToDto(Std3000Data entity)
    {
        return new Std3000DataDto
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
}

