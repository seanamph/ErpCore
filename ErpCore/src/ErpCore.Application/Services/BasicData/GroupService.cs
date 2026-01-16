using Dapper;
using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
using ErpCore.Infrastructure.Data;
using ErpCore.Infrastructure.Repositories.BasicData;
using ErpCore.Shared.Common;
using ErpCore.Shared.Logging;

namespace ErpCore.Application.Services.BasicData;

/// <summary>
/// 組別服務實作
/// </summary>
public class GroupService : BaseService, IGroupService
{
    private readonly IGroupRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public GroupService(
        IGroupRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<GroupDto>> GetGroupsAsync(GroupQueryDto query)
    {
        try
        {
            var repositoryQuery = new GroupQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                GroupId = query.GroupId,
                GroupName = query.GroupName,
                DeptId = query.DeptId,
                OrgId = query.OrgId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 需要從查詢結果中獲取 DeptName 和 OrgName
            var dtos = new List<GroupDto>();
            using var conn = _connectionFactory.CreateConnection();
            
            foreach (var item in result.Items)
            {
                var dto = new GroupDto
                {
                    GroupId = item.GroupId,
                    GroupName = item.GroupName,
                    DeptId = item.DeptId,
                    OrgId = item.OrgId,
                    SeqNo = item.SeqNo,
                    Status = item.Status,
                    Notes = item.Notes,
                    CreatedBy = item.CreatedBy,
                    CreatedAt = item.CreatedAt,
                    UpdatedBy = item.UpdatedBy,
                    UpdatedAt = item.UpdatedAt
                };

                // 查詢部別名稱
                if (!string.IsNullOrEmpty(item.DeptId))
                {
                    var deptName = await conn.QueryFirstOrDefaultAsync<string>(
                        "SELECT DeptName FROM Departments WHERE DeptId = @DeptId",
                        new { DeptId = item.DeptId });
                    if (!string.IsNullOrEmpty(deptName))
                    {
                        dto.DeptName = deptName;
                    }
                }

                // 查詢組織名稱
                if (!string.IsNullOrEmpty(item.OrgId))
                {
                    var orgName = await conn.QueryFirstOrDefaultAsync<string>(
                        "SELECT OrgName FROM Organizations WHERE OrgId = @OrgId",
                        new { OrgId = item.OrgId });
                    if (!string.IsNullOrEmpty(orgName))
                    {
                        dto.OrgName = orgName;
                    }
                }

                dtos.Add(dto);
            }

            return new PagedResult<GroupDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢組別列表失敗", ex);
            throw;
        }
    }

    public async Task<GroupDto> GetGroupByIdAsync(string groupId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(groupId);
            if (entity == null)
            {
                throw new InvalidOperationException($"組別不存在: {groupId}");
            }

            var dto = new GroupDto
            {
                GroupId = entity.GroupId,
                GroupName = entity.GroupName,
                DeptId = entity.DeptId,
                OrgId = entity.OrgId,
                SeqNo = entity.SeqNo,
                Status = entity.Status,
                Notes = entity.Notes,
                CreatedBy = entity.CreatedBy,
                CreatedAt = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedAt = entity.UpdatedAt
            };

            // 查詢部別名稱
            if (!string.IsNullOrEmpty(entity.DeptId))
            {
                using var conn = _connectionFactory.CreateConnection();
                var deptName = await conn.QueryFirstOrDefaultAsync<string>(
                    "SELECT DeptName FROM Departments WHERE DeptId = @DeptId",
                    new { DeptId = entity.DeptId });
                if (!string.IsNullOrEmpty(deptName))
                {
                    dto.DeptName = deptName;
                }
            }

            // 查詢組織名稱
            if (!string.IsNullOrEmpty(entity.OrgId))
            {
                using var conn = _connectionFactory.CreateConnection();
                var orgName = await conn.QueryFirstOrDefaultAsync<string>(
                    "SELECT OrgName FROM Organizations WHERE OrgId = @OrgId",
                    new { OrgId = entity.OrgId });
                if (!string.IsNullOrEmpty(orgName))
                {
                    dto.OrgName = orgName;
                }
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError($"查詢組別失敗: {groupId}", ex);
            throw;
        }
    }

    public async Task<string> CreateGroupAsync(CreateGroupDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.GroupId);
            if (exists)
            {
                throw new InvalidOperationException($"組別已存在: {dto.GroupId}");
            }

            var entity = new Group
            {
                GroupId = dto.GroupId,
                GroupName = dto.GroupName,
                DeptId = dto.DeptId,
                OrgId = dto.OrgId,
                SeqNo = dto.SeqNo ?? 0,
                Status = dto.Status ?? "A",
                Notes = dto.Notes,
                CreatedBy = GetCurrentUserId(),
                CreatedAt = DateTime.Now,
                UpdatedBy = GetCurrentUserId(),
                UpdatedAt = DateTime.Now,
                CreatedPriority = null,
                CreatedGroup = GetCurrentOrgId()
            };

            await _repository.CreateAsync(entity);

            return entity.GroupId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增組別失敗: {dto.GroupId}", ex);
            throw;
        }
    }

    public async Task UpdateGroupAsync(string groupId, UpdateGroupDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(groupId);
            if (entity == null)
            {
                throw new InvalidOperationException($"組別不存在: {groupId}");
            }

            entity.GroupName = dto.GroupName;
            entity.DeptId = dto.DeptId;
            entity.OrgId = dto.OrgId;
            entity.SeqNo = dto.SeqNo ?? 0;
            entity.Status = dto.Status ?? "A";
            entity.Notes = dto.Notes;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"修改組別失敗: {groupId}", ex);
            throw;
        }
    }

    public async Task DeleteGroupAsync(string groupId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(groupId);
            if (entity == null)
            {
                throw new InvalidOperationException($"組別不存在: {groupId}");
            }

            await _repository.DeleteAsync(groupId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除組別失敗: {groupId}", ex);
            throw;
        }
    }

    public async Task DeleteGroupsBatchAsync(BatchDeleteGroupDto dto)
    {
        try
        {
            foreach (var groupId in dto.GroupIds)
            {
                await DeleteGroupAsync(groupId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除組別失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string groupId, UpdateGroupStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(groupId);
            if (entity == null)
            {
                throw new InvalidOperationException($"組別不存在: {groupId}");
            }

            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新組別狀態失敗: {groupId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateGroupDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.GroupId))
        {
            throw new ArgumentException("組別代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.GroupName))
        {
            throw new ArgumentException("組別名稱不能為空");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A (啟用) 或 I (停用)");
        }
    }
}
