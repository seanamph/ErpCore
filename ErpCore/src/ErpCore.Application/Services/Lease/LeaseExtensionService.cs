using ErpCore.Application.DTOs.Lease;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Lease;
using ErpCore.Infrastructure.Repositories.Lease;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Lease;

/// <summary>
/// 租賃擴展服務實作 (SYS8A10-SYS8A45)
/// </summary>
public class LeaseExtensionService : BaseService, ILeaseExtensionService
{
    private readonly ILeaseExtensionRepository _repository;

    public LeaseExtensionService(
        ILeaseExtensionRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<LeaseExtensionDto>> GetLeaseExtensionsAsync(LeaseExtensionQueryDto query)
    {
        try
        {
            var repositoryQuery = new LeaseExtensionQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                ExtensionId = query.ExtensionId,
                LeaseId = query.LeaseId,
                ExtensionType = query.ExtensionType,
                Status = query.Status
            };

            var items = await _repository.QueryAsync(repositoryQuery);
            var totalCount = await _repository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<LeaseExtensionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢租賃擴展列表失敗", ex);
            throw;
        }
    }

    public async Task<LeaseExtensionDto> GetLeaseExtensionByIdAsync(string extensionId)
    {
        try
        {
            var extension = await _repository.GetByIdAsync(extensionId);
            if (extension == null)
            {
                throw new InvalidOperationException($"租賃擴展不存在: {extensionId}");
            }

            return MapToDto(extension);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢租賃擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<List<LeaseExtensionDto>> GetLeaseExtensionsByLeaseIdAsync(string leaseId)
    {
        try
        {
            var items = await _repository.GetByLeaseIdAsync(leaseId);
            return items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"根據租賃編號查詢擴展列表失敗: {leaseId}", ex);
            throw;
        }
    }

    public async Task<string> CreateLeaseExtensionAsync(CreateLeaseExtensionDto dto)
    {
        try
        {
            // 檢查擴展編號是否已存在
            if (await _repository.ExistsAsync(dto.ExtensionId))
            {
                throw new InvalidOperationException($"擴展編號已存在: {dto.ExtensionId}");
            }

            // 驗證日期範圍
            if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
            {
                throw new InvalidOperationException("開始日期不能晚於結束日期");
            }

            var extension = new LeaseExtension
            {
                ExtensionId = dto.ExtensionId,
                LeaseId = dto.LeaseId,
                ExtensionType = dto.ExtensionType,
                ExtensionName = dto.ExtensionName,
                ExtensionValue = dto.ExtensionValue,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Status = dto.Status,
                SeqNo = dto.SeqNo ?? 0,
                Memo = dto.Memo,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(extension);

            _logger.LogInfo($"新增租賃擴展成功: {dto.ExtensionId}");
            return extension.ExtensionId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增租賃擴展失敗: {dto.ExtensionId}", ex);
            throw;
        }
    }

    public async Task UpdateLeaseExtensionAsync(string extensionId, UpdateLeaseExtensionDto dto)
    {
        try
        {
            var extension = await _repository.GetByIdAsync(extensionId);
            if (extension == null)
            {
                throw new InvalidOperationException($"租賃擴展不存在: {extensionId}");
            }

            // 驗證日期範圍
            if (dto.EndDate.HasValue && dto.StartDate.HasValue && dto.StartDate.Value > dto.EndDate.Value)
            {
                throw new InvalidOperationException("開始日期不能晚於結束日期");
            }

            extension.LeaseId = dto.LeaseId;
            extension.ExtensionType = dto.ExtensionType;
            extension.ExtensionName = dto.ExtensionName;
            extension.ExtensionValue = dto.ExtensionValue;
            extension.StartDate = dto.StartDate;
            extension.EndDate = dto.EndDate;
            extension.Status = dto.Status;
            extension.SeqNo = dto.SeqNo ?? 0;
            extension.Memo = dto.Memo;
            extension.UpdatedBy = GetCurrentUserId();
            extension.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(extension);

            _logger.LogInfo($"修改租賃擴展成功: {extensionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改租賃擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task DeleteLeaseExtensionAsync(string extensionId)
    {
        try
        {
            var extension = await _repository.GetByIdAsync(extensionId);
            if (extension == null)
            {
                throw new InvalidOperationException($"租賃擴展不存在: {extensionId}");
            }

            await _repository.DeleteAsync(extensionId);

            _logger.LogInfo($"刪除租賃擴展成功: {extensionId}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除租賃擴展失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteLeaseExtensionsAsync(BatchDeleteLeaseExtensionDto dto)
    {
        try
        {
            foreach (var extensionId in dto.ExtensionIds)
            {
                if (await _repository.ExistsAsync(extensionId))
                {
                    await _repository.DeleteAsync(extensionId);
                }
            }

            _logger.LogInfo($"批次刪除租賃擴展成功: {dto.ExtensionIds.Count} 筆");
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除租賃擴展失敗", ex);
            throw;
        }
    }

    public async Task UpdateLeaseExtensionStatusAsync(string extensionId, UpdateLeaseExtensionStatusDto dto)
    {
        try
        {
            var extension = await _repository.GetByIdAsync(extensionId);
            if (extension == null)
            {
                throw new InvalidOperationException($"租賃擴展不存在: {extensionId}");
            }

            await _repository.UpdateStatusAsync(extensionId, dto.Status);

            _logger.LogInfo($"更新租賃擴展狀態成功: {extensionId}, Status: {dto.Status}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新租賃擴展狀態失敗: {extensionId}", ex);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(string extensionId)
    {
        try
        {
            return await _repository.ExistsAsync(extensionId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"檢查租賃擴展是否存在失敗: {extensionId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private LeaseExtensionDto MapToDto(LeaseExtension extension)
    {
        return new LeaseExtensionDto
        {
            TKey = extension.TKey,
            ExtensionId = extension.ExtensionId,
            LeaseId = extension.LeaseId,
            ExtensionType = extension.ExtensionType,
            ExtensionTypeName = GetExtensionTypeName(extension.ExtensionType),
            ExtensionName = extension.ExtensionName,
            ExtensionValue = extension.ExtensionValue,
            StartDate = extension.StartDate,
            EndDate = extension.EndDate,
            Status = extension.Status,
            StatusName = GetStatusName(extension.Status),
            SeqNo = extension.SeqNo,
            Memo = extension.Memo,
            SiteId = extension.SiteId,
            OrgId = extension.OrgId,
            CreatedBy = extension.CreatedBy,
            CreatedAt = extension.CreatedAt,
            UpdatedBy = extension.UpdatedBy,
            UpdatedAt = extension.UpdatedAt
        };
    }

    /// <summary>
    /// 取得擴展類型名稱
    /// </summary>
    private string GetExtensionTypeName(string extensionType)
    {
        return extensionType switch
        {
            "CONDITION" => "特殊條件",
            "TERM" => "附加條款",
            "SETTING" => "擴展設定",
            _ => extensionType
        };
    }

    /// <summary>
    /// 取得狀態名稱
    /// </summary>
    private string GetStatusName(string status)
    {
        return status switch
        {
            "A" => "啟用",
            "I" => "停用",
            _ => status
        };
    }
}

