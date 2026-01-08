using ErpCore.Application.DTOs.Contract;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Contract;
using ErpCore.Infrastructure.Repositories.Contract;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Contract;

/// <summary>
/// 合同處理服務實作 (SYSF210-SYSF220)
/// </summary>
public class ContractProcessService : BaseService, IContractProcessService
{
    private readonly IContractProcessRepository _repository;

    public ContractProcessService(
        IContractProcessRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ContractProcessDto>> GetContractProcessesAsync(ContractProcessQueryDto query)
    {
        try
        {
            var repositoryQuery = new ContractProcessQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ProcessId = query.ProcessId,
                ContractId = query.ContractId,
                Version = query.Version,
                ProcessType = query.ProcessType,
                Status = query.Status,
                ProcessDateFrom = query.ProcessDateFrom,
                ProcessDateTo = query.ProcessDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ContractProcessDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢合同處理列表失敗", ex);
            throw;
        }
    }

    public async Task<ContractProcessDto> GetContractProcessByIdAsync(string processId)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"合同處理不存在: {processId}");
            }

            return MapToDto(process);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢合同處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<ContractProcessResultDto> CreateContractProcessAsync(CreateContractProcessDto dto)
    {
        try
        {
            // 檢查處理編號是否已存在
            if (await _repository.ExistsAsync(dto.ProcessId))
            {
                throw new InvalidOperationException($"處理編號已存在: {dto.ProcessId}");
            }

            var process = new ContractProcess
            {
                ProcessId = dto.ProcessId,
                ContractId = dto.ContractId,
                Version = dto.Version,
                ProcessType = dto.ProcessType,
                ProcessDate = dto.ProcessDate,
                ProcessAmount = dto.ProcessAmount,
                Status = dto.Status,
                ProcessUserId = dto.ProcessUserId,
                ProcessMemo = dto.ProcessMemo,
                SiteId = dto.SiteId,
                OrgId = dto.OrgId,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(process);

            _logger.LogInfo($"新增合同處理成功: {dto.ProcessId}");
            return new ContractProcessResultDto
            {
                TKey = process.TKey,
                ProcessId = process.ProcessId
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增合同處理失敗: {dto.ProcessId}", ex);
            throw;
        }
    }

    public async Task UpdateContractProcessAsync(string processId, UpdateContractProcessDto dto)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"合同處理不存在: {processId}");
            }

            // 僅待處理或處理中狀態可修改
            if (process.Status != "P" && process.Status != "I")
            {
                throw new InvalidOperationException($"合同處理狀態為 {process.Status}，無法修改");
            }

            process.ProcessType = dto.ProcessType;
            process.ProcessDate = dto.ProcessDate;
            process.ProcessAmount = dto.ProcessAmount;
            process.ProcessUserId = dto.ProcessUserId;
            process.ProcessMemo = dto.ProcessMemo;
            process.SiteId = dto.SiteId;
            process.OrgId = dto.OrgId;
            process.UpdatedBy = GetCurrentUserId();
            process.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(process);

            _logger.LogInfo($"修改合同處理成功: {processId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改合同處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task DeleteContractProcessAsync(string processId)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"合同處理不存在: {processId}");
            }

            // 僅待處理狀態可刪除
            if (process.Status != "P")
            {
                throw new InvalidOperationException($"合同處理狀態為 {process.Status}，無法刪除");
            }

            await _repository.DeleteAsync(processId);

            _logger.LogInfo($"刪除合同處理成功: {processId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除合同處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task CompleteContractProcessAsync(string processId)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"合同處理不存在: {processId}");
            }

            process.Status = "C"; // 已完成
            process.UpdatedBy = GetCurrentUserId();
            process.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(process);

            _logger.LogInfo($"完成合同處理成功: {processId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"完成合同處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task CancelContractProcessAsync(string processId)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"合同處理不存在: {processId}");
            }

            process.Status = "X"; // 已取消
            process.UpdatedBy = GetCurrentUserId();
            process.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(process);

            _logger.LogInfo($"取消合同處理成功: {processId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"取消合同處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string processId)
    {
        try
        {
            return await _repository.ExistsAsync(processId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查合同處理是否存在失敗: {processId}", ex);
            throw;
        }
    }

    private ContractProcessDto MapToDto(ContractProcess process)
    {
        return new ContractProcessDto
        {
            TKey = process.TKey,
            ProcessId = process.ProcessId,
            ContractId = process.ContractId,
            Version = process.Version,
            ProcessType = process.ProcessType,
            ProcessDate = process.ProcessDate,
            ProcessAmount = process.ProcessAmount,
            Status = process.Status,
            ProcessUserId = process.ProcessUserId,
            ProcessMemo = process.ProcessMemo,
            SiteId = process.SiteId,
            OrgId = process.OrgId,
            CreatedBy = process.CreatedBy,
            CreatedAt = process.CreatedAt,
            UpdatedBy = process.UpdatedBy,
            UpdatedAt = process.UpdatedAt
        };
    }
}

