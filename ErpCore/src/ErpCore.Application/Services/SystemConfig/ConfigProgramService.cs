using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.SystemConfig;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SystemConfig;

/// <summary>
/// 系統作業服務實作
/// </summary>
public class ConfigProgramService : BaseService, IConfigProgramService
{
    private readonly IConfigProgramRepository _repository;
    private readonly IConfigSystemRepository _systemRepository;
    private readonly IConfigSubSystemRepository _subSystemRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ConfigProgramService(
        IConfigProgramRepository repository,
        IConfigSystemRepository systemRepository,
        IConfigSubSystemRepository subSystemRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _systemRepository = systemRepository;
        _subSystemRepository = subSystemRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ConfigProgramDto>> GetConfigProgramsAsync(ConfigProgramQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConfigProgramQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ProgramId = query.ProgramId,
                ProgramName = query.ProgramName,
                SystemId = query.SystemId,
                SubSystemId = query.SubSystemId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢主系統名稱和子系統名稱
            var systemIds = result.Items.Select(x => x.SystemId).Distinct().ToList();
            var subSystemIds = result.Items
                .Where(x => !string.IsNullOrEmpty(x.SubSystemId))
                .Select(x => x.SubSystemId!)
                .Distinct()
                .ToList();

            var systems = new Dictionary<string, string>();
            var subSystems = new Dictionary<string, string>();

            foreach (var systemId in systemIds)
            {
                var system = await _systemRepository.GetByIdAsync(systemId);
                if (system != null)
                {
                    systems[systemId] = system.SystemName;
                }
            }

            foreach (var subSystemId in subSystemIds)
            {
                var subSystem = await _subSystemRepository.GetByIdAsync(subSystemId);
                if (subSystem != null)
                {
                    subSystems[subSystemId] = subSystem.SubSystemName;
                }
            }

            var dtos = result.Items.Select(x => new ConfigProgramDto
            {
                TKey = x.TKey,
                ProgramId = x.ProgramId,
                ProgramName = x.ProgramName,
                SeqNo = x.SeqNo,
                SystemId = x.SystemId,
                SystemName = systems.GetValueOrDefault(x.SystemId),
                SubSystemId = x.SubSystemId,
                SubSystemName = !string.IsNullOrEmpty(x.SubSystemId)
                    ? subSystems.GetValueOrDefault(x.SubSystemId)
                    : null,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ConfigProgramDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢作業列表失敗", ex);
            throw;
        }
    }

    public async Task<ConfigProgramDto> GetConfigProgramAsync(string programId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(programId);
            if (entity == null)
            {
                throw new InvalidOperationException($"作業不存在: {programId}");
            }

            // 查詢主系統名稱
            string? systemName = null;
            if (!string.IsNullOrEmpty(entity.SystemId))
            {
                var system = await _systemRepository.GetByIdAsync(entity.SystemId);
                systemName = system?.SystemName;
            }

            // 查詢子系統名稱
            string? subSystemName = null;
            if (!string.IsNullOrEmpty(entity.SubSystemId))
            {
                var subSystem = await _subSystemRepository.GetByIdAsync(entity.SubSystemId);
                subSystemName = subSystem?.SubSystemName;
            }

            return new ConfigProgramDto
            {
                TKey = entity.TKey,
                ProgramId = entity.ProgramId,
                ProgramName = entity.ProgramName,
                SeqNo = entity.SeqNo,
                SystemId = entity.SystemId,
                SystemName = systemName,
                SubSystemId = entity.SubSystemId,
                SubSystemName = subSystemName,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task<string> CreateConfigProgramAsync(CreateConfigProgramDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.ProgramId);
            if (exists)
            {
                throw new InvalidOperationException($"作業已存在: {dto.ProgramId}");
            }

            // 檢查主系統是否存在
            var systemExists = await _systemRepository.ExistsAsync(dto.SystemId);
            if (!systemExists)
            {
                throw new InvalidOperationException($"主系統不存在: {dto.SystemId}");
            }

            // 檢查子系統是否存在（若提供）
            if (!string.IsNullOrEmpty(dto.SubSystemId))
            {
                var subSystemExists = await _subSystemRepository.ExistsAsync(dto.SubSystemId);
                if (!subSystemExists)
                {
                    throw new InvalidOperationException($"子系統不存在: {dto.SubSystemId}");
                }
            }

            var entity = new ConfigProgram
            {
                ProgramId = dto.ProgramId,
                ProgramName = dto.ProgramName,
                SeqNo = dto.SeqNo,
                SystemId = dto.SystemId,
                SubSystemId = string.IsNullOrEmpty(dto.SubSystemId) ? null : dto.SubSystemId,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.ProgramId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增作業失敗: {dto.ProgramId}", ex);
            throw;
        }
    }

    public async Task UpdateConfigProgramAsync(string programId, UpdateConfigProgramDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(programId);
            if (entity == null)
            {
                throw new InvalidOperationException($"作業不存在: {programId}");
            }

            // 檢查主系統是否存在
            var systemExists = await _systemRepository.ExistsAsync(dto.SystemId);
            if (!systemExists)
            {
                throw new InvalidOperationException($"主系統不存在: {dto.SystemId}");
            }

            // 檢查子系統是否存在（若提供）
            if (!string.IsNullOrEmpty(dto.SubSystemId))
            {
                var subSystemExists = await _subSystemRepository.ExistsAsync(dto.SubSystemId);
                if (!subSystemExists)
                {
                    throw new InvalidOperationException($"子系統不存在: {dto.SubSystemId}");
                }
            }

            entity.ProgramName = dto.ProgramName;
            entity.SeqNo = dto.SeqNo;
            entity.SystemId = dto.SystemId;
            entity.SubSystemId = string.IsNullOrEmpty(dto.SubSystemId) ? null : dto.SubSystemId;
            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigProgramAsync(string programId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(programId);
            if (entity == null)
            {
                throw new InvalidOperationException($"作業不存在: {programId}");
            }

            // 檢查是否有按鈕
            var hasButtons = await _repository.HasButtonsAsync(programId);
            if (hasButtons)
            {
                throw new InvalidOperationException($"作業有按鈕，無法刪除: {programId}");
            }

            await _repository.DeleteAsync(programId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigProgramsBatchAsync(BatchDeleteConfigProgramDto dto)
    {
        try
        {
            foreach (var programId in dto.ProgramIds)
            {
                await DeleteConfigProgramAsync(programId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除作業失敗", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateConfigProgramDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ProgramId))
        {
            throw new ArgumentException("作業代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ProgramName))
        {
            throw new ArgumentException("作業名稱不能為空");
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
