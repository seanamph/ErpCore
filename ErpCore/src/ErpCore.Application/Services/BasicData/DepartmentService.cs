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
/// 部別服務實作
/// </summary>
public class DepartmentService : BaseService, IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly IDbConnectionFactory _connectionFactory;

    public DepartmentService(
        IDepartmentRepository repository,
        IDbConnectionFactory connectionFactory,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
        _connectionFactory = connectionFactory;
    }

    public async Task<PagedResult<DepartmentDto>> GetDepartmentsAsync(DepartmentQueryDto query)
    {
        try
        {
            var repositoryQuery = new DepartmentQuery
            {
                PageIndex = query.PageIndex,
                PageSize = query.PageSize,
                SortField = query.SortField,
                SortOrder = query.SortOrder,
                DeptId = query.DeptId,
                DeptName = query.DeptName,
                OrgId = query.OrgId,
                Status = query.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            // 查詢組織名稱（如果有 OrgId）
            var orgIds = result.Items.Where(x => !string.IsNullOrEmpty(x.OrgId)).Select(x => x.OrgId!).Distinct().ToList();
            var orgNameMap = new Dictionary<string, string>();
            
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

            var dtos = result.Items.Select(x => new DepartmentDto
            {
                DeptId = x.DeptId,
                DeptName = x.DeptName,
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

            return new PagedResult<DepartmentDto>
            {
                Items = dtos,
                TotalCount = result.TotalCount,
                PageIndex = result.PageIndex,
                PageSize = result.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("查詢部別列表失敗", ex);
            throw;
        }
    }

    public async Task<DepartmentDto> GetDepartmentByIdAsync(string deptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(deptId);
            if (entity == null)
            {
                throw new InvalidOperationException($"部別不存在: {deptId}");
            }

            // 查詢組織名稱
            string? orgName = null;
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

            return new DepartmentDto
            {
                DeptId = entity.DeptId,
                DeptName = entity.DeptName,
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
            _logger.LogError($"查詢部別失敗: {deptId}", ex);
            throw;
        }
    }

    public async Task<string> CreateDepartmentAsync(CreateDepartmentDto dto)
    {
        try
        {
            // 驗證資料
            ValidateCreateDto(dto);

            // 檢查是否已存在
            var exists = await _repository.ExistsAsync(dto.DeptId);
            if (exists)
            {
                throw new InvalidOperationException($"部別已存在: {dto.DeptId}");
            }

            var entity = new Department
            {
                DeptId = dto.DeptId,
                DeptName = dto.DeptName,
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

            return entity.DeptId;
        }
        catch (Exception ex)
        {
            _logger.LogError($"新增部別失敗: {dto.DeptId}", ex);
            throw;
        }
    }

    public async Task UpdateDepartmentAsync(string deptId, UpdateDepartmentDto dto)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(deptId);
            if (entity == null)
            {
                throw new InvalidOperationException($"部別不存在: {deptId}");
            }

            entity.DeptName = dto.DeptName;
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
            _logger.LogError($"修改部別失敗: {deptId}", ex);
            throw;
        }
    }

    public async Task DeleteDepartmentAsync(string deptId)
    {
        try
        {
            // 檢查是否存在
            var entity = await _repository.GetByIdAsync(deptId);
            if (entity == null)
            {
                throw new InvalidOperationException($"部別不存在: {deptId}");
            }

            await _repository.DeleteAsync(deptId);
        }
        catch (Exception ex)
        {
            _logger.LogError($"刪除部別失敗: {deptId}", ex);
            throw;
        }
    }

    public async Task DeleteDepartmentsBatchAsync(BatchDeleteDepartmentDto dto)
    {
        try
        {
            foreach (var deptId in dto.DeptIds)
            {
                await DeleteDepartmentAsync(deptId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("批次刪除部別失敗", ex);
            throw;
        }
    }

    public async Task UpdateStatusAsync(string deptId, UpdateDepartmentStatusDto dto)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(deptId);
            if (entity == null)
            {
                throw new InvalidOperationException($"部別不存在: {deptId}");
            }

            entity.Status = dto.Status;
            entity.UpdatedBy = GetCurrentUserId();
            entity.UpdatedAt = DateTime.Now;

            await _repository.UpdateAsync(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError($"更新部別狀態失敗: {deptId}", ex);
            throw;
        }
    }

    private void ValidateCreateDto(CreateDepartmentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.DeptId))
        {
            throw new ArgumentException("部別代碼不能為空");
        }

        if (string.IsNullOrWhiteSpace(dto.DeptName))
        {
            throw new ArgumentException("部別名稱不能為空");
        }

        if (!string.IsNullOrEmpty(dto.Status) && dto.Status != "A" && dto.Status != "I")
        {
            throw new ArgumentException("狀態值必須為 A (啟用) 或 I (停用)");
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

