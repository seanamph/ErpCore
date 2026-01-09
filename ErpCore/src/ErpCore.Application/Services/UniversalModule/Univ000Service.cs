using ErpCore.Application.DTOs.UniversalModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.UniversalModule;
using ErpCore.Infrastructure.Repositories.UniversalModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.UniversalModule;

/// <summary>
/// 通用模組服務實作 (UNIV000系列)
/// </summary>
public class Univ000Service : BaseService, IUniv000Service
{
    private readonly IUniv000Repository _repository;

    public Univ000Service(
        IUniv000Repository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Univ000Dto>> GetUniv000ListAsync(Univ000QueryDto query)
    {
        try
        {
            var repositoryQuery = new Univ000Query
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

            return new PagedResult<Univ000Dto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢通用模組資料列表失敗", ex);
            throw;
        }
    }

    public async Task<Univ000Dto?> GetUniv000ByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢通用模組資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Univ000Dto?> GetUniv000ByDataIdAsync(string dataId)
    {
        try
        {
            var entity = await _repository.GetByDataIdAsync(dataId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢通用模組資料失敗: {dataId}", ex);
            throw;
        }
    }

    public async Task<long> CreateUniv000Async(CreateUniv000Dto dto)
    {
        try
        {
            var existing = await _repository.GetByDataIdAsync(dto.DataId);
            if (existing != null)
            {
                throw new InvalidOperationException($"資料代碼已存在: {dto.DataId}");
            }

            var entity = new Univ000
            {
                DataId = dto.DataId,
                DataName = dto.DataName,
                DataType = dto.DataType,
                DataValue = dto.DataValue,
                Status = dto.Status,
                SortOrder = dto.SortOrder,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增通用模組資料成功: {dto.DataId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增通用模組資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateUniv000Async(long tKey, UpdateUniv000Dto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            entity.DataName = dto.DataName;
            entity.DataType = dto.DataType;
            entity.DataValue = dto.DataValue;
            entity.Status = dto.Status;
            entity.SortOrder = dto.SortOrder;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改通用模組資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改通用模組資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteUniv000Async(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除通用模組資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除通用模組資料失敗: {tKey}", ex);
            throw;
        }
    }

    private Univ000Dto MapToDto(Univ000 entity)
    {
        return new Univ000Dto
        {
            TKey = entity.TKey,
            DataId = entity.DataId,
            DataName = entity.DataName,
            DataType = entity.DataType,
            DataValue = entity.DataValue,
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

