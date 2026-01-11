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
/// 系統功能按鈕服務實作 (SYS0440)
/// </summary>
public class ButtonService : BaseService, IButtonService
{
    private readonly IButtonRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ButtonService(
        IButtonRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ButtonDto>> GetButtonsAsync(ButtonQueryDto query)
    {
        try
        {
            var repositoryQuery = new ButtonQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ProgramId = query.Filters?.ProgramId,
                ButtonId = query.Filters?.ButtonId,
                ButtonName = query.Filters?.ButtonName,
                PageId = query.Filters?.PageId
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢作業名稱和訊息型態名稱
            var programIds = result.Items.Select(x => x.ProgramId).Distinct().ToList();
            var msgTypes = result.Items
                .Where(x => !string.IsNullOrEmpty(x.MsgType))
                .Select(x => x.MsgType!)
                .Distinct()
                .ToList();

            var programs = new Dictionary<string, string>();
            var msgTypeNames = new Dictionary<string, string>();

            // 查詢作業名稱（從 Programs 表）
            if (programIds.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                const string programSql = @"
                    SELECT ProgramId, ProgramName 
                    FROM Programs 
                    WHERE ProgramId IN @ProgramIds";
                var programList = await connection.QueryAsync<dynamic>(programSql, new { ProgramIds = programIds });
                foreach (var program in programList)
                {
                    programs[program.ProgramId] = program.ProgramName;
                }
            }

            // 查詢訊息型態名稱（從 Parameters 表）
            if (msgTypes.Any())
            {
                using var connection = _connectionFactory.CreateConnection();
                const string paramSql = @"
                    SELECT Value, Name 
                    FROM Parameters 
                    WHERE Title = 'BUT_MSG' AND Value IN @MsgTypes";
                var paramList = await connection.QueryAsync<dynamic>(paramSql, new { MsgTypes = msgTypes });
                foreach (var param in paramList)
                {
                    msgTypeNames[param.Value] = param.Name;
                }
            }

            var dtos = result.Items.Select(x => new ButtonDto
            {
                TKey = x.TKey,
                ProgramId = x.ProgramId,
                ProgramName = programs.GetValueOrDefault(x.ProgramId),
                ButtonId = x.ButtonId,
                ButtonName = x.ButtonName,
                PageId = x.PageId,
                ButtonMsg = x.ButtonMsg,
                ButtonAttr = x.ButtonAttr,
                ButtonUrl = x.ButtonUrl,
                MsgType = x.MsgType,
                MsgTypeName = !string.IsNullOrEmpty(x.MsgType) ? msgTypeNames.GetValueOrDefault(x.MsgType) : null,
                Status = x.Status,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

            return new PagedResult<ButtonDto>
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

    public async Task<ButtonDto> GetButtonByIdAsync(long tKey)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {tKey}");
            }

            // 查詢作業名稱
            string? programName = null;
            if (!string.IsNullOrEmpty(entity.ProgramId))
            {
                using var connection = _connectionFactory.CreateConnection();
                const string programSql = @"
                    SELECT ProgramName 
                    FROM Programs 
                    WHERE ProgramId = @ProgramId";
                var program = await connection.QueryFirstOrDefaultAsync<dynamic>(programSql, new { ProgramId = entity.ProgramId });
                programName = program?.ProgramName;
            }

            // 查詢訊息型態名稱
            string? msgTypeName = null;
            if (!string.IsNullOrEmpty(entity.MsgType))
            {
                using var connection = _connectionFactory.CreateConnection();
                const string paramSql = @"
                    SELECT Name 
                    FROM Parameters 
                    WHERE Title = 'BUT_MSG' AND Value = @MsgType";
                var param = await connection.QueryFirstOrDefaultAsync<dynamic>(paramSql, new { MsgType = entity.MsgType });
                msgTypeName = param?.Name;
            }

            return new ButtonDto
            {
                TKey = entity.TKey,
                ProgramId = entity.ProgramId,
                ProgramName = programName,
                ButtonId = entity.ButtonId,
                ButtonName = entity.ButtonName,
                PageId = entity.PageId,
                ButtonMsg = entity.ButtonMsg,
                ButtonAttr = entity.ButtonAttr,
                ButtonUrl = entity.ButtonUrl,
                MsgType = entity.MsgType,
                MsgTypeName = msgTypeName,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢按鈕失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task<long> CreateButtonAsync(CreateButtonDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查作業是否存在
            using var connection = _connectionFactory.CreateConnection();
            const string checkProgramSql = @"
                SELECT COUNT(*) 
                FROM Programs 
                WHERE ProgramId = @ProgramId";
            var programExists = await connection.QuerySingleAsync<int>(checkProgramSql, new { ProgramId = dto.ProgramId });
            if (programExists == 0)
            {
                throw new InvalidOperationException($"作業不存在: {dto.ProgramId}");
            }

            var entity = new Button
            {
                ProgramId = dto.ProgramId,
                ButtonId = dto.ButtonId,
                ButtonName = dto.ButtonName,
                PageId = dto.PageId,
                ButtonMsg = dto.ButtonMsg,
                ButtonAttr = dto.ButtonAttr,
                ButtonUrl = dto.ButtonUrl,
                MsgType = dto.MsgType,
                Status = "1",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = GetCurrentUserPriority(),
                CreatedGroup = GetCurrentUserGroup()
            };

            var result = await _repository.CreateAsync(entity);

            return result.TKey;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增按鈕失敗: {dto.ButtonId}", ex);
            throw;
        }
    }

    public async Task UpdateButtonAsync(long tKey, UpdateButtonDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {tKey}");
            }

            // 檢查作業是否存在
            using var connection = _connectionFactory.CreateConnection();
            const string checkProgramSql = @"
                SELECT COUNT(*) 
                FROM Programs 
                WHERE ProgramId = @ProgramId";
            var programExists = await connection.QuerySingleAsync<int>(checkProgramSql, new { ProgramId = dto.ProgramId });
            if (programExists == 0)
            {
                throw new InvalidOperationException($"作業不存在: {dto.ProgramId}");
            }

            entity.ProgramId = dto.ProgramId;
            entity.ButtonId = dto.ButtonId;
            entity.ButtonName = dto.ButtonName;
            entity.PageId = dto.PageId;
            entity.ButtonMsg = dto.ButtonMsg;
            entity.ButtonAttr = dto.ButtonAttr;
            entity.ButtonUrl = dto.ButtonUrl;
            entity.MsgType = dto.MsgType;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改按鈕失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task DeleteButtonAsync(long tKey)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(tKey);
            if (entity == null)
            {
                throw new InvalidOperationException($"按鈕不存在: {tKey}");
            }

            // 檢查是否有權限設定
            var hasPermissions = await _repository.HasPermissionsAsync(tKey);
            if (hasPermissions)
            {
                throw new InvalidOperationException($"按鈕已有權限設定，無法刪除: {tKey}");
            }

            await _repository.DeleteAsync(tKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除按鈕失敗: {tKey}", ex);
            throw;
        }
    }

    public async Task BatchDeleteButtonsAsync(BatchDeleteButtonDto dto)
    {
        try
        {
            foreach (var tKey in dto.TKeys)
            {
                await DeleteButtonAsync(tKey);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除按鈕失敗", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateButtonDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ProgramId))
        {
            throw new ArgumentException("作業代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ButtonId))
        {
            throw new ArgumentException("按鈕代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ButtonName))
        {
            throw new ArgumentException("按鈕名稱不能為空");
        }
    }
}
