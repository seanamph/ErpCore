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
/// 系統作業服務實作 (SYS0430)
/// </summary>
public class ProgramService : BaseService, IProgramService
{
    private readonly IProgramRepository _repository;
    private readonly IMenuRepository _menuRepository;
    private readonly IDbConnectionFactory _connectionFactory;

    public ProgramService(
        IProgramRepository repository,
        IMenuRepository menuRepository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _menuRepository = menuRepository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<ProgramDto>> GetProgramsAsync(ProgramQueryDto query)
    {
        try
        {
            var repositoryQuery = new ProgramQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                ProgramId = query.Filters?.ProgramId,
                ProgramName = query.Filters?.ProgramName,
                MenuId = query.Filters?.MenuId,
                ProgramType = query.Filters?.ProgramType
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 需要单独查询 MenuName, ProgramTypeName, MaintainUserName
            var dtos = new List<ProgramDto>();
            foreach (var item in result.Items)
            {
                var dto = new ProgramDto
                {
                    TKey = item.TKey,
                    ProgramId = item.ProgramId,
                    ProgramName = item.ProgramName,
                    SeqNo = item.SeqNo,
                    MenuId = item.MenuId,
                    ProgramUrl = item.ProgramUrl,
                    ProgramType = item.ProgramType,
                    MaintainUserId = item.MaintainUserId,
                    Status = item.Status,
                    CreatedBy = item.CreatedBy,
                    CreatedAt = item.CreatedAt,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedAt = item.UpdatedAt
                };

                // 查询子系统名称
                if (!string.IsNullOrEmpty(item.MenuId))
                {
                    var menuName = await GetMenuNameAsync(item.MenuId);
                    dto.MenuName = menuName;
                }

                // 查询作业型态名称
                if (!string.IsNullOrEmpty(item.ProgramType))
                {
                    var programTypeName = await GetProgramTypeNameAsync(item.ProgramType);
                    dto.ProgramTypeName = programTypeName;
                }

                // 查询维护者名称
                if (!string.IsNullOrEmpty(item.MaintainUserId))
                {
                    var maintainUserName = await GetUserNameAsync(item.MaintainUserId);
                    dto.MaintainUserName = maintainUserName;
                }

                dtos.Add(dto);
            }

            return new PagedResult<ProgramDto>
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

    public async Task<ProgramDto> GetProgramAsync(string programId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(programId);
            if (entity == null)
            {
                throw new InvalidOperationException($"作業不存在: {programId}");
            }

            var dto = new ProgramDto
            {
                TKey = entity.TKey,
                ProgramId = entity.ProgramId,
                ProgramName = entity.ProgramName,
                SeqNo = entity.SeqNo,
                MenuId = entity.MenuId,
                ProgramUrl = entity.ProgramUrl,
                ProgramType = entity.ProgramType,
                MaintainUserId = entity.MaintainUserId,
                Status = entity.Status,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };

            // 查询子系统名称
            if (!string.IsNullOrEmpty(entity.MenuId))
            {
                dto.MenuName = await GetMenuNameAsync(entity.MenuId);
            }

            // 查询作业型态名称
            if (!string.IsNullOrEmpty(entity.ProgramType))
            {
                dto.ProgramTypeName = await GetProgramTypeNameAsync(entity.ProgramType);
            }

            // 查询维护者名称
            if (!string.IsNullOrEmpty(entity.MaintainUserId))
            {
                dto.MaintainUserName = await GetUserNameAsync(entity.MaintainUserId);
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task<string> CreateProgramAsync(CreateProgramDto dto)
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

            // 檢查子系統是否存在
            var menuExists = await _menuRepository.ExistsAsync(dto.MenuId);
            if (!menuExists)
            {
                throw new InvalidOperationException($"子系統不存在: {dto.MenuId}");
            }

            var entity = new Program
            {
                ProgramId = dto.ProgramId,
                ProgramName = dto.ProgramName,
                SeqNo = dto.SeqNo,
                MenuId = dto.MenuId,
                ProgramUrl = dto.ProgramUrl,
                ProgramType = dto.ProgramType,
                MaintainUserId = GetCurrentUserId(),
                Status = "1",
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = null
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

    public async Task UpdateProgramAsync(string programId, UpdateProgramDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(programId);
            if (entity == null)
            {
                throw new InvalidOperationException($"作業不存在: {programId}");
            }

            // 檢查子系統是否存在
            var menuExists = await _menuRepository.ExistsAsync(dto.MenuId);
            if (!menuExists)
            {
                throw new InvalidOperationException($"子系統不存在: {dto.MenuId}");
            }

            entity.ProgramName = dto.ProgramName;
            entity.SeqNo = dto.SeqNo;
            entity.MenuId = dto.MenuId;
            entity.ProgramUrl = dto.ProgramUrl;
            entity.ProgramType = dto.ProgramType;
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

    public async Task DeleteProgramAsync(string programId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(programId);
            if (entity == null)
            {
                throw new InvalidOperationException($"作業不存在: {programId}");
            }

            // 檢查是否有按鈕關聯
            var hasButtons = await _repository.HasButtonsAsync(programId);
            if (hasButtons)
            {
                throw new InvalidOperationException("此作業下存在按鈕，無法刪除");
            }

            await _repository.DeleteAsync(programId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除作業失敗: {programId}", ex);
            throw;
        }
    }

    public async Task DeleteProgramsBatchAsync(BatchDeleteProgramsDto dto)
    {
        try
        {
            foreach (var programId in dto.ProgramIds)
            {
                await DeleteProgramAsync(programId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除作業失敗", ex);
            throw;
        }
    }

    private async Task<string?> GetMenuNameAsync(string menuId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT MenuName
                FROM Menus
                WHERE MenuId = @MenuId";

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { MenuId = menuId });
            return result;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> GetProgramTypeNameAsync(string programType)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT Content
                FROM Parameters
                WHERE Title = 'PROG_TYPE' AND Tag = @ProgramType";

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { ProgramType = programType });
            return result;
        }
        catch
        {
            return null;
        }
    }

    private async Task<string?> GetUserNameAsync(string userId)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                SELECT UserName
                FROM Users
                WHERE UserId = @UserId";

            var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { UserId = userId });
            return result;
        }
        catch
        {
            return null;
        }
    }

    private void ValidateCreateDto(CreateProgramDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.ProgramId))
        {
            throw new ArgumentException("作業代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ProgramName))
        {
            throw new ArgumentException("作業名稱不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.MenuId))
        {
            throw new ArgumentException("子系統項目代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.ProgramUrl))
        {
            throw new ArgumentException("網頁位址不能為空");
        }
    }
}
