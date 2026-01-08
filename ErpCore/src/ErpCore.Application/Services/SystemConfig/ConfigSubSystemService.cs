using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.SystemConfig;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SystemConfig;

/// <summary>
/// 子系統項目服務實作
/// </summary>
public class ConfigSubSystemService : BaseService, IConfigSubSystemService
{
    private readonly IConfigSubSystemRepository _repository;
    private readonly IConfigSystemRepository _systemRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ConfigSubSystemService(
        IConfigSubSystemRepository repository,
        IConfigSystemRepository systemRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _systemRepository = systemRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ConfigSubSystemDto>> GetConfigSubSystemsAsync(ConfigSubSystemQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConfigSubSystemQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SubSystemId = query.SubSystemId,
                SubSystemName = query.SubSystemName,
                SystemId = query.SystemId,
                ParentSubSystemId = query.ParentSubSystemId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢主系統名稱和上層子系統名稱
            var systemIds = result.Items.Select(x => x.SystemId).Distinct().ToList();
            var parentSubSystemIds = result.Items
                .Where(x => !string.IsNullOrEmpty(x.ParentSubSystemId) && x.ParentSubSystemId != "0")
                .Select(x => x.ParentSubSystemId!)
                .Distinct()
                .ToList();

            var systems = new Dictionary<string, string>();
            var parentSubSystems = new Dictionary<string, string>();

            foreach (var systemId in systemIds)
            {
                var system = await _systemRepository.GetByIdAsync(systemId);
                if (system != null)
                {
                    systems[systemId] = system.SystemName;
                }
            }

            foreach (var parentSubSystemId in parentSubSystemIds)
            {
                var parentSubSystem = await _repository.GetByIdAsync(parentSubSystemId);
                if (parentSubSystem != null)
                {
                    parentSubSystems[parentSubSystemId] = parentSubSystem.SubSystemName;
                }
            }

            var dtos = result.Items.Select(x => new ConfigSubSystemDto
            {
                TKey = x.TKey,
                SubSystemId = x.SubSystemId,
                SubSystemName = x.SubSystemName,
                SeqNo = x.SeqNo,
                SystemId = x.SystemId,
                SystemName = systems.GetValueOrDefault(x.SystemId),
                ParentSubSystemId = x.ParentSubSystemId,
                ParentSubSystemName = !string.IsNullOrEmpty(x.ParentSubSystemId) && x.ParentSubSystemId != "0"
                    ? parentSubSystems.GetValueOrDefault(x.ParentSubSystemId)
                    : null,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ConfigSubSystemDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢子系統列表失敗", ex);
            throw;
        }
    }

    public async Task<ConfigSubSystemDto> GetConfigSubSystemAsync(string subSystemId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(subSystemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"子系統不存在: {subSystemId}");
            }

            // 查詢主系統名稱
            string? systemName = null;
            if (!string.IsNullOrEmpty(entity.SystemId))
            {
                var system = await _systemRepository.GetByIdAsync(entity.SystemId);
                systemName = system?.SystemName;
            }

            // 查詢上層子系統名稱
            string? parentSubSystemName = null;
            if (!string.IsNullOrEmpty(entity.ParentSubSystemId) && entity.ParentSubSystemId != "0")
            {
                var parentSubSystem = await _repository.GetByIdAsync(entity.ParentSubSystemId);
                parentSubSystemName = parentSubSystem?.SubSystemName;
            }

            return new ConfigSubSystemDto
            {
                TKey = entity.TKey,
                SubSystemId = entity.SubSystemId,
                SubSystemName = entity.SubSystemName,
                SeqNo = entity.SeqNo,
                SystemId = entity.SystemId,
                SystemName = systemName,
                ParentSubSystemId = entity.ParentSubSystemId,
                ParentSubSystemName = parentSubSystemName,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢子系統失敗: {subSystemId}", ex);
            throw;
        }
    }

    public async Task<string> CreateConfigSubSystemAsync(CreateConfigSubSystemDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.SubSystemId);
            if (exists)
            {
                throw new InvalidOperationException($"子系統已存在: {dto.SubSystemId}");
            }

            // 檢查主系統是否存在
            var systemExists = await _systemRepository.ExistsAsync(dto.SystemId);
            if (!systemExists)
            {
                throw new InvalidOperationException($"主系統不存在: {dto.SystemId}");
            }

            // 檢查上層子系統是否存在（若不等於 '0'）
            if (!string.IsNullOrEmpty(dto.ParentSubSystemId) && dto.ParentSubSystemId != "0")
            {
                var parentExists = await _repository.ExistsAsync(dto.ParentSubSystemId);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"上層子系統不存在: {dto.ParentSubSystemId}");
                }

