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

            // 查詢部別名稱和組織名稱
            var deptIds = result.Items.Where(x => !string.IsNullOrEmpty(x.DeptId)).Select(x => x.DeptId!).Distinct().ToList();
            var orgIds = result.Items.Where(x => !string.IsNullOrEmpty(x.OrgId)).Select(x => x.OrgId!).Distinct().ToList();
            
            var deptNameMap = new Dictionary<string, string>();
            var orgNameMap = new Dictionary<string, string>();

            if (deptIds.Any())
            {
                try
                {
                    var deptNames = await QueryDeptNamesAsync(deptIds);
                    deptNameMap = deptNames.ToDictionary(x => x.DeptId, x => x.DeptName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("查詢部別名稱失敗，將使用空值", ex);
                }
            }

            if (orgIds.Any())
            {
                try
                {
                    var orgNames = await QueryOrgNamesAsync(orgIds);
                    orgNameMap = orgNames.ToDictionary(x => x.OrgId, x => x.OrgName);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("查詢組織名稱失敗，將使用空值", ex);
                }
            }

            var dtos = result.Items.Select(x => new GroupDto
            {
                GroupId = x.GroupId,
                GroupName = x.GroupName,
                DeptId = x.DeptId,
                DeptName = x.DeptId != null && deptNameMap.ContainsKey(x.DeptId) ? deptNameMap[x.DeptId] : null,
                OrgId = x.OrgId,
                OrgName = x.OrgId != null && orgNameMap.ContainsKey(x.OrgId) ? orgNameMap[x.OrgId] : null,
                SeqNo = x.SeqNo,
                Status = x.Status,
                Notes = x.Notes,
                CreatedBy = x.CreatedBy,
                CreatedAt = x.CreatedAt,
                UpdatedBy = x.UpdatedBy,
                UpdatedAt = x.UpdatedAt
            }).ToList();

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

            // 查詢部別名稱和組織名稱
            string? deptName = null;
            string? orgName = null;

            if (!string.IsNullOrEmpty(entity.DeptId))
            {
                try
                {
                    var deptNames = await QueryDeptNamesAsync(new List<string> { entity.DeptId });
                    deptName = deptNames.FirstOrDefault().DeptName;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"查詢部別名稱失敗: {entity.DeptId}", ex);
                }
            }

            if (!string.IsNullOrEmpty(entity.OrgId))
            {
                try
                {
                    var orgNames = await QueryOrgNamesAsync(new List<string> { entity.OrgId });
                    orgName = orgNames.FirstOrDefault().OrgName;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"查詢組織名稱失敗: {entity.OrgId}", ex);
                }
            }

            return new GroupDto
            {
                GroupId = entity.GroupId,
                GroupName = entity.GroupName,
                DeptId = entity.DeptId,
                DeptName = deptName,
                OrgId = entity.OrgId,
                OrgName = orgName,
                SeqNo = entity.SeqNo,
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

    /// <summary>
    /// 查詢部別名稱
    /// </summary>
    private async Task<List<(string DeptId, string DeptName)>> QueryDeptNamesAsync(List<string> deptIds)
    {
        try
        {
            if (!deptIds.Any())
            {
                return new List<(string, string)>();
            }

            const string sql = @"
                SELECT DeptId, DeptName 
                FROM Departments 
                WHERE DeptId IN @DeptIds";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<(string DeptId, string DeptName)>(sql, new { DeptIds = deptIds });
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢部別名稱失敗", ex);
            throw;
        }
    }

    /// <summary>
    /// 查詢組織名稱
    /// </summary>
    private async Task<List<(string OrgId, string OrgName)>> QueryOrgNamesAsync(List<string> orgIds)
    {
        try
        {
            if (!orgIds.Any())
            {
                return new List<(string, string)>();
            }

            const string sql = @"
                SELECT OrgId, OrgName 
                FROM Organizations 
                WHERE OrgId IN @OrgIds";

            using var connection = _connectionFactory.CreateConnection();
            var results = await connection.QueryAsync<(string OrgId, string OrgName)>(sql, new { OrgIds = orgIds });
            return results.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢組織名稱失敗", ex);
            throw;
        }
    }
}

