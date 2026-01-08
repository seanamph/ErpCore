using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 可管控欄位服務實作 (SYS0510)
/// </summary>
public class ControllableFieldService : BaseService, IControllableFieldService
{
    private readonly IControllableFieldRepository _repository;

    public ControllableFieldService(
        IControllableFieldRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ControllableFieldDto>> GetControllableFieldsAsync(ControllableFieldQueryDto query)
    {
        try
        {
            var repositoryQuery = new ControllableFieldQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                FieldId = query.FieldId,
                FieldName = query.FieldName,
                DbName = query.DbName,
                TableName = query.TableName,
                IsActive = query.IsActive
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new ControllableFieldDto
            {
                FieldId = x.FieldId,
                FieldName = x.FieldName,
                DbName = x.DbName,
                TableName = x.TableName,
                FieldNameInDb = x.FieldNameInDb,
                FieldType = x.FieldType,
                FieldDescription = x.FieldDescription,
                IsRequired = x.IsRequired,
                IsActive = x.IsActive,
                SortOrder = x.SortOrder,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ControllableFieldDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢可管控欄位列表失敗", ex);
            throw;
        }
    }

    public async Task<ControllableFieldDto?> GetControllableFieldByIdAsync(string fieldId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(fieldId);
            if (entity == null)
            {
                return null;
            }

            return new ControllableFieldDto
            {
                FieldId = entity.FieldId,
                FieldName = entity.FieldName,
                DbName = entity.DbName,
                TableName = entity.TableName,
                FieldNameInDb = entity.FieldNameInDb,
                FieldType = entity.FieldType,
                FieldDescription = entity.FieldDescription,
                IsRequired = entity.IsRequired,
                IsActive = entity.IsActive,
                SortOrder = entity.SortOrder,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢可管控欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<List<ControllableFieldDto>> GetControllableFieldsByDbTableAsync(string dbName, string tableName)
    {
        try
        {
            var entities = await _repository.GetByDbTableAsync(dbName, tableName);
            return entities.Select(x => new ControllableFieldDto
            {
                FieldId = x.FieldId,
                FieldName = x.FieldName,
                DbName = x.DbName,
                TableName = x.TableName,
                FieldNameInDb = x.FieldNameInDb,
                FieldType = x.FieldType,
                FieldDescription = x.FieldDescription,
                IsRequired = x.IsRequired,
                IsActive = x.IsActive,
                SortOrder = x.SortOrder,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢可管控欄位失敗: {dbName} - {tableName}", ex);
            throw;
        }
    }

    public async Task<ControllableFieldDto> CreateControllableFieldAsync(CreateControllableFieldDto dto)
    {
        try
        {
            // 檢查欄位代碼是否已存在
            var exists = await _repository.ExistsAsync(dto.FieldId);
            if (exists)
            {
                throw new InvalidOperationException($"欄位代碼已存在: {dto.FieldId}");
            }

            var entity = new ControllableField
            {
                FieldId = dto.FieldId,
                FieldName = dto.FieldName,
                DbName = dto.DbName,
                TableName = dto.TableName,
                FieldNameInDb = dto.FieldNameInDb,
                FieldType = dto.FieldType,
                FieldDescription = dto.FieldDescription,
                IsRequired = dto.IsRequired,
                IsActive = dto.IsActive,
                SortOrder = dto.SortOrder,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);

            return new ControllableFieldDto
            {
                FieldId = result.FieldId,
                FieldName = result.FieldName,
                DbName = result.DbName,
                TableName = result.TableName,
                FieldNameInDb = result.FieldNameInDb,
                FieldType = result.FieldType,
                FieldDescription = result.FieldDescription,
                IsRequired = result.IsRequired,
                IsActive = result.IsActive,
                SortOrder = result.SortOrder,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                UpdatedBy = result.UpdatedBy,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增可管控欄位失敗: {dto.FieldId}", ex);
            throw;
        }
    }

    public async Task<ControllableFieldDto> UpdateControllableFieldAsync(string fieldId, UpdateControllableFieldDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(fieldId);
            if (entity == null)
            {
                throw new InvalidOperationException($"可管控欄位不存在: {fieldId}");
            }

            entity.FieldName = dto.FieldName;
            entity.DbName = dto.DbName;
            entity.TableName = dto.TableName;
            entity.FieldNameInDb = dto.FieldNameInDb;
            entity.FieldType = dto.FieldType;
            entity.FieldDescription = dto.FieldDescription;
            entity.IsRequired = dto.IsRequired;
            entity.IsActive = dto.IsActive;
            entity.SortOrder = dto.SortOrder;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            var result = await _repository.UpdateAsync(entity);

            return new ControllableFieldDto
            {
                FieldId = result.FieldId,
                FieldName = result.FieldName,
                DbName = result.DbName,
                TableName = result.TableName,
                FieldNameInDb = result.FieldNameInDb,
                FieldType = result.FieldType,
                FieldDescription = result.FieldDescription,
                IsRequired = result.IsRequired,
                IsActive = result.IsActive,
                SortOrder = result.SortOrder,
                CreatedBy = result.CreatedBy,
                CreatedAt = result.CreatedAt,
                UpdatedBy = result.UpdatedBy,
                UpdatedAt = result.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改可管控欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task DeleteControllableFieldAsync(string fieldId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(fieldId);
            if (entity == null)
            {
                throw new InvalidOperationException($"可管控欄位不存在: {fieldId}");
            }

            await _repository.DeleteAsync(fieldId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除可管控欄位失敗: {fieldId}", ex);
            throw;
        }
    }

    public async Task<BatchOperationResult> BatchDeleteControllableFieldsAsync(List<string> fieldIds)
    {
        try
        {
            var count = await _repository.BatchDeleteAsync(fieldIds);

            return new BatchOperationResult
            {
                UpdatedCount = count
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("批量刪除可管控欄位失敗", ex);
            throw;
        }
    }
}

