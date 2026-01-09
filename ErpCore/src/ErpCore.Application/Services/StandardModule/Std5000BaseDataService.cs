using ErpCore.Application.DTOs.StandardModule;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.StandardModule;
using ErpCore.Infrastructure.Repositories.StandardModule;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.StandardModule;

/// <summary>
/// STD5000 基礎資料服務實作 (SYS5110-SYS5150 - 基礎資料維護)
/// </summary>
public class Std5000BaseDataService : BaseService, IStd5000BaseDataService
{
    private readonly IStd5000BaseDataRepository _repository;

    public Std5000BaseDataService(
        IStd5000BaseDataRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<Std5000BaseDataDto>> GetStd5000BaseDataListAsync(Std5000BaseDataQueryDto query)
    {
        try
        {
            var repositoryQuery = new Std5000BaseDataQuery
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

            return new PagedResult<Std5000BaseDataDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢STD5000基礎資料列表失敗", ex);
            throw;
        }
    }

    public async Task<Std5000BaseDataDto?> GetStd5000BaseDataByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<Std5000BaseDataDto?> GetStd5000BaseDataByDataIdAndTypeAsync(string dataId, string dataType)
    {
        try
        {
            var entity = await _repository.GetByDataIdAndTypeAsync(dataId, dataType);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢STD5000基礎資料失敗: {dataId}, {dataType}", ex);
            throw;
        }
    }

    public async Task<long> CreateStd5000BaseDataAsync(CreateStd5000BaseDataDto dto)
    {
        try
        {
            // 檢查資料代碼是否已存在
            var existing = await _repository.GetByDataIdAndTypeAsync(dto.DataId, dto.DataType);
            if (existing != null)
            {
                throw new InvalidOperationException($"資料代碼已存在: {dto.DataId}, {dto.DataType}");
            }

            var entity = new Std5000BaseData
            {
                DataId = dto.DataId,
                DataName = dto.DataName,
                DataType = dto.DataType,
                Status = dto.Status,
                Memo = dto.Memo,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增STD5000基礎資料成功: {dto.DataId}, {dto.DataType}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增STD5000基礎資料失敗", ex);
            throw;
        }
    }

    public async Task UpdateStd5000BaseDataAsync(long tKey, UpdateStd5000BaseDataDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            entity.DataName = dto.DataName;
            entity.Status = dto.Status;
            entity.Memo = dto.Memo;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改STD5000基礎資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改STD5000基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteStd5000BaseDataAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除STD5000基礎資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除STD5000基礎資料失敗: {tKey}", ex);
            throw;
        }
    }

    private Std5000BaseDataDto MapToDto(Std5000BaseData entity)
    {
        return new Std5000BaseDataDto
        {
            TKey = entity.TKey,
            DataId = entity.DataId,
            DataName = entity.DataName,
            DataType = entity.DataType,
            Status = entity.Status,
            Memo = entity.Memo,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

