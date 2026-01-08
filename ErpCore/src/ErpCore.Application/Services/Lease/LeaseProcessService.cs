using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃處理服務實作 (SYS8B50-SYS8B90)
/// </summary>
public class LeaseProcessService : BaseService, ILeaseProcessService
{
    private readonly ILeaseProcessRepository _repository;

    public LeaseProcessService(
        ILeaseProcessRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseProcessDto>> GetLeaseProcessesAsync(LeaseProcessQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseProcessQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ProcessId = query.ProcessId,
                LeaseId = query.LeaseId,
                ProcessType = query.ProcessType,
                ProcessStatus = query.ProcessStatus,
                ProcessDateFrom = query.ProcessDateFrom,
                ProcessDateTo = query.ProcessDateTo
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseProcessDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃處理列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseProcessDto> GetLeaseProcessByIdAsync(string processId)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"租賃處理不存在: {processId}");
            }

            return MapToDto(process);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task<List<LeaseProcessDto>> GetLeaseProcessesByLeaseIdAsync(string leaseId)
    {
        try
        {
            var items = await _repository.GetByLeaseIdAsync(leaseId);
            return items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號查詢處理列表失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<string> CreateLeaseProcessAsync(CreateLeaseProcessDto dto)
    {
        try
        {
            // 檢查處理編號是否已存在
            if (await _repository.ExistsAsync(dto.ProcessId))
            {
                throw new InvalidOperationException($"處理編號已存在: {dto.ProcessId}");
            }

            var process = new LeaseProcess
            {
                ProcessId = dto.ProcessId,
                LeaseId = dto.LeaseId,
                ProcessType = dto.ProcessType,
                ProcessDate = dto.ProcessDate,
                ProcessStatus = dto.ProcessStatus,
                ProcessUserId = GetCurrentUserId(),
                ProcessUserName = GetCurrentUserName(),
                ProcessMemo = dto.ProcessMemo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(process);

            _logger.LogInfo($"新增租賃處理成功: {dto.ProcessId}");
            return process.ProcessId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃處理失敗: {dto.ProcessId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseProcessAsync(string processId, UpdateLeaseProcessDto dto)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"租賃處理不存在: {processId}");
            }

            process.LeaseId = dto.LeaseId;
            process.ProcessType = dto.ProcessType;
            process.ProcessDate = dto.ProcessDate;
            process.ProcessStatus = dto.ProcessStatus;
            process.ProcessMemo = dto.ProcessMemo;
            process.UpdatedBy = GetCurrentUserId();
            process.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(process);

            _logger.LogInfo($"修改租賃處理成功: {processId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseProcessAsync(string processId)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"租賃處理不存在: {processId}");
            }

            await _repository.DeleteAsync(processId);

            _logger.LogInfo($"刪除租賃處理成功: {processId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseProcessStatusAsync(string processId, UpdateLeaseProcessStatusDto dto)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"租賃處理不存在: {processId}");
            }

            await _repository.UpdateStatusAsync(processId, dto.ProcessStatus);

            _logger.LogInfo($"更新租賃處理狀態成功: {processId}, Status: {dto.ProcessStatus}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃處理狀態失敗: {processId}", ex);
            throw;
        }
    }

    public async Task ExecuteLeaseProcessAsync(string processId, ExecuteLeaseProcessDto dto)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"租賃處理不存在: {processId}");
            }

            process.ProcessStatus = "C"; // 已完成
            process.ProcessResult = dto.ProcessResult;
            process.ProcessMemo = dto.ProcessMemo ?? process.ProcessMemo;
            process.ProcessUserId = GetCurrentUserId();
            process.ProcessUserName = GetCurrentUserName();
            process.UpdatedBy = GetCurrentUserId();
            process.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(process);

            _logger.LogInfo($"執行租賃處理成功: {processId}, Result: {dto.ProcessResult}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"執行租賃處理失敗: {processId}", ex);
            throw;
        }
    }

    public async Task ApproveLeaseProcessAsync(string processId, ApproveLeaseProcessDto dto)
    {
        try
        {
            var process = await _repository.GetByIdAsync(processId);
            if (process == null)
            {
                throw new InvalidOperationException($"租賃處理不存在: {processId}");
            }

            process.ApprovalStatus = dto.ApprovalStatus;
            process.ApprovalUserId = GetCurrentUserId();
            process.ApprovalDate = DateTime.Now;
            process.UpdatedBy = GetCurrentUserId();
            process.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(process);

            _logger.LogInfo($"審核租賃處理成功: {processId}, Status: {dto.ApprovalStatus}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"審核租賃處理失敗: {processId}", ex);
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
            _logger.LogError($"檢查租賃處理是否存在失敗: {processId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private LeaseProcessDto MapToDto(LeaseProcess process)
    {
        return new LeaseProcessDto
        {
            TKey = process.TKey,
            ProcessId = process.ProcessId,
            LeaseId = process.LeaseId,
            ProcessType = process.ProcessType,
            ProcessTypeName = GetProcessTypeName(process.ProcessType),
            ProcessDate = process.ProcessDate,
            ProcessStatus = process.ProcessStatus,
            ProcessStatusName = GetProcessStatusName(process.ProcessStatus),
            ProcessResult = process.ProcessResult,
            ProcessResultName = GetProcessResultName(process.ProcessResult),
            ProcessUserId = process.ProcessUserId,
            ProcessUserName = process.ProcessUserName,
            ProcessMemo = process.ProcessMemo,
            ApprovalUserId = process.ApprovalUserId,
            ApprovalDate = process.ApprovalDate,
            ApprovalStatus = process.ApprovalStatus,
            ApprovalStatusName = GetApprovalStatusName(process.ApprovalStatus),
            SiteId = process.SiteId,
            OrgId = process.OrgId,
            CreatedBy = process.CreatedBy,
            CreatedAt = process.CreatedAt,
            UpdatedBy = process.UpdatedBy,
            UpdatedAt = process.UpdatedAt
        };
    }

    /// <summary>
    /// 取得處理類型名稱
    /// </summary>
    private string GetProcessTypeName(string processType)
    {
        return processType switch
        {
            "RENEWAL" => "續約",
            "TERMINATION" => "終止",
            "MODIFICATION" => "修改",
            "PAYMENT" => "付款",
            _ => processType
        };
    }

    /// <summary>
    /// 取得處理狀態名稱
    /// </summary>
    private string GetProcessStatusName(string processStatus)
    {
        return processStatus switch
        {
            "P" => "待處理",
            "I" => "處理中",
            "C" => "已完成",
            "X" => "已取消",
            _ => processStatus
        };
    }

    /// <summary>
    /// 取得處理結果名稱
    /// </summary>
    private string? GetProcessResultName(string? processResult)
    {
        if (string.IsNullOrEmpty(processResult))
            return null;

        return processResult switch
        {
            "SUCCESS" => "成功",
            "FAILED" => "失敗",
            "PENDING" => "待處理",
            _ => processResult
        };
    }

    /// <summary>
    /// 取得審核狀態名稱
    /// </summary>
    private string? GetApprovalStatusName(string? approvalStatus)
    {
        if (string.IsNullOrEmpty(approvalStatus))
            return null;

        return approvalStatus switch
        {
            "P" => "待審核",
            "A" => "已審核",
            "R" => "已拒絕",
            _ => approvalStatus
        };
    }
}

