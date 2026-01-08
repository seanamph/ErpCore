using ErpCore.Application.DTOs.Recruitment;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Recruitment;
using ErpCore.Infrastructure.Repositories.Recruitment;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Recruitment;

/// <summary>
/// 潛客主檔服務實作 (SYSC165)
/// </summary>
public class ProspectMasterService : BaseService, IProspectMasterService
{
    private readonly IProspectMasterRepository _repository;

    public ProspectMasterService(
        IProspectMasterRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
    }

    public async Task<PagedResult<ProspectMasterDto>> GetProspectMastersAsync(ProspectMasterQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProspectMasterQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                MasterId = query.MasterId,
                MasterName = query.MasterName,
                MasterType = query.MasterType,
                Category = query.Category,
                Industry = query.Industry,
                BusinessType = query.BusinessType,
                Status = query.Status,
                Source = query.Source
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => MapToDto(x)).ToList();

            return new PagedResult<ProspectMasterDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢潛客主檔列表失敗", ex);
            throw;
        }
    }

    public async Task<ProspectMasterDto> GetProspectMasterByIdAsync(string masterId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(masterId);
            if (entity == null)
            {
                throw new InvalidOperationException($"潛客主檔不存在: {masterId}");
            }

            return MapToDto(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢潛客主檔失敗: {masterId}", ex);
            throw;
        }
    }

    public async Task<string> CreateProspectMasterAsync(CreateProspectMasterDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.MasterId))
            {
                throw new ArgumentException("主檔代碼不能為空");
            }

            if (string.IsNullOrWhiteSpace(dto.MasterName))
            {
                throw new ArgumentException("主檔名稱不能為空");
            }

            // 檢查主檔代碼是否已存在
            var exists = await _repository.ExistsAsync(dto.MasterId);
            if (exists)
            {
                throw new InvalidOperationException($"主檔代碼已存在: {dto.MasterId}");
            }

            // 驗證狀態值
            if (!string.IsNullOrEmpty(dto.Status) && 
                dto.Status != "ACTIVE" && dto.Status != "INACTIVE" && dto.Status != "ARCHIVED")
            {
                throw new ArgumentException("狀態值必須為 ACTIVE、INACTIVE 或 ARCHIVED");
            }

            var entity = new ProspectMaster
            {
                MasterId = dto.MasterId,
                MasterName = dto.MasterName,
                MasterType = dto.MasterType,
                Category = dto.Category,
                Industry = dto.Industry,
                BusinessType = dto.BusinessType,
                Status = dto.Status ?? "ACTIVE",
                Priority = dto.Priority,
                Source = dto.Source,
                ContactPerson = dto.ContactPerson,
                ContactTel = dto.ContactTel,
                ContactEmail = dto.ContactEmail,
                ContactAddress = dto.ContactAddress,
                Website = dto.Website,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = null
            };

            var result = await _repository.CreateAsync(entity);
            return result.MasterId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增潛客主檔失敗: {dto.MasterId}", ex);
            throw;
        }
    }

    public async Task UpdateProspectMasterAsync(string masterId, UpdateProspectMasterDto dto)
    {
        try
        {
            // 驗證必填欄位
            if (string.IsNullOrWhiteSpace(dto.MasterName))
            {
                throw new ArgumentException("主檔名稱不能為空");
            }

            // 檢查主檔是否存在
            var existing = await _repository.GetByIdAsync(masterId);
            if (existing == null)
            {
                throw new InvalidOperationException($"潛客主檔不存在: {masterId}");
            }

            // 驗證狀態值
            if (!string.IsNullOrEmpty(dto.Status) && 
                dto.Status != "ACTIVE" && dto.Status != "INACTIVE" && dto.Status != "ARCHIVED")
            {
                throw new ArgumentException("狀態值必須為 ACTIVE、INACTIVE 或 ARCHIVED");
            }

            existing.MasterName = dto.MasterName;
            existing.MasterType = dto.MasterType;
            existing.Category = dto.Category;
            existing.Industry = dto.Industry;
            existing.BusinessType = dto.BusinessType;
            existing.Status = dto.Status ?? existing.Status;
            existing.Priority = dto.Priority;
            existing.Source = dto.Source;
            existing.ContactPerson = dto.ContactPerson;
            existing.ContactTel = dto.ContactTel;
            existing.ContactEmail = dto.ContactEmail;
            existing.ContactAddress = dto.ContactAddress;
            existing.Website = dto.Website;
            existing.Notes = dto.Notes;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改潛客主檔失敗: {masterId}", ex);
            throw;
        }
    }

    public async Task DeleteProspectMasterAsync(string masterId)
    {
        try
        {
            // 檢查主檔是否存在
            var existing = await _repository.GetByIdAsync(masterId);
            if (existing == null)
            {
                throw new InvalidOperationException($"潛客主檔不存在: {masterId}");
            }

            // 檢查是否有關聯的潛客資料
            var hasRelated = await _repository.HasRelatedProspectsAsync(masterId);
            if (hasRelated)
            {
                throw new InvalidOperationException($"無法刪除潛客主檔，因為有關聯的潛客資料: {masterId}");
            }

            await _repository.DeleteAsync(masterId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除潛客主檔失敗: {masterId}", ex);
            throw;
        }
    }

    public async Task BatchDeleteProspectMastersAsync(BatchDeleteProspectMasterDto dto)
    {
        try
        {
            if (dto.MasterIds == null || dto.MasterIds.Count == 0)
            {
                throw new ArgumentException("主檔代碼列表不能為空");
            }

            foreach (var masterId in dto.MasterIds)
            {
                await DeleteProspectMasterAsync(masterId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除潛客主檔失敗", ex);
            throw;
        }
    }

    public async Task UpdateProspectMasterStatusAsync(string masterId, UpdateProspectMasterStatusDto dto)
    {
        try
        {
            // 驗證狀態值
            if (string.IsNullOrEmpty(dto.Status) || 
                (dto.Status != "ACTIVE" && dto.Status != "INACTIVE" && dto.Status != "ARCHIVED"))
            {
                throw new ArgumentException("狀態值必須為 ACTIVE、INACTIVE 或 ARCHIVED");
            }

            // 檢查主檔是否存在
            var existing = await _repository.GetByIdAsync(masterId);
            if (existing == null)
            {
                throw new InvalidOperationException($"潛客主檔不存在: {masterId}");
            }

            existing.Status = dto.Status;
            existing.UpdatedBy = GetCurrentUserId();
            existing.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(existing);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新潛客主檔狀態失敗: {masterId}", ex);
            throw;
        }
    }

    /// <summary>
    /// 將 Entity 轉換為 DTO
    /// </summary>
    private static ProspectMasterDto MapToDto(ProspectMaster entity)
    {
        return new ProspectMasterDto
        {
            MasterId = entity.MasterId,
            MasterName = entity.MasterName,
            MasterType = entity.MasterType,
            Category = entity.Category,
            Industry = entity.Industry,
            BusinessType = entity.BusinessType,
            Status = entity.Status,
            Priority = entity.Priority,
            Source = entity.Source,
            ContactPerson = entity.ContactPerson,
            ContactTel = entity.ContactTel,
            ContactEmail = entity.ContactEmail,
            ContactAddress = entity.ContactAddress,
            Website = entity.Website,
            Notes = entity.Notes,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }
}

