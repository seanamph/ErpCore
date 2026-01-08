using ErpCore.Application.DTOs.OtherManagement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.OtherManagement;
using ErpCore.Infrastructure.Repositories.OtherManagement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.OtherManagement;

/// <summary>
/// S系統功能服務實作 (SYSS000)
/// </summary>
public class SYSSFunctionService : BaseService, ISYSSFunctionService
{
    private readonly ISYSSFunctionRepository _repository;

    public SYSSFunctionService(
        ISYSSFunctionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<SYSSFunctionDto>> GetSYSSFunctionsAsync(SYSSFunctionQueryDto query)
    {
        try
        {
            var repositoryQuery = new SYSSFunctionQuery
            {
                FunctionId = query.FunctionId,
                FunctionName = query.FunctionName,
                FunctionType = query.FunctionType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => new SYSSFunctionDto
            {
                TKey = x.TKey,
                FunctionId = x.FunctionId,
                FunctionName = x.FunctionName,
                FunctionType = x.FunctionType,
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

            return new PagedResult<SYSSFunctionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢S系統功能列表失敗", ex);
            throw;
        }
    }

    public async Task<SYSSFunctionDto> GetSYSSFunctionByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"S系統功能不存在: {tKey}");
            }

            return new SYSSFunctionDto
            {
                TKey = entity.TKey,
                FunctionId = entity.FunctionId,
                FunctionName = entity.FunctionName,
                FunctionType = entity.FunctionType,
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
            _logger.LogError($"查詢S系統功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<SYSSFunctionDto> GetSYSSFunctionByFunctionIdAsync(string functionId)
    {
        try
        {
            var entity = await _repository.GetByFunctionIdAsync(functionId);
            if (entity == null)
            {
                throw new Exception($"S系統功能不存在: {functionId}");
            }

            return new SYSSFunctionDto
            {
                TKey = entity.TKey,
                FunctionId = entity.FunctionId,
                FunctionName = entity.FunctionName,
                FunctionType = entity.FunctionType,
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
            _logger.LogError($"查詢S系統功能失敗: {functionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateSYSSFunctionAsync(CreateSYSSFunctionDto dto)
    {
        try
        {
            if (await _repository.ExistsAsync(dto.FunctionId))
            {
                throw new Exception($"S系統功能代碼已存在: {dto.FunctionId}");
            }

            var entity = new SYSSFunction
            {
                FunctionId = dto.FunctionId,
                FunctionName = dto.FunctionName,
                FunctionType = dto.FunctionType,
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
            _logger.LogInfo($"新增S系統功能成功: {dto.FunctionId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增S系統功能失敗: {dto.FunctionId}", ex);
            throw;
        }
    }

    public async Task UpdateSYSSFunctionAsync(long tKey, UpdateSYSSFunctionDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"S系統功能不存在: {tKey}");
            }

            entity.FunctionName = dto.FunctionName;
            entity.FunctionType = dto.FunctionType;
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
            _logger.LogInfo($"修改S系統功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改S系統功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteSYSSFunctionAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"S系統功能不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除S系統功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除S系統功能失敗: {tKey}", ex);
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
                throw new Exception($"S系統功能不存在: {tKey}");
            }

            entity.Status = status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"更新S系統功能狀態成功: {tKey}, 狀態: {status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新S系統功能狀態失敗: {tKey}", ex);
            throw;
        }
    }
}

