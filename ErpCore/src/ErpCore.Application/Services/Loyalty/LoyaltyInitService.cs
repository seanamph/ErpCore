using ErpCore.Application.DTOs.Loyalty;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.Loyalty;
using ErpCore.Infrastructure.Repositories.Loyalty;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.Loyalty;

/// <summary>
/// 忠誠度系統初始化服務實作 (WEBLOYALTYINI - 忠誠度系統初始化)
/// </summary>
public class LoyaltyInitService : BaseService, ILoyaltyInitService
{
    private readonly ILoyaltySystemConfigRepository _configRepository;
    private readonly ILoyaltySystemInitLogRepository _initLogRepository;

    public LoyaltyInitService(
        ILoyaltySystemConfigRepository configRepository,
        ILoyaltySystemInitLogRepository initLogRepository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _configRepository = configRepository;
        _initLogRepository = initLogRepository;
    }

    public async Task<PagedResult<LoyaltySystemConfigDto>> GetConfigsAsync(LoyaltySystemConfigQueryDto query)
    {
        try
        {
            var repositoryQuery = new LoyaltySystemConfigQuery
            {
                ConfigId = query.ConfigId,
                ConfigType = query.ConfigType,
                Status = query.Status,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _configRepository.QueryAsync(repositoryQuery);
            var totalCount = await _configRepository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToConfigDto).ToList();

            return new PagedResult<LoyaltySystemConfigDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度系統設定列表失敗", ex);
            throw;
        }
    }

    public async Task<LoyaltySystemConfigDto?> GetConfigByIdAsync(string configId)
    {
        try
        {
            var entity = await _configRepository.GetByIdAsync(configId);
            return entity != null ? MapToConfigDto(entity) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢忠誠度系統設定失敗: {configId}", ex);
            throw;
        }
    }

    public async Task<string> CreateConfigAsync(CreateLoyaltySystemConfigDto dto)
    {
        try
        {
            var entity = new LoyaltySystemConfig
            {
                ConfigId = dto.ConfigId,
                ConfigName = dto.ConfigName,
                ConfigValue = dto.ConfigValue,
                ConfigType = dto.ConfigType,
                Description = dto.Description,
                Status = dto.Status,
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now,
                UpdatedBy = _userContext.UserId,
                UpdatedAt = DateTime.Now
            };

            return await _configRepository.CreateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增忠誠度系統設定失敗: {dto.ConfigId}", ex);
            throw;
        }
    }

    public async Task UpdateConfigAsync(string configId, UpdateLoyaltySystemConfigDto dto)
    {
        try
        {
            var entity = await _configRepository.GetByIdAsync(configId);
            if (entity == null)
            {
                throw new Exception($"忠誠度系統設定不存在: {configId}");
            }

            entity.ConfigName = dto.ConfigName;
            entity.ConfigValue = dto.ConfigValue;
            entity.ConfigType = dto.ConfigType;
            entity.Description = dto.Description;
            entity.Status = dto.Status;
            entity.UpdatedBy = _userContext.UserId;
            entity.UpdatedAt = DateTime.Now;

            await _configRepository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改忠誠度系統設定失敗: {configId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigAsync(string configId)
    {
        try
        {
            await _configRepository.DeleteAsync(configId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除忠誠度系統設定失敗: {configId}", ex);
            throw;
        }
    }

    public async Task<LoyaltySystemInitResponseDto> InitializeAsync(InitializeLoyaltySystemDto dto)
    {
        try
        {
            var initId = await _initLogRepository.GenerateInitIdAsync();

            // 更新或建立系統設定
            foreach (var configItem in dto.Configs)
            {
                var existingConfig = await _configRepository.GetByIdAsync(configItem.ConfigId);
                if (existingConfig != null)
                {
                    existingConfig.ConfigValue = configItem.ConfigValue;
                    existingConfig.UpdatedBy = _userContext.UserId;
                    existingConfig.UpdatedAt = DateTime.Now;
                    await _configRepository.UpdateAsync(existingConfig);
                }
                else
                {
                    var newConfig = new LoyaltySystemConfig
                    {
                        ConfigId = configItem.ConfigId,
                        ConfigName = configItem.ConfigId,
                        ConfigValue = configItem.ConfigValue,
                        ConfigType = "PARAM",
                        Status = "A",
                        CreatedBy = _userContext.UserId,
                        CreatedAt = DateTime.Now,
                        UpdatedBy = _userContext.UserId,
                        UpdatedAt = DateTime.Now
                    };
                    await _configRepository.CreateAsync(newConfig);
                }
            }

            // 記錄初始化日誌
            var initLog = new LoyaltySystemInitLog
            {
                InitId = initId,
                InitStatus = "SUCCESS",
                InitDate = DateTime.Now,
                InitMessage = "初始化成功",
                CreatedBy = _userContext.UserId,
                CreatedAt = DateTime.Now
            };
            await _initLogRepository.CreateAsync(initLog);

            return new LoyaltySystemInitResponseDto
            {
                InitId = initId,
                InitStatus = "SUCCESS",
                InitMessage = "初始化成功"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("執行忠誠度系統初始化失敗", ex);

            // 記錄失敗日誌
            try
            {
                var initId = await _initLogRepository.GenerateInitIdAsync();
                var initLog = new LoyaltySystemInitLog
                {
                    InitId = initId,
                    InitStatus = "FAILED",
                    InitDate = DateTime.Now,
                    InitMessage = ex.Message,
                    CreatedBy = _userContext.UserId,
                    CreatedAt = DateTime.Now
                };
                await _initLogRepository.CreateAsync(initLog);
            }
            catch
            {
                // 忽略日誌記錄錯誤
            }

            throw;
        }
    }

    public async Task<PagedResult<LoyaltySystemInitLogDto>> GetInitLogsAsync(LoyaltySystemInitLogQueryDto query)
    {
        try
        {
            var repositoryQuery = new LoyaltySystemInitLogQuery
            {
                InitId = query.InitId,
                InitStatus = query.InitStatus,
                InitDateFrom = query.InitDateFrom,
                InitDateTo = query.InitDateTo,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder
            };

            var items = await _initLogRepository.QueryAsync(repositoryQuery);
            var totalCount = await _initLogRepository.GetCountAsync(repositoryQuery);

            var dtos = items.Select(MapToInitLogDto).ToList();

            return new PagedResult<LoyaltySystemInitLogDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageIndex = query.PageIndex,
                PageSize = query.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢忠誠度系統初始化記錄列表失敗", ex);
            throw;
        }
    }

    private static LoyaltySystemConfigDto MapToConfigDto(LoyaltySystemConfig entity)
    {
        return new LoyaltySystemConfigDto
        {
            ConfigId = entity.ConfigId,
            ConfigName = entity.ConfigName,
            ConfigValue = entity.ConfigValue,
            ConfigType = entity.ConfigType,
            Description = entity.Description,
            Status = entity.Status,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt,
            UpdatedBy = entity.UpdatedBy,
            UpdatedAt = entity.UpdatedAt
        };
    }

    private static LoyaltySystemInitLogDto MapToInitLogDto(LoyaltySystemInitLog entity)
    {
        return new LoyaltySystemInitLogDto
        {
            TKey = entity.TKey,
            InitId = entity.InitId,
            InitStatus = entity.InitStatus,
            InitDate = entity.InitDate,
            InitMessage = entity.InitMessage,
            CreatedBy = entity.CreatedBy,
            CreatedAt = entity.CreatedAt
        };
    }
}