                // 檢查循環引用
                if (dto.ParentSubSystemId == dto.SubSystemId)
                {
                    throw new InvalidOperationException("上層子系統代碼不能等於自己");
                }
            }

            var entity = new ConfigSubSystem
            {
                SubSystemId = dto.SubSystemId,
                SubSystemName = dto.SubSystemName,
                SeqNo = dto.SeqNo,
                SystemId = dto.SystemId,
                ParentSubSystemId = string.IsNullOrEmpty(dto.ParentSubSystemId) || dto.ParentSubSystemId == "0" ? null : dto.ParentSubSystemId,
                Status = dto.Status,
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.SubSystemId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增子系統失敗: {dto.SubSystemId}", ex);
            throw;
        }
    }

    public async Task UpdateConfigSubSystemAsync(string subSystemId, UpdateConfigSubSystemDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(subSystemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"子系統不存在: {subSystemId}");
            }

            // 檢查主系統是否存在
            var systemExists = await _systemRepository.ExistsAsync(dto.SystemId);
            if (!systemExists)
            {
                throw new InvalidOperationException($"主系統不存在: {dto.SystemId}");
            }

            // 檢查上層子系統是否存在（若不等於 '0'）
            if (!string.IsNullOrEmpty(dto.ParentSubSystemId) && dto.ParentSubSystemId != "0")
            {
                var parentExists = await _repository.ExistsAsync(dto.ParentSubSystemId);
                if (!parentExists)
                {
                    throw new InvalidOperationException($"上層子系統不存在: {dto.ParentSubSystemId}");
                }

                // 檢查循環引用
                if (dto.ParentSubSystemId == subSystemId)
                {
                    throw new InvalidOperationException("上層子系統代碼不能等於自己");
                }
            }

            entity.SubSystemName = dto.SubSystemName;
            entity.SeqNo = dto.SeqNo;
            entity.SystemId = dto.SystemId;
            entity.ParentSubSystemId = string.IsNullOrEmpty(dto.ParentSubSystemId) || dto.ParentSubSystemId == "0" ? null : dto.ParentSubSystemId;
            entity.Status = dto.Status;
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改子系統失敗: {subSystemId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigSubSystemAsync(string subSystemId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(subSystemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"子系統不存在: {subSystemId}");
            }

            // 檢查是否有下層子系統
            var hasChildren = await _repository.HasChildrenAsync(subSystemId);
            if (hasChildren)
            {
                throw new InvalidOperationException($"子系統有下層子系統，無法刪除: {subSystemId}");
            }

            // 檢查是否有作業
            var hasPrograms = await _repository.HasProgramsAsync(subSystemId);
            if (hasPrograms)
            {
                throw new InvalidOperationException($"子系統有作業，無法刪除: {subSystemId}");
            }

            await _repository.DeleteAsync(subSystemId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除子系統失敗: {subSystemId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigSubSystemsBatchAsync(BatchDeleteConfigSubSystemDto dto)
    {
        try
        {
            foreach (var subSystemId in dto.SubSystemIds)
            {
                await DeleteConfigSubSystemAsync(subSystemId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除子系統失敗", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateConfigSubSystemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.SubSystemId))
        {
            throw new ArgumentException("子系統項目代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.SubSystemName))
        {
            throw new ArgumentException("子系統項目名稱不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.SystemId))
        {
            throw new ArgumentException("主系統代碼不能為空");
        }

        if (dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態必須為 A(啟用) 或 I(停用)");
        }
    }
}

