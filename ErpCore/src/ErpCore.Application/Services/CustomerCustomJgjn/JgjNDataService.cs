using ErpCore.Application.DTOs.CustomerCustomJgjn;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.CustomerCustomJgjn;
using ErpCore.Infrastructure.Repositories.CustomerCustomJgjn;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.CustomerCustomJgjn;

/// <summary>
/// JGJN資料服務實作
/// </summary>
public class JgjNDataService : BaseService, IJgjNDataService
{
    private readonly IJgjNDataRepository _repository;

    public JgjNDataService(
        IJgjNDataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<JgjNDataDto>> GetJgjNDataListAsync(JgjNDataQueryDto query)
    {
        try
        {
            var repositoryQuery = new JgjNDataQuery
            {
                ModuleCode = query.ModuleCode,
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

            return new PagedResult<JgjNDataDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢JGJN資料列表失敗", ex);
            throw;
        }
    }

    public async Task<JgjNDataDto?> GetJgjNDataByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<JgjNDataDto?> GetJgjNDataByDataIdAndModuleCodeAsync(string dataId, string moduleCode)
    {
        try
        {
            var entity = await _repository.GetByDataIdAndModuleCodeAsync(dataId, moduleCode);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢JGJN資料失敗: {dataId}, {moduleCode}", ex);
            throw;
        }
    }

    public async Task<long> CreateJgjNDataAsync(CreateJgjNDataDto dto)
    {
        try
        {
            // 檢查資料代碼與模組代碼組合是否已存在
            var existing = await _repository.GetByDataIdAndModuleCodeAsync(dto.DataId, dto.ModuleCode);
            if (existing != null)
            {
                throw new InvalidOperationException($"資料代碼與模組代碼組合已存在: {dto.DataId}, {dto.ModuleCode}");
            }

            var entity = new JgjNData
            {
                DataId = dto.DataId,
                ModuleCode = dto.ModuleCode,
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
            _logger.LogInfo($"新增JGJN資料成功: {dto.DataId}, {dto.ModuleCode}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增JGJN資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateJgjNDataAsync(long tKey, UpdateJgjNDataDto dto)
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
            _logger.LogInfo($"修改JGJN資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改JGJN資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteJgjNDataAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除JGJN資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除JGJN資料失敗: {tKey}", ex);
            throw;
        }
    }

    private JgjNDataDto MapToDto(JgjNData entity)
    {
        return new JgjNDataDto
        {
            TKey = entity.TKey,
            DataId = entity.DataId,
            ModuleCode = entity.ModuleCode,
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

