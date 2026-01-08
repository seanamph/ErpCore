using ErpCore.Application.DTOs.SystemConfig;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.SystemConfig;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.SystemConfig;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.SystemConfig;

/// <summary>
/// 系統功能按鈕服務實作
/// </summary>
public class ConfigButtonService : BaseService, IConfigButtonService
{
    private readonly IConfigButtonRepository _repository;
    private readonly IConfigProgramRepository _programRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ConfigButtonService(
        IConfigButtonRepository repository,
        IConfigProgramRepository programRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _programRepository = programRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ConfigButtonDto>> GetConfigButtonsAsync(ConfigButtonQueryDto query)
    {
        try
        {
            var repositoryQuery = new ConfigButtonQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ButtonId = query.ButtonId,
                ButtonName = query.ButtonName,
                ProgramId = query.ProgramId,
                ButtonType = query.ButtonType,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢作業名稱
            var programIds = result.Items.Select(x => x.ProgramId).Distinct().ToList();
            var programs = new Dictionary<string, string>();

            foreach (var programId in programIds)
            {
                var program = await _programRepository.GetByIdAsync(programId);
                if (program != null)
                {
                    programs[programId] = program.ProgramName;
                }
            }

            var dtos = result.Items.Select(x => new ConfigButtonDto
            {
                ButtonId = x.ButtonId,
                ProgramId = x.ProgramId,
                ProgramName = programs.GetValueOrDefault(x.ProgramId),
                ButtonName = x.ButtonName,
                ButtonType = x.ButtonType,
                SeqNo = x.SeqNo,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ConfigButtonDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢按鈕列表失敗", ex);
            throw;
        }
    }

    public async Task<ConfigButtonDto> GetConfigButtonAsync(string buttonId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(buttonId);
            if (entity == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {buttonId}");
            }

            // 查詢作業名稱
            string? programName = null;
            if (!string.IsNullOrEmpty(entity.ProgramId))
            {
                var program = await _programRepository.GetByIdAsync(entity.ProgramId);
                programName = program?.ProgramName;
            }

            return new ConfigButtonDto
            {
                ButtonId = entity.ButtonId,
                ProgramId = entity.ProgramId,
                ProgramName = programName,
                ButtonName = entity.ButtonName,
                ButtonType = entity.ButtonType,
                SeqNo = entity.SeqNo,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢按鈕失敗: {buttonId}", ex);
            throw;
        }
    }

    public async Task<string> CreateConfigButtonAsync(CreateConfigButtonDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.ButtonId);
            if (exists)
            {
                throw new InvalidOperationException($"按鈕已存在: {dto.ButtonId}");
            }

            // 檢查作業是否存在
            var programExists = await _programRepository.ExistsAsync(dto.ProgramId);
            if (!programExists)
            {
                throw new InvalidOperationException($"作業不存在: {dto.ProgramId}");
            }

            var entity = new ConfigButton
            {
                ButtonId = dto.ButtonId,
                ProgramId = dto.ProgramId,
                ButtonName = dto.ButtonName,
                ButtonType = dto.ButtonType,
                SeqNo = dto.SeqNo,
                Status = dto.Status,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now
            };

            await _repository.CreateAsync(entity);

            return entity.ButtonId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增按鈕失敗: {dto.ButtonId}", ex);
            throw;
        }
    }

    public async Task UpdateConfigButtonAsync(string buttonId, UpdateConfigButtonDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(buttonId);
            if (entity == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {buttonId}");
            }

            // 檢查作業是否存在
            var programExists = await _programRepository.ExistsAsync(dto.ProgramId);
            if (!programExists)
            {
                throw new InvalidOperationException($"作業不存在: {dto.ProgramId}");
            }

            entity.ProgramId = dto.ProgramId;
            entity.ButtonName = dto.ButtonName;
            entity.ButtonType = dto.ButtonType;
            entity.SeqNo = dto.SeqNo;
            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改按鈕失敗: {buttonId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigButtonAsync(string buttonId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(buttonId);
            if (entity == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {buttonId}");
            }

            await _repository.DeleteAsync(buttonId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除按鈕失敗: {buttonId}", ex);
            throw;
        }
    }

    public async Task DeleteConfigButtonsBatchAsync(BatchDeleteConfigButtonDto dto)
    {
        try
        {
            foreach (var buttonId in dto.ButtonIds)
            {
                await DeleteConfigButtonAsync(buttonId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除按鈕失敗", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateConfigButtonDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ButtonId))
        {
            throw new ArgumentException("按鈕代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ButtonName))
        {
            throw new ArgumentException("按鈕名稱不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ProgramId))
        {
            throw new ArgumentException("作業代碼不能為空");
        }

        if (dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態必須為 A(啟用) 或 I(停用)");
        }
    }
}

