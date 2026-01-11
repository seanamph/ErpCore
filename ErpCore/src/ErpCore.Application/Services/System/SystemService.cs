using Dapper;
using ErpCore.Application.DTOs.System;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.System;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.System;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.System;

/// <summary>
/// 主系統項目服務實作 (SYS0410)
/// </summary>
public class SystemService : BaseService, ISystemService
{
    private readonly ISystemRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public SystemService(
        ISystemRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<SystemDto>> GetSystemsAsync(SystemQueryDto query)
    {
        try
        {
            var repositoryQuery = new SystemQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                SystemId = query.Filters?.SystemId,
                SystemName = query.Filters?.SystemName,
                SystemType = query.Filters?.SystemType,
                ServerIp = query.Filters?.ServerIp
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 需要单独查询 SystemTypeName，因为 Dapper 不支持动态属性映射
            var dtos = new List<SystemDto>();
            foreach (var item in result.Items)
            {
                var dto = new SystemDto
                {
                    SystemId = item.SystemId,
                    SystemName = item.SystemName,
                    SeqNo = item.SeqNo,
                    SystemType = item.SystemType,
                    ServerIp = item.ServerIp,
                    ModuleId = item.ModuleId,
                    DbUser = item.DbUser,
                    Notes = item.Notes,
                    Status = item.Status,
                    CreatedBy = item.CreatedBy,
                    CreatedAt = item.CreatedAt,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedAt = item.UpdatedAt
                };

                // 查询系统型态名称
                if (!string.IsNullOrEmpty(item.SystemType))
                {
                    var systemTypeName = await GetSystemTypeNameAsync(item.SystemType);
                    dto.SystemTypeName = systemTypeName;
                }

                dtos.Add(dto);
            }

            return new PagedResult<SystemDto>
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

    public async Task<SystemDto> GetSystemAsync(string systemId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(systemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"主系統不存在: {systemId}");
            }

            var dto = new SystemDto
            {
                SystemId = entity.SystemId,
                SystemName = entity.SystemName,
                SeqNo = entity.SeqNo,
                SystemType = entity.SystemType,
                ServerIp = entity.ServerIp,
                ModuleId = entity.ModuleId,
                DbUser = entity.DbUser,
                Notes = entity.Notes,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };

            // 查询系统型态名称
            if (!string.IsNullOrEmpty(entity.SystemType))
            {
                dto.SystemTypeName = await GetSystemTypeNameAsync(entity.SystemType);
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task<string> CreateSystemAsync(CreateSystemDto dto)
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

            var entity = new Systems
            {
                SystemId = dto.SystemId,
                SystemName = dto.SystemName,
                SeqNo = dto.SeqNo,
                SystemType = dto.SystemType,
                ServerIp = dto.ServerIp,
                ModuleId = dto.ModuleId,
                DbUser = dto.DbUser,
                DbPass = EncryptPassword(dto.DbPass), // 加密密碼
                Notes = dto.Notes,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null, // 可從 UserContext 取得
                CreatedGroup = null // 可從 UserContext 取得
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

    public async Task UpdateSystemAsync(string systemId, UpdateSystemDto dto)
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
            if (!string.IsNullOrEmpty(dto.DbPass))
            {
                entity.DbPass = EncryptPassword(dto.DbPass); // 加密密碼
            }
            entity.Notes = dto.Notes;
            entity.Status = dto.Status;
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

    public async Task DeleteSystemAsync(string systemId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(systemId);
            if (entity == null)
            {
                throw new InvalidOperationException($"主系統不存在: {systemId}");
            }

            // 檢查是否有子系統關聯
            var hasSubSystems = await _repository.HasSubSystemsAsync(systemId);
            if (hasSubSystems)
            {
                throw new InvalidOperationException("此主系統下存在子系統，無法刪除");
            }

            // 檢查是否有作業關聯
            var hasPrograms = await _repository.HasProgramsAsync(systemId);
            if (hasPrograms)
            {
                throw new InvalidOperationException("此主系統下存在作業，無法刪除");
            }

            await _repository.DeleteAsync(systemId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除主系統失敗: {systemId}", ex);
            throw;
        }
    }

    public async Task DeleteSystemsBatchAsync(BatchDeleteSystemsDto dto)
    {
        try
        {
            foreach (var systemId in dto.SystemIds)
            {
                await DeleteSystemAsync(systemId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除主系統失敗", ex);
            throw;
        }
    }

    public async Task<List<SystemTypeOptionDto>> GetSystemTypesAsync()
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Tag AS Value, Content AS Label
                FROM Parameters
                WHERE Title = 'PROG_TYPE'
                ORDER BY Tag";

            var result = await connection.QueryAsync<SystemTypeOptionDto>(sql);
            return result.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("取得系統型態選項失敗", ex);
            throw;
        }
    }

    private async Task<string?> GetSystemTypeNameAsync(string systemType)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Content
                FROM Parameters
                WHERE Title = 'PROG_TYPE' AND Tag = @SystemType";

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { SystemType = systemType });
            return result;
        }
        catch
        {
            return null;
        }
    }

    private void ValidateCreateDto(CreateSystemDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.SystemId))
        {
            throw new ArgumentException("主系統代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.SystemName))
        {
            throw new ArgumentException("主系統名稱不能為空");
        }

        if (dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態必須為 A(啟用) 或 I(停用)");
        }
    }

    private string? EncryptPassword(string? password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return null;
        }

        // TODO: 實作密碼加密邏輯
        // 可以使用 AES 加密或其他加密方式
        // 暫時直接返回，需實作加密
        return password;
    }
}
