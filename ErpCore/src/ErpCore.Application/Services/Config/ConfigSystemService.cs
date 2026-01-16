using ErpCore.Application.DTOs.Config;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Config;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.Config;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Config;

/// <summary>
/// 主系統項目服務實作 (CFG0410)
/// </summary>
public class ConfigSystemService : BaseService, IConfigSystemService
{
    private readonly IConfigSystemRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ConfigSystemService(
        IConfigSystemRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ConfigSystemDto>> GetConfigSystemsAsync(ConfigSystemQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConfigSystemQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SystemId = query.SystemId,
                SystemName = query.SystemName,
                SystemType = query.SystemType,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(item => new ConfigSystemDto
            {
                SystemId = item.SystemId,
                SystemName = item.SystemName,
                SeqNo = item.SeqNo,
                SystemType = item.SystemType,
                ServerIp = item.ServerIp,
                ModuleId = item.ModuleId,
                DbUser = item.DbUser,
                DbPass = item.DbPass, // 注意：實際應用中可能需要隱藏密碼
                Notes = item.Notes,
                Status = item.Status,
                CreatedBy = item.CreatedBy,
                CreatedAt = item.CreatedAt,
                UpdatedBy = item.UpdatedBy,
                UpdatedAt = item.UpdatedAt
            }).ToList();

            return new PagedResult<ConfigSystemDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢主系統列表失敗", ex);
            throw;
        }
    }

    public async Task<ConfigSystemDto> GetConfigSystemByIdAsync(string systemId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(systemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"主系統不存在: {systemId}");
            }

            return new ConfigSystemDto
            {
                SystemId = entity.SystemId,
                SystemName = entity.SystemName,
                SeqNo = entity.SeqNo,
                SystemType = entity.SystemType,
                ServerIp = entity.ServerIp,
                ModuleId = entity.ModuleId,
                DbUser = entity.DbUser,
                DbPass = entity.DbPass, // 注意：實際應用中可能需要隱藏密碼
                Notes = entity.Notes,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task<string> CreateConfigSystemAsync(CreateConfigSystemDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.SystemId);
            if (exists)
            {
                throw new InvalidOperationException($"主系統已存在: {dto.SystemId}");
            }

            var entity = new ConfigSystem
            {
                SystemId = dto.SystemId,
                SystemName = dto.SystemName,
                SeqNo = dto.SeqNo,
                SystemType = dto.SystemType,
                ServerIp = dto.ServerIp,
                ModuleId = dto.ModuleId,
                DbUser = dto.DbUser,
                DbPass = dto.DbPass, // 注意：實際應用中需要加密儲存
                Notes = dto.Notes,
                Status = dto.Status ?? "A",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            await _repository.CreateAsync(entity);

            return entity.SystemId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增主系統失敗: {dto.SystemId}", ex);
            throw;
        }
    }

    public async Task UpdateConfigSystemAsync(string systemId, UpdateConfigSystemDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(systemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"主系統不存在: {systemId}");
            }

            entity.SystemName = dto.SystemName;
            entity.SeqNo = dto.SeqNo;
            entity.SystemType = dto.SystemType;
            entity.ServerIp = dto.ServerIp;
            entity.ModuleId = dto.ModuleId;
            entity.DbUser = dto.DbUser;
            // 只有當密碼不為空時才更新
            if (!string.IsNullOrEmpty(dto.DbPass))
            {
                entity.DbPass = dto.DbPass; // 注意：實際應用中需要加密儲存
            }
            entity.Notes = dto.Notes;
            entity.Status = dto.Status ?? "A";
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigSystemAsync(string systemId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(systemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"主系統不存在: {systemId}");
            }

            await _repository.DeleteAsync(systemId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigSystemsBatchAsync(BatchDeleteConfigSystemDto dto)
    {
        try
        {
            foreach (var systemId in dto.SystemIds)
            {
                await DeleteConfigSystemAsync(systemId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除主系統失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string systemId, UpdateConfigSystemStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(systemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"主系統不存在: {systemId}");
            }

            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新主系統狀態失敗: {systemId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateConfigSystemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.SystemId))
        {
            throw new ArgumentException("主系統代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.SystemName))
        {
            throw new ArgumentException("主系統名稱不能為空");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A (啟用) 或 I (停用)");
        }
    }
}
