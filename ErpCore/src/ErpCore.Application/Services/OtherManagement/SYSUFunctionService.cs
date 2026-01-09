using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.OtherManagement;
using ErpCore.Infrastructure.Repositories.OtherManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.OtherManagement;

/// <summary>
/// U系統功能服務實作 (SYSU000)
/// </summary>
public class SYSUFunctionService : BaseService, ISYSUFunctionService
{
    private readonly ISYSUFunctionRepository _repository;

    public SYSUFunctionService(
        ISYSUFunctionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SYSUFunctionDto>> GetSYSUFunctionsAsync(SYSUFunctionQueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSUFunctionQuery
            {
                FunctionId = query.FunctionId,
                FunctionName = query.FunctionName,
                FunctionType = query.FunctionType,
                FunctionCategory = query.FunctionCategory,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new SYSUFunctionDto
            {
                TKey = x.TKey,
                FunctionId = x.FunctionId,
                FunctionName = x.FunctionName,
                FunctionType = x.FunctionType,
                FunctionCategory = x.FunctionCategory,
                FunctionValue = x.FunctionValue,
                FunctionConfig = x.FunctionConfig,
                SeqNo = x.SeqNo,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt,
                CreatedPriority = x.CreatedPriority,
                CreatedGroup = x.CreatedGroup
            }).ToList();

            return new PagedResult<SYSUFunctionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢U系統功能列表失敗", ex);
            throw;
        }
    }

    public async Task<SYSUFunctionDto> GetSYSUFunctionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"U系統功能不存在: {tKey}");
            }

            return new SYSUFunctionDto
            {
                TKey = entity.TKey,
                FunctionId = entity.FunctionId,
                FunctionName = entity.FunctionName,
                FunctionType = entity.FunctionType,
                FunctionCategory = entity.FunctionCategory,
                FunctionValue = entity.FunctionValue,
                FunctionConfig = entity.FunctionConfig,
                SeqNo = entity.SeqNo,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                CreatedPriority = entity.CreatedPriority,
                CreatedGroup = entity.CreatedGroup
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢U系統功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<SYSUFunctionDto> GetSYSUFunctionByFunctionIdAsync(string functionId)
    {
        try
        {
            var entity = await _repository.GetByFunctionIdAsync(functionId);
            if (entity == null)
            {
                throw new Exception($"U系統功能不存在: {functionId}");
            }

            return new SYSUFunctionDto
            {
                TKey = entity.TKey,
                FunctionId = entity.FunctionId,
                FunctionName = entity.FunctionName,
                FunctionType = entity.FunctionType,
                FunctionCategory = entity.FunctionCategory,
                FunctionValue = entity.FunctionValue,
                FunctionConfig = entity.FunctionConfig,
                SeqNo = entity.SeqNo,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt,
                CreatedPriority = entity.CreatedPriority,
                CreatedGroup = entity.CreatedGroup
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢U系統功能失敗: {functionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateSYSUFunctionAsync(CreateSYSUFunctionDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.FunctionId))
            {
                throw new Exception($"U系統功能代碼已存在: {dto.FunctionId}");
            }

            var entity = new SYSUFunction
            {
                FunctionId = dto.FunctionId,
                FunctionName = dto.FunctionName,
                FunctionType = dto.FunctionType,
                FunctionCategory = dto.FunctionCategory,
                FunctionValue = dto.FunctionValue,
                FunctionConfig = dto.FunctionConfig,
                SeqNo = dto.SeqNo ?? 0,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = dto.CreatedPriority,
                CreatedGroup = dto.CreatedGroup
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增U系統功能成功: {dto.FunctionId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增U系統功能失敗: {dto.FunctionId}", ex);
            throw;
        }
    }

    public async Task UpdateSYSUFunctionAsync(long tKey, UpdateSYSUFunctionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"U系統功能不存在: {tKey}");
            }

            entity.FunctionName = dto.FunctionName;
            entity.FunctionType = dto.FunctionType;
            entity.FunctionCategory = dto.FunctionCategory;
            entity.FunctionValue = dto.FunctionValue;
            entity.FunctionConfig = dto.FunctionConfig;
            entity.SeqNo = dto.SeqNo ?? 0;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;
            entity.CreatedPriority = dto.CreatedPriority;
            entity.CreatedGroup = dto.CreatedGroup;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改U系統功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改U系統功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteSYSUFunctionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"U系統功能不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除U系統功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除U系統功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(long tKey, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"U系統功能不存在: {tKey}");
            }

            entity.Status = status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"更新U系統功能狀態成功: {tKey}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新U系統功能狀態失敗: {tKey}", ex);
            throw;
        }
    }
}

