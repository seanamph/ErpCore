using ErpCore.Application.DTOs.Procurement;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Procurement;
using ErpCore.Infrastructure.Repositories.Procurement;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Procurement;

/// <summary>
/// 採購其他功能服務實作 (SYSP510-SYSP530)
/// </summary>
public class ProcurementOtherService : BaseService, IProcurementOtherService
{
    private readonly IProcurementOtherRepository _repository;

    public ProcurementOtherService(
        IProcurementOtherRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProcurementOtherDto>> GetProcurementOthersAsync(ProcurementOtherQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProcurementOtherQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                FunctionId = query.FunctionId,
                FunctionName = query.FunctionName,
                FunctionType = query.FunctionType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ProcurementOtherDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢採購其他功能列表失敗", ex);
            throw;
        }
    }

    public async Task<ProcurementOtherDto> GetProcurementOtherByTKeyAsync(long tKey)
    {
        try
        {
            var procurementOther = await _repository.GetByIdAsync(tKey);
            if (procurementOther == null)
            {
                throw new InvalidOperationException($"採購其他功能不存在: {tKey}");
            }

            return MapToDto(procurementOther);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購其他功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<ProcurementOtherDto> GetProcurementOtherByFunctionIdAsync(string functionId)
    {
        try
        {
            var procurementOther = await _repository.GetByFunctionIdAsync(functionId);
            if (procurementOther == null)
            {
                throw new InvalidOperationException($"採購其他功能不存在: {functionId}");
            }

            return MapToDto(procurementOther);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢採購其他功能失敗: {functionId}", ex);
            throw;
        }
    }

    public async Task<long> CreateProcurementOtherAsync(CreateProcurementOtherDto dto)
    {
        try
        {
            // 檢查功能代碼是否已存在
            if (await _repository.ExistsAsync(dto.FunctionId))
            {
                throw new InvalidOperationException($"功能代碼已存在: {dto.FunctionId}");
            }

            var procurementOther = new ProcurementOther
            {
                FunctionId = dto.FunctionId,
                FunctionName = dto.FunctionName,
                FunctionType = dto.FunctionType,
                FunctionDesc = dto.FunctionDesc,
                FunctionConfig = dto.FunctionConfig,
                Status = dto.Status,
                SeqNo = dto.SeqNo,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(procurementOther);

            _logger.LogInfo($"新增採購其他功能成功: {dto.FunctionId}");
            return procurementOther.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增採購其他功能失敗: {dto.FunctionId}", ex);
            throw;
        }
    }

    public async Task UpdateProcurementOtherAsync(long tKey, UpdateProcurementOtherDto dto)
    {
        try
        {
            var procurementOther = await _repository.GetByIdAsync(tKey);
            if (procurementOther == null)
            {
                throw new InvalidOperationException($"採購其他功能不存在: {tKey}");
            }

            procurementOther.FunctionName = dto.FunctionName;
            procurementOther.FunctionType = dto.FunctionType;
            procurementOther.FunctionDesc = dto.FunctionDesc;
            procurementOther.FunctionConfig = dto.FunctionConfig;
            procurementOther.Status = dto.Status;
            procurementOther.SeqNo = dto.SeqNo;
            procurementOther.Memo = dto.Memo;
            procurementOther.UpdatedBy = GetCurrentUserId();
            procurementOther.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(procurementOther);

            _logger.LogInfo($"修改採購其他功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改採購其他功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteProcurementOtherAsync(long tKey)
    {
        try
        {
            var procurementOther = await _repository.GetByIdAsync(tKey);
            if (procurementOther == null)
            {
                throw new InvalidOperationException($"採購其他功能不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);

            _logger.LogInfo($"刪除採購其他功能成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除採購其他功能失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string functionId)
    {
        try
        {
            return await _repository.ExistsAsync(functionId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查採購其他功能是否存在失敗: {functionId}", ex);
            throw;
        }
    }

    private ProcurementOtherDto MapToDto(ProcurementOther procurementOther)
    {
        return new ProcurementOtherDto
        {
            TKey = procurementOther.TKey,
            FunctionId = procurementOther.FunctionId,
            FunctionName = procurementOther.FunctionName,
            FunctionType = procurementOther.FunctionType,
            FunctionDesc = procurementOther.FunctionDesc,
            FunctionConfig = procurementOther.FunctionConfig,
            Status = procurementOther.Status,
            SeqNo = procurementOther.SeqNo,
            Memo = procurementOther.Memo,
            CreatedBy = procurementOther.CreatedBy,
            CreatedAt = procurementOther.CreatedAt,
            UpdatedBy = procurementOther.UpdatedBy,
            UpdatedAt = procurementOther.UpdatedAt
        };
    }
}

