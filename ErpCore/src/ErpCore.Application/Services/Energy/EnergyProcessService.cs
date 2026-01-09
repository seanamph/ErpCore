using ErpCore.Application.DTOs.Energy;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Energy;
using ErpCore.Infrastructure.Repositories.Energy;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Energy;

/// <summary>
/// 能源處理服務實作 (SYSO310 - 能源處理功能)
/// </summary>
public class EnergyProcessService : BaseService, IEnergyProcessService
{
    private readonly IEnergyProcessRepository _repository;

    public EnergyProcessService(
        IEnergyProcessRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<EnergyProcessDto>> GetEnergyProcessesAsync(EnergyProcessQueryDto query)
    {
        try
        {
            var repositoryQuery = new EnergyProcessQuery
            {
                ProcessId = query.ProcessId,
                EnergyId = query.EnergyId,
                ProcessDateFrom = query.ProcessDateFrom,
                ProcessDateTo = query.ProcessDateTo,
                ProcessType = query.ProcessType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToDto).ToList();

            return new PagedResult<EnergyProcessDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢能源處理資料列表失敗", ex);
            throw;
        }
    }

    public async Task<EnergyProcessDto?> GetEnergyProcessByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源處理資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<EnergyProcessDto?> GetEnergyProcessByProcessIdAsync(string processId)
    {
        try
        {
            var entity = await _repository.GetByProcessIdAsync(processId);
            return entity != null ? MapToDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢能源處理資料失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<long> CreateEnergyProcessAsync(CreateEnergyProcessDto dto)
    {
        try
        {
            var entity = new EnergyProcess
            {
                ProcessId = dto.ProcessId,
                EnergyId = dto.EnergyId,
                ProcessDate = dto.ProcessDate,
                ProcessType = dto.ProcessType,
                Amount = dto.Amount,
                Cost = dto.Cost,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var tKey = await _repository.CreateAsync(entity);
            _logger.LogInfo($"新增能源處理資料成功: {dto.ProcessId}");
            return tKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增能源處理資料失敗: {dto.ProcessId}", ex);
            throw;
        }
    }

    public async Task UpdateEnergyProcessAsync(long tKey, UpdateEnergyProcessDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"能源處理資料不存在: {tKey}");
            }

            entity.EnergyId = dto.EnergyId;
            entity.ProcessDate = dto.ProcessDate;
            entity.ProcessType = dto.ProcessType;
            entity.Amount = dto.Amount;
            entity.Cost = dto.Cost;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
            _logger.LogInfo($"修改能源處理資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改能源處理資料失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteEnergyProcessAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new Exception($"能源處理資料不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
            _logger.LogInfo($"刪除能源處理資料成功: {tKey}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除能源處理資料失敗: {tKey}", ex);
            throw;
        }
    }

    private EnergyProcessDto MapToDto(EnergyProcess entity)
    {
        return new EnergyProcessDto
        {
            TKey = entity.TKey,
            ProcessId = entity.ProcessId,
            EnergyId = entity.EnergyId,
            ProcessDate = entity.ProcessDate,
            ProcessType = entity.ProcessType,
            Amount = entity.Amount,
            Cost = entity.Cost,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

