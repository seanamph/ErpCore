using ErpCore.Application.DTOs.BusinessReport;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BusinessReport;
using ErpCore.Infrastructure.Repositories.BusinessReport;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BusinessReport;

/// <summary>
/// 業務報表管理服務實作 (SYSL145)
/// </summary>
public class BusinessReportManagementService : BaseService, IBusinessReportManagementService
{
    private readonly IBusinessReportManagementRepository _repository;

    public BusinessReportManagementService(
        IBusinessReportManagementRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<BusinessReportManagementDto>> GetBusinessReportManagementsAsync(BusinessReportManagementQueryDto query)
    {
        try
        {
            var repositoryQuery = new BusinessReportManagementQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SiteId = query.SiteId,
                Type = query.Type,
                Id = query.Id,
                UserId = query.UserId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<BusinessReportManagementDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢業務報表管理列表失敗", ex);
            throw;
        }
    }

    public async Task<BusinessReportManagementDto> GetBusinessReportManagementByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"業務報表管理不存在: {tKey}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢業務報表管理失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateBusinessReportManagementAsync(CreateBusinessReportManagementDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.SiteId))
            {
                throw new ArgumentException("店別代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                throw new ArgumentException("類型不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.Id))
            {
                throw new ArgumentException("ID不能為空");
            }

            // 檢查重複資料
            var existing = await _repository.CheckDuplicateAsync(dto.SiteId, dto.Type, dto.Id);
            if (existing != null)
            {
                throw new InvalidOperationException($"店別+類型+ID已存在，現有使用者: {existing.UserName ?? existing.UserId ?? "未知"}");
            }

            var entity = new BusinessReportManagement
            {
                SiteId = dto.SiteId,
                Type = dto.Type,
                Id = dto.Id,
                UserId = dto.UserId,
                UserName = dto.UserName,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            var result = await _repository.CreateAsync(entity);
            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError("新增業務報表管理失敗", ex);
            throw;
        }
    }

    public async Task UpdateBusinessReportManagementAsync(long tKey, UpdateBusinessReportManagementDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"業務報表管理不存在: {tKey}");
            }

            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.SiteId))
            {
                throw new ArgumentException("店別代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.Type))
            {
                throw new ArgumentException("類型不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.Id))
            {
                throw new ArgumentException("ID不能為空");
            }

            // 檢查重複資料（排除自己）
            var existing = await _repository.CheckDuplicateAsync(dto.SiteId, dto.Type, dto.Id, tKey);
            if (existing != null)
            {
                throw new InvalidOperationException($"店別+類型+ID已存在，現有使用者: {existing.UserName ?? existing.UserId ?? "未知"}");
            }

            entity.SiteId = dto.SiteId;
            entity.Type = dto.Type;
            entity.Id = dto.Id;
            entity.UserId = dto.UserId;
            entity.UserName = dto.UserName;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改業務報表管理失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteBusinessReportManagementAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"業務報表管理不存在: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除業務報表管理失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<int> BatchDeleteBusinessReportManagementAsync(List<long> tKeys)
    {
        try
        {
            return await _repository.BatchDeleteAsync(tKeys);
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除業務報表管理失敗", ex);
            throw;
        }
    }

    public async Task<List<BusinessReportManagementDto>> LoadManagementDataAsync()
    {
        try
        {
            var query = new BusinessReportManagementQuery
            {
                PageIndex = 1,
                PageSize = int.MaxValue,
                Status = "A" // 只載入啟用的資料
            };

            var result = await _repository.QueryAsync(query);
            return result.Items.Select(x => MapToDto(x)).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("載入管理權限資料失敗", ex);
            throw;
        }
    }

    public async Task<CheckDuplicateResultDto> CheckDuplicateAsync(CheckDuplicateDto dto)
    {
        try
        {
            var existing = await _repository.CheckDuplicateAsync(dto.SiteId, dto.Type, dto.Id, dto.ExcludeTKey);

            return new CheckDuplicateResultDto
            {
                IsDuplicate = existing != null,
                ExistingRecord = existing != null ? MapToDto(existing) : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("檢查重複資料失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private BusinessReportManagementDto MapToDto(BusinessReportManagement entity)
    {
        return new BusinessReportManagementDto
        {
            TKey = entity.TKey,
            SiteId = entity.SiteId,
            Type = entity.Type,
            Id = entity.Id,
            UserId = entity.UserId,
            UserName = entity.UserName,
            Status = entity.Status,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

