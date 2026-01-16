using ErpCore.Application.DTOs.BasicData;
using ErpCore.Application.Services.Base;
using ErpCore.Domain.Entities.BasicData;
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

    public DepartmentService(
        IDepartmentRepository repository,
        ILoggerService logger,
        IUserContext userContext) : base(logger, userContext)
    {
        _repository = repository;
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
                DeptId = query.Filters?.DeptId,
                DeptName = query.Filters?.DeptName,
                OrgId = query.Filters?.OrgId,
                Status = query.Filters?.Status
            };

            var result = await _repository.QueryAsync(repositoryQuery);

            var dtos = result.Items.Select(x => new DepartmentDto
            {
                DeptId = x.DeptId,
                DeptName = x.DeptName,
                OrgId = x.OrgId,
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

    public async Task<DepartmentDto> GetDepartmentAsync(string deptId)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(deptId);
            if (entity == null)
            {
                throw new InvalidOperationException($"部別不存在: {deptId}");
            }

            return new DepartmentDto
            {
                DeptId = entity.DeptId,
                DeptName = entity.DeptName,
                OrgId = entity.OrgId,
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

    public async Task UpdateStatusAsync(string deptId, string status)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(deptId);
            if (entity == null)
            {
                throw new InvalidOperationException($"部別不存在: {deptId}");
            }

            if (status != "A" && status != "I")
            {
                throw new ArgumentException("狀態值必須為 A (啟用) 或 I (停用)");
            }

            entity.Status = status;
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
    }
}
